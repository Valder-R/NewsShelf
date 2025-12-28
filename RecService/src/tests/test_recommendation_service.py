"""
Unit tests for recommendation service
"""

import pytest
import json
from datetime import datetime, timedelta
import pickle
import numpy as np

from src.services.recommendation_service import (
    process_news_view,
    get_user_activity,
    get_user_interests,
    get_recommendations
)
from src.models.news import News
from src.models.user_activity import UserActivity
from src.db import SessionLocal, Base, engine


# Setup and teardown
@pytest.fixture
def db_session():
    """Create a fresh database for each test"""
    Base.metadata.create_all(bind=engine)
    session = SessionLocal()
    yield session
    session.close()
    Base.metadata.drop_all(bind=engine)


@pytest.fixture
def sample_news(db_session):
    """Create sample news for testing"""
    news_items = [
        News(
            id=1,
            category="Technology",
            title="AI News 1",
            short_description="Latest AI developments",
            authors="Author 1",
            link="http://example.com/1",
            date=datetime.now().date()
        ),
        News(
            id=2,
            category="Technology",
            title="AI News 2",
            short_description="Machine learning advances",
            authors="Author 2",
            link="http://example.com/2",
            date=datetime.now().date()
        ),
        News(
            id=3,
            category="Sports",
            title="Sports News",
            short_description="Latest sports updates",
            authors="Author 3",
            link="http://example.com/3",
            date=datetime.now().date()
        ),
    ]
    
    for news in news_items:
        # Create dummy embeddings
        dummy_embedding = np.random.randn(384)
        news.description_embedding = pickle.dumps(dummy_embedding)
        db_session.add(news)
    
    db_session.commit()
    return news_items


class TestRecommendationService:
    """Test cases for recommendation service"""
    
    def test_process_news_view(self, db_session, sample_news):
        """Test processing news view event"""
        process_news_view(user_id=1, news_id=1, db=db_session)
        
        activity = db_session.query(UserActivity).filter_by(
            user_id=1,
            news_id=1
        ).first()
        
        assert activity is not None
        assert activity.activity_type == "view"
    
    def test_get_user_activity(self, db_session, sample_news):
        """Test retrieving user activity"""
        # Create some activities
        for i in range(3):
            activity = UserActivity(
                user_id=1,
                news_id=i + 1,
                activity_type="view",
                timestamp=datetime.utcnow()
            )
            db_session.add(activity)
        db_session.commit()
        
        activities = get_user_activity(user_id=1, db=db_session)
        assert len(activities) == 3
    
    def test_get_user_interests(self, db_session, sample_news):
        """Test calculating user interests"""
        # Create activities across different categories
        for i in range(2):
            activity = UserActivity(
                user_id=1,
                news_id=1,  # Technology
                activity_type="view",
                timestamp=datetime.utcnow()
            )
            db_session.add(activity)
        
        activity = UserActivity(
            user_id=1,
            news_id=3,  # Sports
            activity_type="view",
            timestamp=datetime.utcnow()
        )
        db_session.add(activity)
        db_session.commit()
        
        interests = get_user_interests(user_id=1, db=db_session)
        
        assert "Technology" in interests
        assert "Sports" in interests
        # Technology should have higher weight
        assert interests["Technology"] > interests["Sports"]
    
    def test_no_activity_returns_popular(self, db_session, sample_news):
        """Test that users with no activity get popular news"""
        # Increment view count
        sample_news[0].view_count = 100
        sample_news[1].view_count = 50
        sample_news[2].view_count = 25
        db_session.commit()
        
        recommendations = get_recommendations(user_id=999, count=2, db=db_session)
        
        # Should return popular news
        assert len(recommendations) > 0


class TestEmbeddingModel:
    """Test cases for embedding model"""
    
    def test_embedding_dimensions(self):
        """Test that embeddings have correct dimensions"""
        from src.ml.embedding_model import get_embedding_model
        
        model = get_embedding_model()
        embedding = model.encode("Test text")
        
        assert embedding.shape == (384,)  # all-MiniLM-L6-v2 outputs 384 dimensions
    
    def test_embedding_similarity(self):
        """Test similarity calculation"""
        from src.ml.embedding_model import get_embedding_model
        
        model = get_embedding_model()
        
        text1 = "Machine learning is a subset of AI"
        text2 = "Machine learning is a subset of AI"
        text3 = "The weather is nice today"
        
        emb1 = model.encode(text1)
        emb2 = model.encode(text2)
        emb3 = model.encode(text3)
        
        sim_same = model.similarity(emb1, emb2)
        sim_diff = model.similarity(emb1, emb3)
        
        # Same text should have higher similarity
        assert sim_same > sim_diff
        assert sim_same > 0.9
        assert sim_diff < 0.5


if __name__ == "__main__":
    pytest.main([__file__, "-v"])
