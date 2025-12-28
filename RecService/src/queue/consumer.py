"""
RabbitMQ consumer for processing user activity events
"""

import json
import time
import pika
from pika.exceptions import AMQPConnectionError

from src.db import SessionLocal
from src.services.recommendation_service import process_news_view
from src.logger import logger
from src.config import get_settings

settings = get_settings()


def wait_for_rabbitmq(max_retries: int = 30):
    """Wait for RabbitMQ to be ready
    
    Args:
        max_retries: Maximum number of retry attempts
    """
    logger.info("⏳ Waiting for RabbitMQ...")
    
    for attempt in range(max_retries):
        try:
            connection = pika.BlockingConnection(
                pika.URLParameters(settings.rabbitmq_url)
            )
            connection.close()
            logger.info("✅ RabbitMQ is ready")
            return
        except AMQPConnectionError:
            logger.warning(f"RabbitMQ not ready, retrying in 5s... (attempt {attempt + 1}/{max_retries})")
            time.sleep(5)
    
    raise Exception("❌ RabbitMQ connection failed after all retries")


def callback(ch, method, properties, body):
    """Callback for processing incoming messages
    
    Args:
        ch: Channel
        method: Delivery method
        properties: Message properties
        body: Message body
    """
    try:
        event = json.loads(body)
        event_type = event.get("event") or event.get("EventType") or "unknown"
        logger.info(f"📩 Received event: {event_type} - {event}")
        
        # Process different event types
        if event_type in ["news_viewed", "NewsViewed"]:
            process_news_view(
                user_id=event.get("user_id") or event.get("UserId"),
                news_id=event.get("news_id") or event.get("NewsId")
            )
            logger.info(f"✅ Processed news view: user={event.get('user_id')} news={event.get('news_id')}")
        
        elif event_type in ["user_registered", "UserRegistered"]:
            logger.info(f"👤 User registered: {event.get('UserId')} - {event.get('Email')}")
            # TODO: Initialize user recommendations
        
        elif event_type in ["favorite_topics_added", "FavoriteTopicAdded"]:
            logger.info(f"⭐ Favorite topics added for user {event.get('UserId')}: {event.get('Topics')}")
            # TODO: Update recommendations based on favorite topics
        
        elif event_type in ["news_searched", "NewsSearched"]:
            logger.info(f"🔍 News search: user={event.get('UserId')} query={event.get('SearchQuery')}")
            # TODO: Track search behavior for recommendations
        
        else:
            logger.warning(f"Unknown event type: {event_type}")
        
        # Acknowledge message
        ch.basic_ack(delivery_tag=method.delivery_tag)
        
    except json.JSONDecodeError as e:
        logger.error(f"❌ Invalid JSON message: {e}")
        ch.basic_nack(delivery_tag=method.delivery_tag, requeue=False)
    except Exception as e:
        logger.error(f"❌ Error processing message: {e}")
        ch.basic_nack(delivery_tag=method.delivery_tag, requeue=True)


def main():
    """Main consumer loop"""
    logger.info("🚀 Starting Recommendation Consumer...")
    
    # Wait for RabbitMQ
    wait_for_rabbitmq()
    
    try:
        # Connect to RabbitMQ
        connection = pika.BlockingConnection(
            pika.URLParameters(settings.rabbitmq_url)
        )
        channel = connection.channel()
        
        # Declare queue
        channel.queue_declare(
            queue=settings.queue_name,
            durable=True
        )
        
        # Set QoS (process one message at a time)
        channel.basic_qos(prefetch_count=1)
        
        # Subscribe to queue
        channel.basic_consume(
            queue=settings.queue_name,
            on_message_callback=callback
        )
        
        logger.info(f"🚀 Waiting for messages on queue '{settings.queue_name}'...")
        channel.start_consuming()
        
    except KeyboardInterrupt:
        logger.info("⏹️  Shutting down consumer...")
    except Exception as e:
        logger.error(f"❌ Fatal error in consumer: {e}")
        raise
    finally:
        if 'connection' in locals() and connection.is_open:
            connection.close()
            logger.info("✅ Consumer stopped")


if __name__ == "__main__":
    main()
