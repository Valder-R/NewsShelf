"""
Pydantic schemas for API requests and responses
"""

from pydantic import BaseModel, Field
from datetime import datetime
from typing import List, Optional


class NewsBase(BaseModel):
    """Base news schema"""
    category: str
    title: str
    short_description: Optional[str] = None
    authors: Optional[str] = None
    link: str
    date: str


class NewsResponse(NewsBase):
    """News response schema"""
    id: int
    view_count: int = 0
    
    class Config:
        from_attributes = True


class RecommendationResponse(BaseModel):
    """Single recommendation"""
    news_id: int
    title: str
    category: str
    short_description: Optional[str] = None
    authors: Optional[str] = None
    similarity_score: float = Field(..., ge=0.0, le=1.0)
    link: str
    
    class Config:
        from_attributes = True


class RecommendationsListResponse(BaseModel):
    """List of recommendations"""
    user_id: int
    recommendations: List[RecommendationResponse]
    total_count: int
    
    class Config:
        from_attributes = True


class HealthCheckResponse(BaseModel):
    """Health check response"""
    status: str
    database: str
    message: str
