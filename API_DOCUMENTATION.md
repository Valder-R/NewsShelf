# NewsShelf API Documentation

## Base URLs

| Environment | URL                                                                                       |
| ----------- | ----------------------------------------------------------------------------------------- |
| Development | http://localhost:5001 (User), http://localhost:5002 (Search), http://localhost:8001 (Rec) |
| Docker      | http://api-gateway:80                                                                     |
| Production  | https://api.newsshelf.example.com                                                         |

---

## Authentication

### JWT Tokens

All authenticated endpoints require a Bearer token in the `Authorization` header:

```
Authorization: Bearer <JWT_TOKEN>
```

### Token Structure

```json
{
  "sub": "user_id",
  "email": "user@example.com",
  "name": "Display Name",
  "iat": 1704067200,
  "exp": 1704153600,
  "iss": "newsshelf-api"
}
```

### Token Expiration

- **Issued**: Upon successful login or registration
- **Duration**: 60 minutes (configurable)
- **Refresh**: Login again to get new token
- **On Expiration**: API returns `401 Unauthorized`

---

## User Service API

### Base URL

- Development: `http://localhost:5001`
- Production: `https://api.newsshelf.example.com/api`

### Endpoints

---

#### POST `/api/auth/register`

Register a new user account.

**Request**:

```json
{
  "email": "user@example.com",
  "password": "SecurePassword123!@#",
  "displayName": "John Doe"
}
```

**Validation**:

- Email: Valid format, unique in system
- Password: Minimum 8 characters, uppercase, lowercase, number, special char
- Display Name: 3-100 characters

**Response** (201 Created):

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "email": "user@example.com",
  "displayName": "John Doe",
  "createdAt": "2024-01-01T10:00:00Z"
}
```

**Error Responses**:

- `400 Bad Request`: Invalid data format
- `409 Conflict`: Email already registered
- `422 Unprocessable Entity`: Password doesn't meet requirements

**Events Triggered**:

- `user.registered` → RabbitMQ

**Example**:

```bash
curl -X POST http://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "SecurePass123!@#",
    "displayName": "John Doe"
  }'
```

---

#### POST `/api/auth/login`

Authenticate user and receive JWT token.

**Request**:

```json
{
  "email": "user@example.com",
  "password": "SecurePassword123!@#"
}
```

**Response** (200 OK):

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600,
  "user": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "user@example.com",
    "displayName": "John Doe"
  }
}
```

**Error Responses**:

- `401 Unauthorized`: Invalid credentials
- `404 Not Found`: User doesn't exist

**Example**:

```bash
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "SecurePass123!@#"
  }'
```

---

#### GET `/api/profile`

Get current user's profile information.

**Headers**:

```
Authorization: Bearer <TOKEN>
```

**Response** (200 OK):

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "email": "user@example.com",
  "displayName": "John Doe",
  "bio": "Tech enthusiast and news junkie",
  "createdAt": "2024-01-01T10:00:00Z",
  "favoriteTopics": ["TECHNOLOGY", "BUSINESS", "SCIENCE"]
}
```

**Error Responses**:

- `401 Unauthorized`: Invalid or missing token
- `404 Not Found`: User not found

**Example**:

```bash
curl -X GET http://localhost:5001/api/profile \
  -H "Authorization: Bearer <TOKEN>"
```

---

#### PUT `/api/profile`

Update user profile information.

**Headers**:

```
Authorization: Bearer <TOKEN>
Content-Type: application/json
```

**Request**:

```json
{
  "displayName": "John Smith",
  "bio": "Updated bio text"
}
```

**Response** (200 OK):

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "email": "user@example.com",
  "displayName": "John Smith",
  "bio": "Updated bio text",
  "updatedAt": "2024-01-02T10:00:00Z"
}
```

**Error Responses**:

- `400 Bad Request`: Invalid data
- `401 Unauthorized`: Invalid token
- `422 Unprocessable Entity`: Display name too long

**Events Triggered**:

- `user.profile_updated` → RabbitMQ

**Example**:

```bash
curl -X PUT http://localhost:5001/api/profile \
  -H "Authorization: Bearer <TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{
    "displayName": "John Smith",
    "bio": "I love technology and science"
  }'
```

---

#### POST `/api/activities`

Record a user activity (article read, topic favorited, etc.)

**Headers**:

```
Authorization: Bearer <TOKEN>
Content-Type: application/json
```

**Request**:

```json
{
  "activityType": "READ",
  "newsId": "123456",
  "category": "TECHNOLOGY"
}
```

**Activity Types**:

