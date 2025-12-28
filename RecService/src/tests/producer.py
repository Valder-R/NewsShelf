"""
Test producer for RabbitMQ events
"""

import pika
import json
import time
from typing import Optional


def send_event(
    event_type: str,
    user_id: int,
    news_id: int,
    host: str = "localhost",
    queue_name: str = "news_events"
):
    """Send a test event to RabbitMQ
    
    Args:
        event_type: Type of event (e.g., 'news_viewed')
        user_id: User ID
        news_id: News ID
        host: RabbitMQ host
        queue_name: Queue name
    """
    try:
        connection = pika.BlockingConnection(
            pika.ConnectionParameters(host=host)
        )
        channel = connection.channel()
        
        # Declare queue
        channel.queue_declare(queue=queue_name, durable=True)
        
        # Create message
        message = {
            "event": event_type,
            "user_id": user_id,
            "news_id": news_id,
            "timestamp": time.time()
        }
        
        # Publish message
        channel.basic_publish(
            exchange="",
            routing_key=queue_name,
            body=json.dumps(message),
            properties=pika.BasicProperties(
                delivery_mode=pika.spec.PERSISTENT_DELIVERY_MODE
            )
        )
        
        print(f"✅ Event sent: {message}")
        connection.close()
        return True
        
    except Exception as e:
        print(f"❌ Error sending event: {e}")
        return False


def send_multiple_events(
    user_id: int,
    news_ids: list,
    host: str = "localhost"
):
    """Send multiple news view events for a user
    
    Args:
        user_id: User ID
        news_ids: List of news IDs to view
        host: RabbitMQ host
    """
    for news_id in news_ids:
        send_event(
            event_type="news_viewed",
            user_id=user_id,
            news_id=news_id,
            host=host
        )
        time.sleep(0.5)  # Small delay between events


def simulate_user_behavior(
    user_id: int,
    num_events: int = 10,
    host: str = "localhost"
):
    """Simulate user viewing random news
    
    Args:
        user_id: User ID
        num_events: Number of events to generate
        host: RabbitMQ host
    """
    import random
    
    print(f"📊 Simulating {num_events} events for user {user_id}...")
    
    for _ in range(num_events):
        news_id = random.randint(1, 1000)  # Random news ID
        send_event(
            event_type="news_viewed",
            user_id=user_id,
            news_id=news_id,
            host=host
        )
        time.sleep(0.2)
    
    print(f"✅ Simulation completed")


if __name__ == "__main__":
    import sys
    
    if len(sys.argv) > 1:
        if sys.argv[1] == "single":
            # Send single event
            user_id = int(sys.argv[2]) if len(sys.argv) > 2 else 1
            news_id = int(sys.argv[3]) if len(sys.argv) > 3 else 10
            send_event("news_viewed", user_id, news_id)
        
        elif sys.argv[1] == "simulate":
            # Simulate user behavior
            user_id = int(sys.argv[2]) if len(sys.argv) > 2 else 1
            num_events = int(sys.argv[3]) if len(sys.argv) > 3 else 10
            simulate_user_behavior(user_id, num_events)
        
        elif sys.argv[1] == "batch":
            # Send batch events
            user_id = int(sys.argv[2]) if len(sys.argv) > 2 else 1
            news_ids = [int(x) for x in sys.argv[3:]]
            send_multiple_events(user_id, news_ids)
    else:
        print("Usage:")
        print("  python producer.py single [user_id] [news_id]")
        print("  python producer.py simulate [user_id] [num_events]")
        print("  python producer.py batch [user_id] [news_id1] [news_id2] ...")
        print("\nExample:")
        print("  python producer.py single 1 5")
        print("  python producer.py simulate 1 20")
