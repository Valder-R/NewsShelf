"""
Import news from JSON file to database
"""

import json
from datetime import datetime
from src.db import SessionLocal
from src.models.news import News
from src.logger import logger


def import_news(file_path: str = "data/News_Category.json"):
    """Import news from JSON file
    
    Args:
        file_path: Path to the JSON file with news data
    """
    logger.info(f"📂 Loading news from {file_path}...")
    
    db = SessionLocal()
    
    try:
        # Check if news already imported
        existing_count = db.query(News).count()
        if existing_count > 0:
            logger.info(f"ℹ️  Database already contains {existing_count} news articles, skipping import")
            return
        
        # Try to load and import - handle both JSON array and JSONL format
        imported = 0
        
        try:
            # First try as JSON array
            with open(file_path, "r", encoding="utf-8") as f:
                data = json.load(f)
                
            # If it's a list, process each item
            if isinstance(data, list):
                logger.info(f"📰 Processing {len(data)} news articles (JSON array format)...")
                imported = _process_articles(db, data)
            else:
                # Single object
                logger.info("📰 Processing 1 news article...")
                imported = _process_articles(db, [data])
                
        except json.JSONDecodeError:
            # Try JSONL format (one JSON object per line)
            logger.info("Trying JSONL format (one object per line)...")
            data = []
            with open(file_path, "r", encoding="utf-8") as f:
                for line_num, line in enumerate(f, 1):
                    line = line.strip()
                    if not line:
                        continue
                    try:
                        data.append(json.loads(line))
                    except json.JSONDecodeError as e:
                        logger.warning(f"⚠️  Error parsing line {line_num}: {e}")
                        continue
            
            if data:
                logger.info(f"📰 Processing {len(data)} news articles (JSONL format)...")
                imported = _process_articles(db, data)
        
        if imported > 0:
            logger.info(f"✅ Imported {imported} news articles successfully")
        else:
            logger.warning("⚠️  No news articles were imported")
        
    except FileNotFoundError:
        logger.error(f"❌ File not found: {file_path}")
        raise
    except Exception as e:
        logger.error(f"❌ Error importing news: {e}")
        db.rollback()
        raise
    finally:
        db.close()


def _process_articles(db: SessionLocal, articles: list) -> int:
    """Process and save articles to database
    
    Args:
        db: Database session
        articles: List of article dictionaries
        
    Returns:
        Number of articles imported
    """
    imported = 0
    
    for idx, item in enumerate(articles):
        try:
            # Parse date
            date_str = item.get("date", "")
            if isinstance(date_str, str):
                try:
                    parsed_date = datetime.strptime(date_str, "%Y-%m-%d").date()
                except:
                    parsed_date = datetime.now().date()
            else:
                parsed_date = datetime.now().date()
            
            # Create news object
            news = News(
                category=item.get("category", "Unknown"),
                title=item.get("headline", ""),
                short_description=item.get("short_description", ""),
                authors=item.get("authors", ""),
                link=item.get("link", ""),
                date=parsed_date
            )
            
            db.add(news)
            imported += 1
            
            # Commit every 100 items
            if imported % 100 == 0:
                db.commit()
                logger.info(f"  ✓ Processed {imported} articles")
            
        except Exception as e:
            logger.warning(f"⚠️  Error processing item {idx}: {e}")
            continue
    
    # Final commit
    if imported > 0:
        db.commit()
    
    return imported


if __name__ == "__main__":
    import_news()