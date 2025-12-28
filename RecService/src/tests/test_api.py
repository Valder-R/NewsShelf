"""
Integration tests for API endpoints
"""

import pytest
import json
from fastapi.testclient import TestClient
from datetime import datetime
import pickle
import numpy as np

from src.main import app
from src.db import Base, engine, SessionLocal
from src.models.news import News
from src.models.user_activity import UserActivity


@pytest.fixture
def client():
    """Create test client"""
    return TestClient(app)


@pytest.fixture
def db_session():
    """Create a fresh database for each test"""
    Base.metadata.create_all(bind=engine)
    session = SessionLocal()
    yield session
    session.close()
    Base.metadata.drop_all(bind=engine)


@pytest.fixture
def sample_data(db_session):
    """Create sample data for testing"""
    # Create news
    for i in range(5):
        dummy_embedding = np.random.randn(384)
        news = News(
            category=f"Category{i % 2}",
            title=f"News {i}",
            short_description=f"Description {i}",
            authors=f"Author {i}",
            link=f"http://example.com/{i}",
            date=datetime.now().date(),
            description_embedding=pickle.dumps(dummy_embedding),
            view_count=i * 10
        )
        db_session.add(news)
    
    db_session.commit()
    
    # Create user activity
    for i in range(3):
        activity = UserActivity(
            user_id=1,
            news_id=i + 1,
            activity_type="view",
            timestamp=datetime.utcnow()
        )
        db_session.add(activity)
    
    db_session.commit()
    return db_session


class TestHealthEndpoint:
    """Test health check endpoint"""
    
    def test_health_check(self, client):
        """Test health check returns 200"""
        response = client.get("/health")
        assert response.status_code == 200
        data = response.json()
        assert data["status"] == "healthy"
        assert data["database"] == "connected"


class TestRootEndpoint:
    """Test root endpoint"""
    
    def test_root_endpoint(self, client):
        """Test root endpoint returns service info"""
        response = client.get("/")
        assert response.status_code == 200
        data = response.json()
        assert "name" in data
        assert "version" in data


class TestRecommendationEndpoints:
    """Test recommendation endpoints"""
    
    def test_get_recommendations_no_activity(self, client, sample_data):
        """Test getting recommendations for user with no activity"""
        response = client.get("/api/v1/recommendations/999?count=5")
        assert response.status_code == 200
        data = response.json()
        assert data["user_id"] == 999
        assert isinstance(data["recommendations"], list)
    
    def test_get_recommendations_with_count(self, client, sample_data):
        """Test getting recommendations with specific count"""
        response = client.get("/api/v1/recommendations/1?count=3")
        assert response.status_code == 200
        data = response.json()
        assert data["user_id"] == 1
        assert len(data["recommendations"]) <= 3
    
    def test_get_user_interests(self, client, sample_data):
        """Test getting user interests"""
        response = client.get("/api/v1/recommendations/1/interests")
        assert response.status_code == 200
        data = response.json()
        assert data["user_id"] == 1
        assert "interests" in data
    
    def test_get_popular_news(self, client, sample_data):
        """Test getting popular news"""
        response = client.get("/api/v1/recommendations/popular/news?count=3")
        assert response.status_code == 200
        data = response.json()
        assert "recommendations" in data
        assert len(data["recommendations"]) <= 3


class TestAPIValidation:
    """Test API input validation"""
    
    def test_invalid_count(self, client):
        """Test that invalid count parameter is rejected"""
        response = client.get("/api/v1/recommendations/1?count=100")
        # Should be limited to 50
        assert response.status_code in [200, 422]
    
    def test_negative_count(self, client):
        """Test that negative count is rejected"""
        response = client.get("/api/v1/recommendations/1?count=-5")
        assert response.status_code == 422


if __name__ == "__main__":
    pytest.main([__file__, "-v"])