- `READ`: User read an article
- `FAVORITE`: User marked topic as favorite
- `SEARCH`: User performed search
- `BOOKMARK`: User bookmarked article

**Response** (201 Created):

```json
{
  "id": "activity-123",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "activityType": "READ",
  "newsId": "123456",
  "category": "TECHNOLOGY",
  "timestamp": "2024-01-02T10:05:00Z"
}
```

**Error Responses**:

- `400 Bad Request`: Invalid activity type
- `401 Unauthorized`: Invalid token

**Events Triggered**:

- `news.read` → RabbitMQ (if activityType is READ)

**Example**:

```bash
curl -X POST http://localhost:5001/api/activities \
  -H "Authorization: Bearer <TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{
    "activityType": "READ",
    "newsId": "123456",
    "category": "TECHNOLOGY"
  }'
```

---

#### GET `/api/activities`

Retrieve user's activity history.

**Headers**:

```
Authorization: Bearer <TOKEN>
```

**Query Parameters**:

- `skip`: Number of results to skip (default: 0)
- `take`: Number of results to return (default: 20, max: 100)
- `activityType`: Filter by activity type (optional)

**Response** (200 OK):

```json
{
  "total": 150,
  "skip": 0,
  "take": 20,
  "activities": [
    {
      "id": "activity-123",
      "userId": "550e8400-e29b-41d4-a716-446655440000",
      "activityType": "READ",
      "newsId": "123456",
      "category": "TECHNOLOGY",
      "timestamp": "2024-01-02T10:05:00Z"
    }
  ]
}
```

**Error Responses**:

- `401 Unauthorized`: Invalid token

**Example**:

```bash
curl -X GET "http://localhost:5001/api/activities?skip=0&take=20" \
  -H "Authorization: Bearer <TOKEN>"
```

---

#### POST `/api/activities/favorites`

Add topic to user's favorite topics.

**Headers**:

```
Authorization: Bearer <TOKEN>
Content-Type: application/json
```

**Request**:

```json
{
  "topics": ["TECHNOLOGY", "BUSINESS", "SCIENCE"]
}
```

**Response** (201 Created):

```json
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "favoriteTopics": ["TECHNOLOGY", "BUSINESS", "SCIENCE"],
  "updatedAt": "2024-01-02T10:10:00Z"
}
```

**Error Responses**:

- `400 Bad Request`: Invalid topics
- `401 Unauthorized`: Invalid token

**Events Triggered**:

- `user.favorite_topics_added` → RabbitMQ

**Example**:

```bash
curl -X POST http://localhost:5001/api/activities/favorites \
  -H "Authorization: Bearer <TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{
    "topics": ["TECHNOLOGY", "BUSINESS"]
  }'
```

---

#### GET `/health`

Health check endpoint for monitoring.

**Response** (200 OK):

```json
{
  "status": "healthy",
  "timestamp": "2024-01-02T10:00:00Z",
  "dependencies": {
    "database": "connected",
    "rabbitmq": "connected"
  }
}
```

**Example**:

```bash
curl http://localhost:5001/health
```

---

## Search Service API

### Base URL

- Development: `http://localhost:5002`
- Production: `https://api.newsshelf.example.com/api`

### Endpoints

---

#### GET `/api/news/search`

Search news articles by query.

**Query Parameters**:

- `query` (required): Search term (min 2 characters)
- `skip`: Number of results to skip (default: 0)
- `take`: Number of results (default: 20, max: 100)
- `category`: Filter by category (optional)
- `sortBy`: Sort field: `relevance`, `date`, `popularity` (default: relevance)
- `sortOrder`: `asc`, `desc` (default: desc)

**Response** (200 OK):

```json
{
  "query": "artificial intelligence",
  "total": 245,
  "skip": 0,
  "take": 20,
  "results": [
    {
      "id": "123456",
      "title": "Latest AI Breakthroughs in 2024",
      "content": "A comprehensive overview of recent AI developments...",
      "category": "TECHNOLOGY",
      "author": "John Smith",
      "publishedDate": "2024-01-01T10:00:00Z",
      "imageUrl": "https://example.com/image.jpg",
      "source": "TechNews",
      "relevanceScore": 0.95
    }
  ]
}
```

**Error Responses**:

- `400 Bad Request`: Query too short
- `422 Unprocessable Entity`: Invalid parameters

**Example**:

```bash
curl "http://localhost:5002/api/news/search?query=technology&skip=0&take=20&category=TECHNOLOGY"
```

---

#### GET `/api/news/:id`

Get a specific news article.

**Path Parameters**:

