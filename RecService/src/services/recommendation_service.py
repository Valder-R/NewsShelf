"""
Recommendation service - main business logic
"""

from datetime import datetime, timedelta
from typing import List, Tuple
import numpy as np
import pickle

from src.db import SessionLocal
from src.models.user_activity import UserActivity
from src.models.news import News
from src.ml.embedding_model import get_embedding_model
from src.logger import logger
from src.config import get_settings
from sqlalchemy.orm import Session


settings = get_settings()


def process_news_view(user_id: int, news_id: int, db: Session = None):
    """
    Process 'news_viewed' event.
    Stores user activity for recommendations.
    
    Args:
        user_id: ID of the user
        news_id: ID of the news article
        db: Database session
    """
    if db is None:
        db = SessionLocal()
    
    try:
        activity = UserActivity(
            user_id=user_id,
            news_id=news_id,
            activity_type="view",
            timestamp=datetime.utcnow()
        )
        db.add(activity)
        db.commit()
        logger.info(f"✅ Stored activity: user={user_id}, news={news_id}")
    except Exception as e:
        db.rollback()
        logger.error(f"❌ Error processing news view: {e}")
        raise
    finally:
        db.close()


def get_user_activity(user_id: int, days: int = 30, db: Session = None) -> List[UserActivity]:
    """Get user's recent activity
    
    Args:
        user_id: User ID
        days: How many days of history to fetch
        db: Database session
        
    Returns:
        List of user activities
    """
    if db is None:
        db = SessionLocal()
    
    try:
        since_date = datetime.utcnow() - timedelta(days=days)
        activities = db.query(UserActivity).filter(
            UserActivity.user_id == user_id,
            UserActivity.timestamp >= since_date
        ).order_by(UserActivity.timestamp.desc()).all()
        return activities
    except Exception as e:
        logger.error(f"❌ Error fetching user activity: {e}")
        return []
    finally:
        db.close()


def get_user_interests(user_id: int, db: Session = None) -> dict:
    """Get user's category interests based on activity
    
    Args:
        user_id: User ID
        db: Database session
        
    Returns:
        Dictionary with categories and their weights
    """
    if db is None:
        db = SessionLocal()
    
    try:
        activities = db.query(UserActivity).filter(
            UserActivity.user_id == user_id
        ).all()
        
        if not activities:
            return {}
        
        # Get categories from viewed news
        activity_ids = [a.news_id for a in activities]
        news_items = db.query(News).filter(News.id.in_(activity_ids)).all()
        
        # Count by category
        category_counts = {}
        for news in news_items:
            category_counts[news.category] = category_counts.get(news.category, 0) + 1
        
        # Normalize to weights
        total = sum(category_counts.values())
        category_weights = {cat: count / total for cat, count in category_counts.items()}
        
        return category_weights
    except Exception as e:
        logger.error(f"❌ Error getting user interests: {e}")
        return {}
    finally:
        db.close()


def get_recommendations(user_id: int, count: int = None, db: Session = None) -> List[Tuple[News, float]]:
    """Generate recommendations for a user
    
    Args:
        user_id: User ID
        count: Number of recommendations
        db: Database session
        
    Returns:
        List of (News, similarity_score) tuples
    """
    if count is None:
        count = settings.recommendations_count
    
    if db is None:
        db = SessionLocal()
    
    try:
        # Get user's activity
        activities = get_user_activity(user_id, db=db)
        
        if not activities:
            logger.info(f"No activity found for user {user_id}, returning popular news")
            return get_popular_news(count, db=db)
        
        # Get embeddings for viewed news
        viewed_news_ids = [a.news_id for a in activities]
        viewed_news = db.query(News).filter(News.id.in_(viewed_news_ids)).all()
        
        if not viewed_news:
            logger.warning(f"No news found for user {user_id}'s activity")
            return get_popular_news(count, db=db)
        
        # Get embedding model
        embedding_model = get_embedding_model()
        
        # Calculate average embedding from viewed news
        embeddings = []
        for news in viewed_news:
            if news.description_embedding:
                try:
                    embedding = pickle.loads(news.description_embedding)
                    embeddings.append(embedding)
                except:
                    logger.warning(f"Failed to load embedding for news {news.id}")
        
        if not embeddings:
            logger.info("No embeddings available, returning popular news")
            return get_popular_news(count, db=db)
        
        # Average user's embedding
        user_embedding = np.mean(embeddings, axis=0)
        
        # Get all non-viewed news with embeddings
        all_news = db.query(News).filter(
            ~News.id.in_(viewed_news_ids),
            News.description_embedding != None
        ).all()
        
        # Calculate similarities
        recommendations = []
        for news in all_news:
            try:
                news_embedding = pickle.loads(news.description_embedding)
                similarity = embedding_model.similarity(user_embedding, news_embedding)
                
                if similarity >= settings.similarity_threshold:
                    recommendations.append((news, float(similarity)))
            except Exception as e:
                logger.warning(f"Error calculating similarity for news {news.id}: {e}")
        
        # Sort by similarity and return top N
        recommendations.sort(key=lambda x: x[1], reverse=True)
        logger.info(f"✅ Generated {len(recommendations)} recommendations for user {user_id}")
        
        return recommendations[:count]
        
    except Exception as e:
        logger.error(f"❌ Error generating recommendations: {e}")
        return []
    finally:
        db.close()


def get_popular_news(count: int = None, db: Session = None) -> List[Tuple[News, float]]:
    """Get popular news by view count
    
    Args:
        count: Number of news to return
        db: Database session
        
    Returns:
        List of (News, score) tuples where score is based on popularity
    """
    if count is None:
        count = settings.recommendations_count
    
    if db is None:
        db = SessionLocal()
    
    try:
        popular = db.query(News).order_by(
            News.view_count.desc()
        ).limit(count).all()
        
        # Return with popularity score
        if popular:
            max_views = popular[0].view_count or 1
            results = [(news, min(1.0, news.view_count / max_views)) for news in popular]
        else:
            results = []
        
        return results
    except Exception as e:
        logger.error(f"❌ Error getting popular news: {e}")
        return []
    finally:
        db.close()
