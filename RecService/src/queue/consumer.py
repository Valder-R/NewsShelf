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
        logger.info(f"📩 Received event: {event}")
        
        # Process different event types
        if event.get("event") == "news_viewed":
            process_news_view(
                user_id=event.get("user_id"),
                news_id=event.get("news_id")
            )
        elif event.get("event") == "news_liked":
            logger.info(f"📌 User {event.get('user_id')} liked news {event.get('news_id')}")
            # TODO: Implement like tracking
        else:
            logger.warning(f"Unknown event type: {event.get('event')}")
        
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
