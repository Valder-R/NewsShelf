# NewsShelf Testing Guide

## 🚀 Services Status

All services are running and connected:

| Service            | Port       | Status     | Notes                                   |
| ------------------ | ---------- | ---------- | --------------------------------------- |
| **Frontend**       | 3000       | ✅ Running | React + Vite, Nginx reverse proxy       |
| **User Service**   | 5001       | ✅ Running | ASP.NET Core, JWT Auth, SQLite          |
| **Search Service** | 5002       | ✅ Running | ASP.NET Core, News CRUD, PostgreSQL     |
| **Rec Service**    | 8001       | ✅ Running | FastAPI, ML recommendations, PostgreSQL |
| **PostgreSQL**     | 5432       | ✅ Healthy | Databases: newsshelf, recommendations   |
| **RabbitMQ**       | 5672/15672 | ✅ Healthy | Message broker, Management UI on 15672  |

---

## 🌐 Application URLs

### User Interfaces

- **Frontend**: http://localhost:3000
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)

### API Documentation (Swagger)

- **User Service**: http://localhost:5001/swagger
- **Search Service**: http://localhost:5002/swagger
- **Rec Service**: http://localhost:8001/api/v1/docs

---

## 📋 Test Scenarios

### 1. **User Registration & Login**

#### Register New User

1. Go to http://localhost:3000/register
2. Fill in:
   - Email: `test@example.com`
   - Display Name: `Test User`
   - Password: `Test123!`
   - Select favorite topics (optional)
3. Click "Register"
4. Should redirect to home page and see authenticated menu

#### Login

1. Go to http://localhost:3000/login
2. Enter credentials:
   - Email: `test@example.com`
   - Password: `Test123!`
3. Click "Sign In"
4. Should redirect to home page with authenticated menu

### 2. **News Search**

#### Search News Articles

1. Must be logged in
2. Click "Search News" in navigation
3. Enter search query (e.g., "technology", "politics", "sports")
4. Results should display with:
   - News title
   - Content preview
   - Category
   - Author
   - Publication date
   - Image (if available)

### 3. **View Recommendations**

#### Get AI Recommendations

1. Must be logged in
2. Click "Recommendations" in navigation
3. System should show personalized news based on:
   - User's reading history
   - Selected favorite topics
   - ML model predictions
4. Each recommendation shows relevance score

### 4. **User Profile**

#### View/Edit Profile

1. Must be logged in
2. Click "Profile" in navigation
3. View:
   - Email (read-only)
   - Display Name (editable)
   - Bio (editable)
4. Edit fields and click "Save" to update

---

## 🔍 API Testing with cURL

### User Service (Port 5001)

#### Register

```bash
curl -X POST http://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!",
    "displayName": "Test User"
  }'
```

#### Login

```bash
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!"
  }'
```

#### Get Profile (requires token)

```bash
curl -X GET http://localhost:5001/api/profile/me \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Search Service (Port 5002)

#### Search News

```bash
curl -X GET "http://localhost:5002/api/news/search/by-text?query=technology"
```

#### Get News by Category

```bash
curl -X GET "http://localhost:5002/api/news/category/TECH"
```

#### Get All News

```bash
curl -X GET "http://localhost:5002/api/news"
```

### Rec Service (Port 8001)

#### Get Recommendations

```bash
curl -X GET "http://localhost:8001/api/v1/recommendations?user_id=123"
```

#### Health Check

```bash
curl -X GET "http://localhost:8001/health"
```

---

## 📊 Expected Data Flow

```
Frontend (Port 3000)
    ↓
Nginx Reverse Proxy
    ├→ /api/auth/* → User Service (5001)
    ├→ /api/profile/* → User Service (5001)
    ├→ /api/activities/* → User Service (5001)
    ├→ /api/news/* → Search Service (5002)
    └→ /api/recommendations/* → Rec Service (8001)

User Service (5001)
    ├→ Publishes: user.registered, news.read events → RabbitMQ
    └→ Database: SQLite (embedded)

Search Service (5002)
    ├→ Subscribes to: user.registered, news.read events
    ├→ Database: PostgreSQL (newsshelf)
    └→ Provides: News CRUD, Search API

Rec Service (8001)
    ├→ Subscribes to: user.registered, news.read events
    ├→ Database: PostgreSQL (recommendations)
    ├→ ML Model: Sentence Transformers
    └→ Provides: Personalized recommendations

RabbitMQ (5672)
    └→ Message exchange between all services
```

---

## ✅ Verification Checklist

- [ ] Frontend loads at http://localhost:3000
- [ ] Can register new user successfully
- [ ] Can login with registered credentials
- [ ] Authentication token stored in localStorage
- [ ] Authenticated pages (Search, Recommendations, Profile) are protected
- [ ] Can search for news articles
- [ ] Search results display correctly
- [ ] Can view personalized recommendations
- [ ] Can view and edit profile
- [ ] Logout clears token and redirects to home
- [ ] RabbitMQ Management UI is accessible
- [ ] All services show healthy status
- [ ] API endpoints respond with correct data

---

## 🐛 Troubleshooting

### Frontend shows connection refused error

- **Issue**: POST http://localhost:5000/api/auth/register → ERR_CONNECTION_REFUSED
- **Solution**: Verify User Service is running on port 5001 (not 5000)
- **Fix Applied**: Updated api.ts to use `http://localhost:5001/api`

### Registration fails with validation error

- **Check**: Email format is valid
- **Check**: Password length >= 6 characters
- **Check**: User Service logs: `docker logs newsshelf_user_service`

### Search returns no results

- **Possible Cause**: News data not imported into database
- **Solution**: Load sample data using import script
- **Command**: Check RecService/scripts/ for data import

### RabbitMQ shows no connections

- **Check**: Services may not have started correctly
- **Solution**: Restart services: `docker-compose restart`
- **Verify**: Check service logs for connection errors

---

## 📝 Notes

- Frontend API URL is configured in `Frontend/src/services/api.ts` (default: http://localhost:5001/api)
- Authentication uses JWT tokens stored in localStorage
- RabbitMQ enables async event processing between services
- PostgreSQL stores search and recommendation data
- User Service uses embedded SQLite for user authentication

---

**Last Updated**: December 28, 2025
