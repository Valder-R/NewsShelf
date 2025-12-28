"""
Initialize database: create tables and import data
"""

import time
import sys
from src.db import engine, Base
from src.scripts.import_news import import_news
from src.scripts.generate_embeddings import generate_embeddings
from src.logger import logger


def main():
    """Initialize database"""
    logger.info("⏳ Waiting for database to be ready...")
    time.sleep(5)  # Give PostgreSQL time to start

    try:
        # Create tables
        logger.info("📦 Creating tables...")
        Base.metadata.create_all(bind=engine)
        logger.info("✅ Tables created successfully")
        
        # Import news
        logger.info("📰 Importing news dataset...")
        import_news()
        
        # Generate embeddings
        logger.info("🧠 Generating embeddings...")
        generate_embeddings()
        
        logger.info("✅ Database fully initialized!")
        return 0
        
    except Exception as e:
        logger.error(f"❌ Database initialization failed: {e}")
        return 1


if __name__ == "__main__":
    exit_code = main()
    sys.exit(exit_code)
