"""
User activity model for tracking interactions with news
"""

from sqlalchemy import Column, Integer, DateTime, ForeignKey, String
from src.db import Base
from datetime import datetime


class UserActivity(Base):
    """Track user interactions with news articles"""
    
    __tablename__ = "user_activity"

    id = Column(Integer, primary_key=True)
    user_id = Column(Integer, index=True, nullable=False)
    news_id = Column(Integer, ForeignKey("news.id"), nullable=False)
    timestamp = Column(DateTime, default=datetime.utcnow, index=True)
    activity_type = Column(String(50), default="view")  # view, like, share, etc.
    
    def __repr__(self):
        return f"<UserActivity(user_id={self.user_id}, news_id={self.news_id}, type='{self.activity_type}')>"
