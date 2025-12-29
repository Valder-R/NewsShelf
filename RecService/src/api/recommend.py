"""
Recommendation API endpoints
"""

from fastapi import APIRouter, HTTPException, Query, Depends
from typing import List

from src.services.recommendation_service import (
    get_recommendations, 
    get_popular_news,
    get_user_interests,
    get_user_activity
)
from src.schemas import RecommendationsListResponse, RecommendationResponse
from src.logger import logger
from src.db import SessionLocal
from src.config import get_settings


router = APIRouter(prefix="/api/v1/recommendations", tags=["Recommendations"])
settings = get_settings()


@router.get(
    "/{user_id}",
    response_model=RecommendationsListResponse,
    summary="Get recommendations for a user",
    description="Generate personalized news recommendations for a specific user"
)
def get_user_recommendations(
    user_id: int,
    count: int = Query(default=10, ge=1, le=50, description="Number of recommendations"),
    threshold: float = Query(default=0.3, ge=0.0, le=1.0, description="Similarity threshold")
):
    """Get personalized recommendations for a user"""
    try:
        logger.info(f"📌 Fetching recommendations for user {user_id}")
        
        db = SessionLocal()
        
        # Get recommendations
        recommendations = get_recommendations(user_id, count=count, db=db)
        
        if not recommendations:
            logger.warning(f"No recommendations found for user {user_id}")
            return RecommendationsListResponse(
                user_id=user_id,
                recommendations=[],
                total_count=0
            )
        
        # Convert to response model
        rec_responses = [
            RecommendationResponse(
                news_id=news.id,
                title=news.title,
                category=news.category,
                short_description=news.short_description,
                authors=news.authors,
                similarity_score=score,
                link=news.link
            )
            for news, score in recommendations
        ]
        
        logger.info(f"✅ Returned {len(rec_responses)} recommendations for user {user_id}")
        
        return RecommendationsListResponse(
            user_id=user_id,
            recommendations=rec_responses,
            total_count=len(rec_responses)
        )
        
    except Exception as e:
        logger.error(f"❌ Error fetching recommendations: {e}")
        raise HTTPException(status_code=500, detail=str(e))


@router.get(
    "/{user_id}/interests",
    summary="Get user interests",
    description="Get the user's category interests based on their activity"
)
def get_user_category_interests(user_id: int):
    """Get user's category interests"""
    try:
        logger.info(f"📌 Fetching interests for user {user_id}")
        
        interests = get_user_interests(user_id)
        
        return {
            "user_id": user_id,
            "interests": interests,
            "total_activities": sum(
                len(get_user_activity(user_id)) if get_user_activity(user_id) else 0
                for _ in [None]
            )
        }
    except Exception as e:
        logger.error(f"❌ Error fetching user interests: {e}")
        raise HTTPException(status_code=500, detail=str(e))


@router.get(
    "/popular/news",
    summary="Get popular news",
    description="Get most popular news by view count"
)
def get_popular_recommendations(
    count: int = Query(default=10, ge=1, le=50)
):
    """Get popular news"""
    try:
        logger.info(f"📌 Fetching popular news (count={count})")
        
        db = SessionLocal()
        popular = get_popular_news(count, db=db)
        
        rec_responses = [
            RecommendationResponse(
                news_id=news.id,
                title=news.title,
                category=news.category,
                short_description=news.short_description,
                authors=news.authors,
                similarity_score=score,
                link=news.link
            )
            for news, score in popular
        ]
        
        return {
            "recommendations": rec_responses,
            "total_count": len(rec_responses)
        }
    except Exception as e:
        logger.error(f"❌ Error fetching popular news: {e}")
        raise HTTPException(status_code=500, detail=str(e))
