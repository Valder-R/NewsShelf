# 🎬 Getting Started Guide - NewsShelf

Welcome to NewsShelf! This guide will help you get up and running in minutes.

---

## ⚡ 60-Second Quick Start

### On Windows (PowerShell)

```powershell
# Navigate to project directory
cd "c:\Users\forti\Repos\NewsShelf"

# Run the quick start script
.\quickstart.bat
```

### On Linux/Mac (Terminal)

```bash
# Navigate to project directory
cd /path/to/NewsShelf

# Make script executable
chmod +x quickstart.sh

# Run the quick start script
./quickstart.sh
```

**That's it!** The script will:

- ✅ Start PostgreSQL
- ✅ Start RabbitMQ
- ✅ Start User Service
- ✅ Start Search Service
- ✅ Start Recommendation Service
- ✅ Start Frontend
- ✅ Wait for all services to be healthy
- ✅ Display access points

---

## 🌐 Access Your Services

After running the quick start script, open these URLs:

### 👥 Frontend (Main Application)

```
http://localhost:3000
```

This is your user-facing application where you can:

- Register a new account
- Login
- Search for news articles
- View personalized recommendations
- Manage your profile

### 🔌 APIs

**User Service** (Authentication & Profile)

```
http://localhost:5001/swagger
```

**Search Service** (News Search)

```
http://localhost:5002/swagger
```

**Recommendation Service** (Personalized Recommendations)

```
http://localhost:8001/api/v1/docs
```

### 📨 Message Queue

**RabbitMQ Management Console**

```
http://localhost:15672
Username: guest
Password: guest
```

View messages flowing between services in real-time!

### 💾 Database

**PostgreSQL** (if you want to connect)

```
Host: localhost
Port: 5432
Database: newsshelf
Username: postgres
Password: postgres
```

---

## 🧪 Test the Full Flow

### Step 1: Register a New User

Go to http://localhost:3000 and click "Register"

Or via API:

```bash
curl -X POST http://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "yourname@example.com",
    "password": "SecurePassword123!@#",
    "displayName": "Your Name"
  }'
```

### Step 2: Login

Click "Login" on the frontend or:

```bash
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "yourname@example.com",
    "password": "SecurePassword123123!@#"
  }'
```

Copy the `token` from the response - you'll need it for other requests.

### Step 3: Search for News

Try searching for "technology" or "artificial intelligence":

```bash
curl "http://localhost:5002/api/news/search?query=technology"
```

Or on the frontend, go to Search page.

### Step 4: View Recommendations

Get personalized recommendations:

```bash
curl "http://localhost:8001/api/v1/recommendations?userId=<YOUR_USER_ID>"
```

Or on the frontend, go to Recommendations page.

---

## 🐛 Troubleshooting

### Services Won't Start?

**Check if Docker is running:**

```bash
docker ps
```

**View service logs:**

```bash
docker-compose logs -f user-service
docker-compose logs -f search-service
docker-compose logs -f rec-service
```

**Restart a specific service:**

```bash
docker-compose restart user-service
```

### Port Already in Use?

If you get "Address already in use" error:

**On Windows (PowerShell):**

```powershell
# Find process using port
netstat -ano | findstr :5001

# Kill the process (replace PID with actual number)
taskkill /PID <PID> /F
```

**On Linux/Mac:**

```bash
# Find process using port
lsof -i :5001

# Kill the process
kill -9 <PID>
```

### Database Connection Issues?

Check if PostgreSQL is running:

```bash
docker exec newsshelf_postgres pg_isready -U postgres
```

### RabbitMQ Management Console Not Loading?

Check if RabbitMQ is running:

```bash
docker exec newsshelf_rabbitmq rabbitmq-diagnostics ping
```

---

## 📚 Next: Learn the Architecture

Read these documents (in order):

1. **[PROJECT_SUMMARY.md](PROJECT_SUMMARY.md)** - Overview of everything created
2. **[ARCHITECTURE.md](ARCHITECTURE.md)** - System design and data flow
3. **[API_DOCUMENTATION.md](API_DOCUMENTATION.md)** - Complete API reference
4. **[DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)** - Deploy to Kubernetes
5. **[DEVELOPMENT_SETUP.md](DEVELOPMENT_SETUP.md)** - Local development guide

---

## 🔍 Verify Everything Works

### 1. Check All Services Are Running

```bash
docker-compose ps
```

You should see:

```
NAME                       STATUS
newsshelf_postgres         Up (healthy)
newsshelf_rabbitmq         Up (healthy)
newsshelf_user_service     Up (healthy)
newsshelf_search_service   Up (healthy)
newsshelf_rec_service      Up (healthy)
newsshelf_frontend         Up (healthy)
```

### 2. Check RabbitMQ Queue

Open http://localhost:15672 and login with `guest/guest`

- Go to **Queues** tab
- You should see queues for each service
- When you perform actions, messages should appear

### 3. Test Each Service

**User Service:**

```bash
curl http://localhost:5001/health
```

Expected response: `{"status":"healthy"}`

**Search Service:**

```bash
curl http://localhost:5002/health
```

Expected response: `{"status":"healthy"}`

**Rec Service:**

```bash
curl http://localhost:8001/health
```

Expected response: `{"status":"healthy"}`

### 4. Frontend Test

Open http://localhost:3000