- `id` (required): Article ID

**Headers**:

```
Authorization: Bearer <TOKEN> (optional)
```

**Response** (200 OK):

```json
{
  "id": "123456",
  "title": "Latest AI Breakthroughs in 2024",
  "content": "A comprehensive overview of recent AI developments...",
  "category": "TECHNOLOGY",
  "author": "John Smith",
  "publishedDate": "2024-01-01T10:00:00Z",
  "imageUrl": "https://example.com/image.jpg",
  "source": "TechNews",
  "viewCount": 15234,
  "keywords": ["AI", "machine learning", "neural networks"]
}
```

**Error Responses**:

- `404 Not Found`: Article doesn't exist

**Events Triggered** (if authenticated):

- `news.viewed` → RabbitMQ

**Example**:

```bash
curl "http://localhost:5002/api/news/123456" \
  -H "Authorization: Bearer <TOKEN>"
```

---

#### GET `/api/news/category/:category`

Get news articles by category.

**Path Parameters**:

- `category` (required): Category name (e.g., TECHNOLOGY, BUSINESS)

**Query Parameters**:

- `skip`: Number of results to skip (default: 0)
- `take`: Number of results (default: 20, max: 100)
- `sortBy`: `date`, `popularity` (default: date)

**Response** (200 OK):

```json
{
  "category": "TECHNOLOGY",
  "total": 5234,
  "skip": 0,
  "take": 20,
  "articles": [
    {
      "id": "123456",
      "title": "Latest AI Breakthroughs in 2024",
      "category": "TECHNOLOGY",
      "publishedDate": "2024-01-01T10:00:00Z",
      "imageUrl": "https://example.com/image.jpg"
    }
  ]
}
```

**Example**:

```bash
curl "http://localhost:5002/api/news/category/TECHNOLOGY?skip=0&take=20"
```

---

#### GET `/api/categories`

Get all available news categories.

**Response** (200 OK):

```json
{
  "categories": [
    {
      "name": "TECHNOLOGY",
      "count": 5234,
      "description": "Technology and innovation news"
    },
    {
      "name": "BUSINESS",
      "count": 3421,
      "description": "Business and finance news"
    },
    {
      "name": "ENTERTAINMENT",
      "count": 4521,
      "description": "Entertainment and celebrity news"
    },
    {
      "name": "POLITICS",
      "count": 2341,
      "description": "Politics and government news"
    },
    {
      "name": "SPORTS",
      "count": 3456,
      "description": "Sports and athletics news"
    },
    {
      "name": "HEALTH",
      "count": 2100,
      "description": "Health and wellness news"
    },
    {
      "name": "SCIENCE",
      "count": 1890,
      "description": "Science and research news"
    },
    {
      "name": "WORLD NEWS",
      "count": 4567,
      "description": "International and world news"
    }
  ],
  "total": 50
}
```

**Example**:

```bash
curl http://localhost:5002/api/categories
```

---

#### POST `/api/news` (Admin)

Create a new news article.

**Headers**:

```
Authorization: Bearer <ADMIN_TOKEN>
Content-Type: application/json
```

**Request**:

```json
{
  "title": "Breaking News: New Discovery",
  "content": "Full article content here...",
  "category": "SCIENCE",
  "author": "Jane Doe",
  "imageUrl": "https://example.com/image.jpg",
  "source": "ScienceDaily"
}
```

**Response** (201 Created):

```json
{
  "id": "123457",
  "title": "Breaking News: New Discovery",
  "category": "SCIENCE",
  "publishedDate": "2024-01-02T10:00:00Z",
  "createdAt": "2024-01-02T10:00:00Z"
}
```

**Error Responses**:

- `401 Unauthorized`: Invalid or missing token
- `403 Forbidden`: User is not admin
- `400 Bad Request`: Invalid data

---

#### PUT `/api/news/:id` (Admin)

Update a news article.

**Headers**:

```
Authorization: Bearer <ADMIN_TOKEN>
Content-Type: application/json
```

**Request**:

```json
{
  "title": "Updated Title",
  "content": "Updated content...",
  "category": "SCIENCE"
}
```

**Response** (200 OK):

```json
{
  "id": "123457",
  "title": "Updated Title",
  "updatedAt": "2024-01-02T11:00:00Z"
}
```

---

#### DELETE `/api/news/:id` (Admin)

Delete a news article.

**Headers**:

```
Authorization: Bearer <ADMIN_TOKEN>
```

**Response** (204 No Content)

---

#### GET `/health`

Health check endpoint.

**Response** (200 OK):

