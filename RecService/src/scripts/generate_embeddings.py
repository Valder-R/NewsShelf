"""
Generate and store embeddings for all news articles
"""

import pickle
import time
from src.db import SessionLocal
from src.models.news import News
from src.ml.embedding_model import get_embedding_model
from src.logger import logger


def generate_embeddings():
    """Generate embeddings for all news without embeddings"""
    
    logger.info("🚀 Starting embedding generation...")
    
    db = SessionLocal()
    embedding_model = get_embedding_model()
    
    try:
        # Get all news without embeddings
        news_without_embeddings = db.query(News).filter(
            News.description_embedding == None
        ).all()
        
        total = len(news_without_embeddings)
        logger.info(f"📰 Found {total} news articles to process")
        
        if total == 0:
            logger.info("✅ All news articles already have embeddings")
            return
        
        # Process in batches
        batch_size = 50
        for i in range(0, total, batch_size):
            batch = news_without_embeddings[i:i+batch_size]
            logger.info(f"Processing batch {i//batch_size + 1}/{(total + batch_size - 1)//batch_size}")
            
            for news in batch:
                try:
                    # Generate embeddings
                    if news.short_description:
                        description_embedding = embedding_model.encode(news.short_description)
                        news.description_embedding = pickle.dumps(description_embedding)
                    
                    if news.title:
                        title_embedding = embedding_model.encode(news.title)
                        news.title_embedding = pickle.dumps(title_embedding)
                    
                    news.embedding_generated = 1
                    
                except Exception as e:
                    logger.error(f"❌ Error processing news {news.id}: {e}")
                    continue
            
            # Commit batch
            try:
                db.commit()
                logger.info(f"✅ Batch committed: {len(batch)} news processed")
            except Exception as e:
                db.rollback()
                logger.error(f"❌ Error committing batch: {e}")
        
        logger.info(f"✅ Embedding generation completed for {total} articles")
        
    except Exception as e:
        logger.error(f"❌ Fatal error during embedding generation: {e}")
        db.rollback()
        raise
    finally:
        db.close()


if __name__ == "__main__":
    generate_embeddings()