- Register a test account
- Login
- Search for "news"
- Go to Recommendations to see AI-powered suggestions

---

## 🛑 Stop Services

When you're done:

```bash
# Stop all services (keep data)
docker-compose down

# Stop and remove all data
docker-compose down -v

# Stop a specific service
docker-compose stop user-service
```

---

## 📊 View Service Status

### Docker Compose Status

```bash
docker-compose ps
```

### View All Containers

```bash
docker ps -a
```

### View Container Logs (Last 50 lines)

```bash
docker logs --tail 50 newsshelf_user_service
```

### View Real-Time Logs

```bash
docker-compose logs -f
```

### Check Container Resource Usage

```bash
docker stats
```

---

## 🔧 Common Commands

| Task            | Command                                                        |
| --------------- | -------------------------------------------------------------- |
| Start services  | `docker-compose up -d`                                         |
| Stop services   | `docker-compose down`                                          |
| View logs       | `docker-compose logs -f [service]`                             |
| Restart service | `docker-compose restart [service]`                             |
| Rebuild image   | `docker-compose up -d --build [service]`                       |
| Remove volumes  | `docker-compose down -v`                                       |
| View network    | `docker network inspect newsshelf_network`                     |
| Run SQL query   | `docker exec newsshelf_postgres psql -U postgres -d newsshelf` |

---

## 📝 Example Workflow

### 1. Start System (Terminal 1)

```bash
./quickstart.sh    # or quickstart.bat on Windows
```

### 2. Open Frontend (Browser)

```
http://localhost:3000
```

### 3. Register Account

- Click "Register"
- Enter email, password, name
- Select favorite news categories
- Click "Register"

### 4. Login

- Click "Login"
- Enter email and password
- Click "Login"

### 5. Search News

- Click "Search" in navigation
- Search for "technology" or any topic
- Click on an article
- This records your interest!

### 6. View Recommendations

- Click "Recommendations" in navigation
- See AI-powered suggestions based on:
  - Articles you read
  - Your favorite categories
  - Semantic similarity to articles you liked

### 7. Check Message Flow

- Open http://localhost:15672
- Login with guest/guest
- Go to "Queues" tab
- Watch messages being published and consumed

---

## 🎓 Learning Path

### For Beginners

1. Run the application
2. Test all features in the frontend
3. Read PROJECT_SUMMARY.md
4. Explore API endpoints using Swagger UI

### For Developers

1. Review ARCHITECTURE.md
2. Study the code:
   - Frontend: `Frontend/src/` (React/TypeScript)
   - User Service: `UserService/` (.NET)
   - Search Service: `SearchService/` (.NET)
   - Rec Service: `RecService/src/` (Python)
3. Review API documentation
4. Try making API calls with curl/Postman

### For DevOps Engineers

1. Review DEPLOYMENT_GUIDE.md
2. Study Kubernetes manifests in `k8s/`
3. Review docker-compose.yml
4. Plan scale-up strategy

---

## 🌟 Key Features to Try

### 1. Personalized Recommendations

```
The system learns from your reading history and
suggests articles you'll enjoy!
```

### 2. Full-Text Search

```
Search 63,500+ news articles across 8 categories
```

### 3. Event-Driven Architecture

```
Services communicate via RabbitMQ message bus
Watch events in real-time at http://localhost:15672
```

### 4. Semantic Similarity

```
Recommendations based on AI embeddings,
not just keyword matching
```

### 5. Scalable Design

```
Services are horizontally scalable
Deploy to Kubernetes for production
```

---

## 💡 Tips & Tricks

### Tail Logs in Real-Time

```bash
docker-compose logs -f user-service
```

### See Message Flow

Watch the RabbitMQ queue: http://localhost:15672 → Queues

### Test Recommendations Immediately

After registering and reading an article, go to Recommendations page

### Check Database

```bash
docker exec newsshelf_postgres psql -U postgres -d newsshelf -c "SELECT COUNT(*) FROM news;"
```

### Monitor Resource Usage

```bash
docker stats
```

---

## 📞 Need Help?

### Check Documentation

- PROJECT_SUMMARY.md
- ARCHITECTURE.md
- API_DOCUMENTATION.md
- DEPLOYMENT_GUIDE.md

### View Logs

```bash
docker-compose logs -f [service_name]
```

### Check RabbitMQ

http://localhost:15672 (guest/guest)

### Check Swagger

- User Service: http://localhost:5001/swagger
- Search Service: http://localhost:5002/swagger

---

## 🚀 Ready for Next Steps?

Once you're comfortable with the system:

1. **Deploy to Kubernetes**: See DEPLOYMENT_GUIDE.md
2. **Add AdminService**: Follow the same pattern as other services
3. **Add Caching**: Implement Redis layer
4. **Set up Monitoring**: Add Prometheus + Grafana
5. **Production Hardening**: SSL, rate limiting, auth0, etc.

---

## ✅ Checklist

- [ ] Docker is installed and running
- [ ] Project is cloned/extracted
- [ ] Run quickstart.sh or quickstart.bat
- [ ] Frontend loads at http://localhost:3000
- [ ] Can register and login
- [ ] Can search news
- [ ] Can see recommendations
- [ ] RabbitMQ shows messages
- [ ] All health checks pass

---

**You're all set! 🎉 Enjoy NewsShelf!**

For detailed information, see the documentation files in the project root.
