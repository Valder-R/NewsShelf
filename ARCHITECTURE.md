# NewsShelf Architecture Guide

## System Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                        Frontend (React)                          │
│                  TypeScript + Tailwind CSS + Vite                │
│              (Authentication, Search, Recommendations)           │
└──────────────────────────┬──────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────────┐
│                    API Gateway (Nginx)                           │
│          Route /api/* to respective microservices              │
│         SSL/TLS termination, Rate limiting, Caching             │
└──────────────────────────┬──────────────────────────────────────┘
         ┌──────────────────┼──────────────────┐
         ▼                  ▼                  ▼
    ┌─────────┐        ┌─────────┐        ┌─────────┐
    │  User   │        │ Search  │        │   Rec   │
    │Service  │        │Service  │        │Service  │
    │(ASP.NET)│        │(ASP.NET)│        │(Python) │
    └────┬────┘        └────┬────┘        └────┬────┘
         │                  │                  │
         └──────────────────┼──────────────────┘
                            │
                    ┌───────▼────────┐
                    │   RabbitMQ     │
                    │   Message Bus  │
                    └────────────────┘
                            │
           ┌────────────────┼────────────────┐
           ▼                ▼                ▼
      ┌─────────┐      ┌─────────┐      ┌──────────┐
      │PostgreSQL│      │  SQLite │     │ Redis    │
      │(Primary) │      │(UserSvc)│     │(Optional)│
      └──────────┘      └─────────┘     └──────────┘
```

---

## Microservices Architecture

### 1. User Service (C# .NET 8)

**Purpose**: Authentication, user profile management, activity tracking

**Technology Stack**:

- Framework: ASP.NET Core 8.0
- Database: SQLite (embedded)
- ORM: Entity Framework Core
- Authentication: JWT tokens
- Messaging: RabbitMQ (publisher)

**Key Responsibilities**:

- User registration and authentication
- JWT token generation and validation
- User profile management
- Activity tracking (read history, favorites)
- Publishing user events to message bus

**API Endpoints**:

```
POST   /api/auth/register          - Register new user
POST   /api/auth/login             - Login and get JWT
POST   /api/profile                - Update profile
GET    /api/profile                - Get user profile
POST   /api/activities             - Record user activity
GET    /api/activities             - Get activity history
POST   /api/activities/favorites   - Add favorite topic
```

**Event Publishing**:

- `user_registered`: When new user registers
- `news_read`: When user reads an article
- `favorite_topics_added`: When user marks topic as favorite
- `user_profile_updated`: When user updates profile

**Technology Choices Rationale**:

- SQLite: Lightweight, good for auth storage, single-user reads
- JWT: Stateless, scalable authentication
- RabbitMQ: Asynchronous event publishing

---

### 2. Search Service (C# .NET 8)

**Purpose**: News search, filtering, and CRUD operations

**Technology Stack**:

- Framework: ASP.NET Core 8.0
- Database: PostgreSQL (can use SQL Server)
- ORM: Entity Framework Core
- Search: Full-text search via PostgreSQL
- Messaging: RabbitMQ (publisher)

**Key Responsibilities**:

- Index and store news articles
- Full-text search functionality
- Category and date filtering
- CRUD operations for news
- Publishing search events to message bus

**API Endpoints**:

```
GET    /api/news/search            - Search news by query
GET    /api/news/category/:cat     - Get news by category
GET    /api/news/:id               - Get specific article
POST   /api/news                   - Create news (admin)
PUT    /api/news/:id               - Update news (admin)
DELETE /api/news/:id               - Delete news (admin)
GET    /api/categories             - Get all categories
```

**Event Publishing**:

- `news_searched`: When user performs search
- `news_viewed`: When user views an article

**Technology Choices Rationale**:

- PostgreSQL: Robust full-text search, good for analytics
- Full-text Search: Native capability in PostgreSQL
- RabbitMQ: Asynchronous event logging

---

### 3. Recommendation Service (Python FastAPI)

**Purpose**: Semantic similarity recommendations, personalization

**Technology Stack**:

- Framework: FastAPI
- AI/ML: sentence-transformers (all-MiniLM-L6-v2)
- Database: PostgreSQL
- Message Queue: RabbitMQ (consumer)
- Computation: NumPy, SciPy

**Key Responsibilities**:

- Generate semantic embeddings for news articles
- Calculate similarity scores between articles
- Recommend articles based on user preferences
- Process user events from RabbitMQ
- Fallback to popular news for cold-start problem

**API Endpoints**:

```
GET    /api/v1/recommendations     - Get personalized recommendations
GET    /api/v1/recommendations/popular - Get popular articles
GET    /health                      - Health check endpoint
```

**Event Consumption**:

- `user_registered`: Initialize user preferences
- `news_read`: Update user interest profile
- `favorite_topics_added`: Boost certain categories

**Technology Choices Rationale**:

- FastAPI: Async, fast, easy ML integration
- sentence-transformers: Lightweight embeddings, accurate similarity
- PostgreSQL: Persistent storage of embeddings and user preferences

---

## Data Flow Diagrams

### User Registration & Initialization

```
User → Frontend → UserService → RabbitMQ → RecService
                                  │
                                  ▼
                              PostgreSQL
                           (Store embeddings)
```

### Article Search & Personalization

```
User → Frontend
       │
       ▼
  API Gateway
       │
       ├─────────────────────────┐
       ▼                         ▼
  SearchService           UserService
       │                         │
       ├─────────────┬───────────┘
       │             │
       ▼             ▼
  PostgreSQL   SQLite (get user)
                     │
                     ▼
                  RabbitMQ
                     │
                     ▼
                 RecService
                     │
                     ▼
             Calculate Similarity
```

### Recommendation Generation

```
RecService
    │
    ├─ Get user interests from PostgreSQL
    ├─ Get article embeddings from PostgreSQL
    ├─ Calculate cosine similarity
    ├─ Apply personalization weights
    ├─ Fallback to popular articles
    │
    ▼
Return Top-K Recommendations
```

---

## Database Schema

### PostgreSQL (Primary Database)

**Tables**:

```sql
-- News articles
Table: news
  - id (PK)
  - title (TEXT)
  - content (TEXT)
  - category (VARCHAR)
  - author (VARCHAR)
  - published_date (TIMESTAMP)
  - image_url (VARCHAR)
  - source (VARCHAR)
  - created_at (TIMESTAMP)

-- User interests and preferences
Table: user_interests
  - id (PK)
  - user_id (FK to user_service)
  - category (VARCHAR)
  - score (FLOAT)
  - updated_at (TIMESTAMP)

-- Article embeddings
Table: embeddings
  - id (PK)
  - article_id (FK to news)
  - vector (VECTOR)  -- pgvector extension
  - created_at (TIMESTAMP)

-- View tracking
Table: article_views
  - id (PK)
  - user_id (FK)
  - article_id (FK)
  - view_date (TIMESTAMP)
```

### SQLite (UserService)

**Tables**:

```sql
-- Users
Table: AspNetUsers
  - Id (PK)
  - Email (VARCHAR)
  - PasswordHash (VARCHAR)
  - DisplayName (VARCHAR)
  - CreatedAt (TIMESTAMP)

-- Activities
Table: UserActivities
  - Id (PK)
  - UserId (FK)
  - ActivityType (VARCHAR)
  - NewsCategory (VARCHAR)
  - Timestamp (TIMESTAMP)

-- Favorite Topics
Table: UserFavoriteTopics
  - Id (PK)
  - UserId (FK)
  - Topic (VARCHAR)
  - AddedAt (TIMESTAMP)
```

---

## Message Queue Architecture

### RabbitMQ Setup

**Exchanges**:

```
newsshelf.events  (type: topic, durable: true)
  ├── user.* routing key
  ├── news.* routing key
  └── rec.* routing key
```

**Queues**:

```
user_service_queue
  - Binding: user.*
  - Consumer: UserService

search_service_queue
  - Binding: news.*
  - Consumer: SearchService

rec_service_queue
  - Binding: user.*, news.*
  - Consumer: RecService
```

**Event Flow**:

1. **User Registration**

   ```
   UserService → "user.registered" → RecService
     {
       userId: 123,
       email: "user@example.com",
       timestamp: 2024-01-01T10:00:00Z
     }
   ```

2. **News Read**

   ```
   UserService → "news.read" → RecService
     {
       userId: 123,
       articleId: 456,
       category: "TECHNOLOGY",
       timestamp: 2024-01-01T10:00:00Z
     }
   ```

3. **Favorite Topics Added**

   ```
   UserService → "user.favorite_topics_added" → RecService
     {
       userId: 123,
       topics: ["POLITICS", "BUSINESS"],
       timestamp: 2024-01-01T10:00:00Z
     }
   ```

4. **Article Searched**
   ```
   SearchService → "news.searched" → (logged)
     {
       userId: 123,
       query: "artificial intelligence",
       resultCount: 45,
       timestamp: 2024-01-01T10:00:00Z
     }
   ```

---

## API Gateway Architecture

### Nginx Configuration

**Routing Rules**:

```nginx
# Upstream definitions
upstream user_service {
  server user-service:5001;
}

upstream search_service {
  server search-service:5002;
}

upstream rec_service {
  server rec-service:8001;
}

upstream frontend {
  server frontend:3000;
}

# Route /api/auth/* to UserService
location /api/auth/ {
  proxy_pass http://user_service;
}

# Route /api/profile/* to UserService
location /api/profile/ {
  proxy_pass http://user_service;
}

# Route /api/news/* to SearchService
location /api/news/ {
  proxy_pass http://search_service;
}

# Route /api/recommendations/* to RecService
location /api/recommendations/ {
  proxy_pass http://rec_service;
}

# Route / to Frontend
location / {
  proxy_pass http://frontend;
}
```

**Features**:

- Load balancing across service replicas
- Connection pooling
- Request/response compression
- Caching for static assets
- CORS handling

---

## Deployment Architecture

### Docker Compose (Development)

```yaml
Services:
├── postgres (5432)
├── rabbitmq (5672/15672)
├── user-service (5001)
├── search-service (5002)
├── rec-service (8001)
└── frontend (3000)

Network: newsshelf_network
Volumes:
├── postgres_data
├── rabbitmq_data
└── rec_logs
```

### Kubernetes (Production)

**Namespace**: `newsshelf`

**Deployments**:

```
├── postgres (1 replica, stateful)
├── rabbitmq (1 replica, stateful)
├── user-service (2+ replicas, stateless)
├── search-service (2+ replicas, stateless)
├── rec-service (1 replica, stateful)
└── frontend (2+ replicas, stateless)

ConfigMaps:
├── postgres-config
├── rabbitmq-config
├── user-service-config
├── search-service-config
├── rec-service-config
└── frontend-config

Secrets:
├── db-credentials
└── jwt-secret

Services:
├── postgres (ClusterIP:5432)
├── rabbitmq (ClusterIP:5672)
├── user-service (ClusterIP:5001)
├── search-service (ClusterIP:5002)
├── rec-service (ClusterIP:8001)
├── frontend (ClusterIP:3000)
└── api-gateway (LoadBalancer:80)

Ingress:
└── newsshelf.example.com → api-gateway

HPA (Horizontal Pod Autoscaler):
├── user-service (2-5 replicas)
├── search-service (2-5 replicas)
└── frontend (2-5 replicas)
```

---

## Scalability Considerations

### Horizontal Scaling

**Stateless Services** (can scale easily):

- User Service: 2-10 replicas
- Search Service: 2-10 replicas
- Frontend: 2-5 replicas

**Stateful Services** (harder to scale):

- PostgreSQL: Use read replicas, master-slave
- RabbitMQ: Use cluster mode
- RecService: Scale compute, not state

### Vertical Scaling

**Resource Allocation**:

```
User Service:      256Mi memory, 100m CPU
Search Service:    256Mi memory, 100m CPU
Rec Service:       512Mi memory, 200m CPU
Frontend:          128Mi memory, 50m CPU
PostgreSQL:        512Mi memory, 200m CPU
RabbitMQ:          256Mi memory, 100m CPU
```

### Caching Strategy

```
Level 1: HTTP Cache (Nginx)
  ├── Static assets (1 week)
  ├── API responses (5 minutes)
  └── Search results (1 minute)

Level 2: Application Cache (Redis)
  ├── User profiles (1 hour)
  ├── Recommendations (30 minutes)
  └── Search metadata (1 hour)

Level 3: Database Cache
  ├── Connection pooling
  ├── Query result caching
  └── Full-text search indexes
```

---

## Security Architecture

### Authentication & Authorization

```
Frontend → JWT Token → API Gateway
           │
           ├─ Validate signature
           ├─ Check expiration
           ├─ Extract claims
           │
           ▼
      Route to Service
           │
           ├─ Verify user identity
           ├─ Check permissions
           │
           ▼
      Return Response
```

### Data Protection

**In Transit**:

- HTTPS/TLS for all external connections
- Internal services use HTTP (assume trusted network)

**At Rest**:

- Passwords: bcrypt with salt
- Sensitive data: encrypted in database
- Backups: encrypted storage

**Access Control**:

- Database credentials: Kubernetes secrets
- API keys: Environment variables
- RabbitMQ credentials: Centralized configuration

---

## Monitoring & Observability

### Metrics Collection

**Services**:

- Prometheus scrapes `/metrics` endpoints
- Collects: requests/sec, latency, errors, resource usage

**Database**:

- PostgreSQL pg_stat_statements
- Query performance analysis
- Connection pool monitoring

**Message Queue**:

- RabbitMQ management API
- Queue depth, consumer lag
- Message throughput

### Logging Strategy

```
Service Logs → Syslog/Stdout
              │
              ▼
        Log Collector (Fluentd/Logstash)
              │
              ▼
        ELK Stack (Elasticsearch, Logstash, Kibana)
              │
              ├─ Indexing
              ├─ Storage
              └─ Visualization
```

### Health Checks

**Kubernetes Liveness**:

```
GET /health → 200 OK
Every 10 seconds
```

**Readiness**:

```
GET /ready → 200 OK
Database connected
Message queue connected
```

---

## Performance Optimization

### Database Optimization

- Indexes on search columns
- Connection pooling (10-20 connections)
- Query optimization via EXPLAIN ANALYZE
- Pagination for large result sets

### API Optimization

- Response compression (gzip)
- Caching headers
- Async/await for I/O
- Connection reuse

### Frontend Optimization

- Code splitting
- Lazy loading routes
- Image optimization (WebP)
- Service worker caching

### Message Queue Optimization

- Batch processing
- Message prefetch limits
- Dead-letter queues
- Consumer group scaling

---

## Disaster Recovery

### Backup Strategy

**Database**:

- Daily PostgreSQL dumps
- 7-day retention
- Off-site storage
- Test restore monthly

**Configuration**:

- Version control for all configs
- ConfigMaps in Kubernetes
- Encrypted credential backup

**Messages**:

- RabbitMQ persistence enabled
- Acknowledgment-based delivery
- Dead-letter queues for failed messages

### Recovery Procedures

**Database Recovery**:

1. Identify backup to restore
2. Create new database
3. Restore from backup
4. Verify data integrity
5. Update connection strings

**Service Recovery**:

1. Delete failed deployment
2. Re-apply Kubernetes manifest
3. Monitor pod startup
4. Verify health checks pass

---

**Last Updated**: December 2024
**Version**: 1.0.0
