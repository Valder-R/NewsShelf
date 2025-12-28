#!/usr/bin/env python3
"""Test script to verify recommendations service"""

import requests
import json

print("\n" + "="*60)
print("🧪 ТЕСТУВАННЯ СЕРВІСУ РЕКОМЕНДАЦІЙ")
print("="*60)

# Get recommendations for different users
for user_id in [1, 2, 3, 4, 5]:
    try:
        resp = requests.get(f'http://localhost:8001/api/v1/recommendations/{user_id}')
        data = resp.json()
        
        print(f"\n{'='*60}")
        print(f"👤 КОРИСТУВАЧ {user_id}")
        print(f"{'='*60}")
        print(f"Всього рекомендацій: {data.get('total_count', 0)}")
        
        if data.get('recommendations'):
            print("\n📰 Топ-5 персоналізованих рекомендацій:")
            for i, rec in enumerate(data['recommendations'][:5], 1):
                print(f"\n  {i}. НОВИНА #{rec['news_id']}")
                print(f"     Заголовок: {rec['title'][:60]}...")
                print(f"     Категорія: {rec['category']}")
                print(f"     Оцінка подібності: {rec['similarity_score']:.3f}")
        else:
            print("\n⚠️  Немає персоналізованих рекомендацій (користувач новий)")
            
        # Get user interests
        resp = requests.get(f'http://localhost:8001/api/v1/recommendations/{user_id}/interests')
        interests = resp.json()
        if interests.get('interests'):
            print(f"\n📊 Інтереси користувача:")
            for category, weight in list(interests['interests'].items())[:5]:
                print(f"   - {category}: {weight:.2%}")
        
    except Exception as e:
        print(f'❌ Помилка для користувача {user_id}: {e}')

# Get popular news
print(f"\n{'='*60}")
print("🌟 ГЛОБАЛЬНІ ПОПУЛЯРНІ НОВИНИ (FALLBACK)")
print(f"{'='*60}")
try:
    resp = requests.get('http://localhost:8001/api/v1/recommendations/popular/news?count=5')
    data = resp.json()
    
    print(f"Всього популярних новин: {data.get('total_count', 0)}")
    
    if data.get('recommendations'):
        print("\n📰 Топ-5 популярних новин:")
        for i, rec in enumerate(data['recommendations'][:5], 1):
            print(f"\n  {i}. НОВИНА #{rec['news_id']}")
            print(f"     Заголовок: {rec['title'][:60]}...")
            print(f"     Категорія: {rec['category']}")
except Exception as e:
    print(f'❌ Помилка при отриманні популярних новин: {e}')

print(f"\n{'='*60}")
print("✅ ТЕСТУВАННЯ ЗАВЕРШЕНО!")
print(f"{'='*60}\n")
