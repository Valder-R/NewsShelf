"""
Examples of how to use the Recommendation Service API
"""

import requests
import json
from typing import List, Dict

BASE_URL = "http://localhost:8001"
API_BASE = f"{BASE_URL}/api/v1"


class RecommendationClient:
    """Client for Recommendation Service API"""
    
    def __init__(self, base_url: str = API_BASE):
        self.base_url = base_url
    
    def get_recommendations(
        self, 
        user_id: int, 
        count: int = 10,
        threshold: float = 0.3
    ) -> Dict:
        """Get personalized recommendations for a user
        
        Args:
            user_id: User ID
            count: Number of recommendations (1-50)
            threshold: Similarity threshold (0.0-1.0)
            
        Returns:
            Dictionary with recommendations
        """
        url = f"{self.base_url}/recommendations/{user_id}"
        params = {
            "count": count,
            "threshold": threshold
        }
        
        response = requests.get(url, params=params)
        response.raise_for_status()
        return response.json()
    
    def get_user_interests(self, user_id: int) -> Dict:
        """Get user's category interests
        
        Args:
            user_id: User ID
            
        Returns:
            Dictionary with user interests
        """
        url = f"{self.base_url}/recommendations/{user_id}/interests"
        response = requests.get(url)
        response.raise_for_status()
        return response.json()
    
    def get_popular_news(self, count: int = 10) -> Dict:
        """Get popular news
        
        Args:
            count: Number of news items (1-50)
            
        Returns:
            Dictionary with popular news
        """
        url = f"{self.base_url}/recommendations/popular/news"
        params = {"count": count}
        
        response = requests.get(url, params=params)
        response.raise_for_status()
        return response.json()
    
    def health_check(self) -> Dict:
        """Check service health
        
        Returns:
            Health status
        """
        url = f"{BASE_URL}/health"
        response = requests.get(url)
        response.raise_for_status()
        return response.json()


# ============================================================================
# USAGE EXAMPLES
# ============================================================================

def example_1_basic_recommendations():
    """Example 1: Get basic recommendations"""
    print("=" * 60)
    print("Example 1: Getting recommendations for a user")
    print("=" * 60)
    
    client = RecommendationClient()
    
    try:
        recommendations = client.get_recommendations(user_id=1, count=5)
        
        print(f"User ID: {recommendations['user_id']}")
        print(f"Total recommendations: {recommendations['total_count']}")
        print("\nRecommendations:")
        
        for i, rec in enumerate(recommendations['recommendations'], 1):
            print(f"\n{i}. {rec['title']}")
            print(f"   Category: {rec['category']}")
            print(f"   Similarity: {rec['similarity_score']:.2%}")
            print(f"   Authors: {rec['authors']}")
            
    except Exception as e:
        print(f"Error: {e}")


def example_2_user_interests():
    """Example 2: Get user interests"""
    print("\n" + "=" * 60)
    print("Example 2: Getting user interests")
    print("=" * 60)
    
    client = RecommendationClient()
    
    try:
        interests = client.get_user_interests(user_id=1)
        
        print(f"User ID: {interests['user_id']}")
        print(f"Total activities: {interests['total_activities']}")
        print("\nInterests:")
        
        for category, weight in interests['interests'].items():
            print(f"  {category}: {weight:.1%}")
            
    except Exception as e:
        print(f"Error: {e}")


def example_3_popular_news():
    """Example 3: Get popular news"""
    print("\n" + "=" * 60)
    print("Example 3: Getting popular news")
    print("=" * 60)
    
    client = RecommendationClient()
    
    try:
        popular = client.get_popular_news(count=5)
        
        print(f"Total popular news: {popular['total_count']}")
        print("\nPopular news:")
        
        for i, news in enumerate(popular['recommendations'], 1):
            print(f"\n{i}. {news['title']}")
            print(f"   Category: {news['category']}")
            print(f"   Popularity score: {news['similarity_score']:.2%}")
            
    except Exception as e:
        print(f"Error: {e}")


def example_4_health_check():
    """Example 4: Health check"""
    print("\n" + "=" * 60)
    print("Example 4: Health check")
    print("=" * 60)
    
    client = RecommendationClient()
    
    try:
        health = client.health_check()
        
        print(f"Status: {health['status']}")
        print(f"Database: {health['database']}")
        print(f"Message: {health['message']}")
        
    except Exception as e:
        print(f"Error: {e}")


def example_5_different_users():
    """Example 5: Compare recommendations for different users"""
    print("\n" + "=" * 60)
    print("Example 5: Comparing recommendations for multiple users")
    print("=" * 60)
    
    client = RecommendationClient()
    users = [1, 2, 3]
    
    for user_id in users:
        try:
            recs = client.get_recommendations(user_id=user_id, count=3)
            print(f"\nUser {user_id} - {recs['total_count']} recommendations:")
            
            for rec in recs['recommendations']:
                print(f"  • {rec['title'][:50]}... ({rec['category']})")
                
        except Exception as e:
            print(f"Error for user {user_id}: {e}")


def example_6_raw_curl_commands():
    """Example 6: Using curl commands"""
    print("\n" + "=" * 60)
    print("Example 6: CURL commands")
    print("=" * 60)
    
    commands = [
        "# Health check",
        "curl http://localhost:8001/health",
        "",
        "# Get recommendations",
        "curl 'http://localhost:8001/api/v1/recommendations/1?count=5'",
        "",
        "# Get user interests",
        "curl 'http://localhost:8001/api/v1/recommendations/1/interests'",
        "",
        "# Get popular news",
        "curl 'http://localhost:8001/api/v1/recommendations/popular/news?count=10'",
        "",
        "# API documentation",
        "# Open: http://localhost:8001/api/v1/docs"
    ]
    
    for cmd in commands:
        print(cmd)


def example_7_advanced_filtering():
    """Example 7: Advanced filtering with thresholds"""
    print("\n" + "=" * 60)
    print("Example 7: Advanced filtering")
    print("=" * 60)
    
    client = RecommendationClient()
    
    thresholds = [0.3, 0.5, 0.7, 0.9]
    
    print("Comparing recommendations with different similarity thresholds:\n")
    
    for threshold in thresholds:
        try:
            recs = client.get_recommendations(
                user_id=1,
                count=10,
                threshold=threshold
            )
            print(f"Threshold {threshold}: {recs['total_count']} recommendations")
            
        except Exception as e:
            print(f"Error with threshold {threshold}: {e}")


# ============================================================================
# MAIN
# ============================================================================

if __name__ == "__main__":
    print("\n" + "=" * 60)
    print("RECOMMENDATION SERVICE API - USAGE EXAMPLES")
    print("=" * 60)
    
    try:
        # Run examples
        example_4_health_check()
        example_1_basic_recommendations()
        example_2_user_interests()
        example_3_popular_news()
        example_5_different_users()
        example_7_advanced_filtering()
        example_6_raw_curl_commands()
        
    except KeyboardInterrupt:
        print("\n\nExamples interrupted by user")
    except Exception as e:
        print(f"\nUnexpected error: {e}")
    
    print("\n" + "=" * 60)
    print("Examples completed!")
    print("=" * 60)
