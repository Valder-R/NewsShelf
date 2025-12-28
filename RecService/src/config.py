"""
Environment configuration for Recommendation Service
"""

from functools import lru_cache
import os
from dotenv import load_dotenv

# Load .env file
load_dotenv()


class Settings:
    """Application settings from environment variables"""
    
    # Database
    database_url: str = os.getenv(
        "DATABASE_URL",
        "postgresql://postgres:postgres@postgres:5432/recommendations"
    )
    
    # RabbitMQ
    rabbitmq_url: str = os.getenv(
        "RABBITMQ_URL",
        "amqp://guest:guest@rabbitmq:5672/"
    )
    queue_name: str = os.getenv("QUEUE_NAME", "news_events")
    
    # API
    api_title: str = os.getenv("API_TITLE", "Recommendation Service API")
    api_version: str = os.getenv("API_VERSION", "1.0.0")
    api_description: str = os.getenv(
        "API_DESCRIPTION",
        "Microservice for news recommendations"
    )
    
    # Embedding Model
    embedding_model_name: str = os.getenv(
        "EMBEDDING_MODEL_NAME",
        "sentence-transformers/all-MiniLM-L6-v2"
    )
    
    # Recommendation settings
    recommendations_count: int = int(os.getenv("RECOMMENDATIONS_COUNT", "10"))
    similarity_threshold: float = float(os.getenv("SIMILARITY_THRESHOLD", "0.3"))
    
    # Logging
    log_level: str = os.getenv("LOG_LEVEL", "INFO")
    log_format: str = os.getenv(
        "LOG_FORMAT",
        "<level>{level: <8}</level> | <cyan>{name}</cyan>:<cyan>{function}</cyan>:<cyan>{line}</cyan> - <level>{message}</level>"
    )


@lru_cache()
def get_settings() -> Settings:
    """Get cached settings instance"""
    return Settings()