```json
{
  "status": "healthy",
  "timestamp": "2024-01-02T10:00:00Z",
  "dependencies": {
    "database": "connected",
    "rabbitmq": "connected"
  }
}
```

---

## Recommendation Service API

### Base URL

- Development: `http://localhost:8001`
- Production: `https://api.newsshelf.example.com/api/recommendations`

### Endpoints

---

#### GET `/api/v1/recommendations`

Get personalized news recommendations for a user.

**Query Parameters**:

- `userId` (required): User ID
- `take`: Number of recommendations (default: 10, max: 50)
- `minSimilarity`: Minimum similarity score 0-1 (default: 0.3)

**Response** (200 OK):

```json
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "recommendations": [
    {
      "articleId": "123456",
      "title": "Latest AI Breakthroughs in 2024",
      "category": "TECHNOLOGY",
      "author": "John Smith",
      "publishedDate": "2024-01-01T10:00:00Z",
      "imageUrl": "https://example.com/image.jpg",
      "similarityScore": 0.92,
      "reason": "Based on your interest in technology"
    }
  ],
  "userInterests": {
    "TECHNOLOGY": 0.95,
    "SCIENCE": 0.87,
    "BUSINESS": 0.65
  },
  "recommendationMethod": "semantic_similarity"
}
```

**Error Responses**:

- `404 Not Found`: User not found
- `400 Bad Request`: Invalid parameters

**Example**:

```bash
curl "http://localhost:8001/api/v1/recommendations?userId=550e8400-e29b-41d4-a716-446655440000&take=10"
```

---

#### GET `/api/v1/recommendations/popular`

Get popular news articles (no personalization).

**Query Parameters**:

- `take`: Number of articles (default: 10, max: 50)
- `category`: Filter by category (optional)
- `days`: Last N days (default: 7)

**Response** (200 OK):

```json
{
  "articles": [
    {
      "articleId": "123456",
      "title": "Most Viewed Article",
      "category": "TECHNOLOGY",
      "publishedDate": "2024-01-01T10:00:00Z",
      "viewCount": 15234,
      "trending": true
    }
  ],
  "generatedAt": "2024-01-02T10:00:00Z"
}
```

**Example**:

```bash
curl "http://localhost:8001/api/v1/recommendations/popular?take=10&category=TECHNOLOGY"
```

---

#### GET `/health`

Health check endpoint.

**Response** (200 OK):

```json
{
  "status": "healthy",
  "timestamp": "2024-01-02T10:00:00Z",
  "dependencies": {
    "database": "connected",
    "rabbitmq": "connected",
    "embeddings": "ready"
  }
}
```

---

## Error Response Format

All errors follow this format:

```json
{
  "error": {
    "code": "ERROR_CODE",
    "message": "Human readable error message",
    "details": {
      "field": "value"
    },
    "timestamp": "2024-01-02T10:00:00Z"
  }
}
```

### Common Error Codes

| Code                   | Status | Description                       |
| ---------------------- | ------ | --------------------------------- |
| `INVALID_REQUEST`      | 400    | Request validation failed         |
| `UNAUTHORIZED`         | 401    | Missing or invalid authentication |
| `FORBIDDEN`            | 403    | Insufficient permissions          |
| `NOT_FOUND`            | 404    | Resource not found                |
| `CONFLICT`             | 409    | Resource already exists           |
| `UNPROCESSABLE_ENTITY` | 422    | Data validation failed            |
| `INTERNAL_ERROR`       | 500    | Server error                      |
| `SERVICE_UNAVAILABLE`  | 503    | Service temporarily down          |

---

## Rate Limiting

Rate limits are applied per user:

- **Authentication endpoints**: 5 requests per minute per IP
- **Search endpoints**: 100 requests per minute per user
- **Recommendation endpoints**: 50 requests per minute per user
- **Activity endpoints**: 200 requests per minute per user

Rate limit headers:

```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 45
X-RateLimit-Reset: 1704067200
```

On rate limit exceeded:

```
HTTP/1.1 429 Too Many Requests
Retry-After: 45
```

---

## Pagination

List endpoints support cursor-based pagination:

**Query Parameters**:

- `skip`: Number of items to skip (default: 0)
- `take`: Number of items to return (default: 20, max: 100)

**Response Format**:

```json
{
  "total": 1000,
  "skip": 0,
  "take": 20,
  "items": []
}
```

---

## Timestamps

All timestamps are in ISO 8601 format with UTC timezone:

```
2024-01-02T10:00:00Z
```

---

**Last Updated**: December 2024
**API Version**: 1.0.0
