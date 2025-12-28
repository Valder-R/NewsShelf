# Development Setup Guide

## Prerequisites Installation

### .NET Core Services Setup

#### 1. Install Required NuGet Packages

For both **UserService** and **SearchService**, add the following NuGet package:

```bash
cd UserService.Api
dotnet add package RabbitMQ.Client

cd ../SearchService.Api  # if exists
dotnet add package RabbitMQ.Client
```

Or manually edit the `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="RabbitMQ.Client" Version="6.7.0" />
</ItemGroup>
```

#### 2. Restore Dependencies

```bash
dotnet restore
```

#### 3. Build Services

```bash
dotnet build
```

#### 4. Run Services Locally

```bash
# UserService
cd UserService.Api
dotnet run

# SearchService (in another terminal)
cd SearchService.Api
dotnet run
```

### Python RecService Setup

#### 1. Create Virtual Environment

```bash
cd RecService
python -m venv venv

# Activate
# On Windows:
venv\Scripts\activate
# On Linux/Mac:
source venv/bin/activate
```

#### 2. Install Dependencies

```bash
pip install -r requirements.txt
```

#### 3. Initialize Database

```bash
python src/scripts/init_db.py
python src/scripts/import_news.py
python src/scripts/generate_embeddings.py
```

#### 4. Run Service

```bash
cd src
python main.py
```

### Frontend Setup

#### 1. Install Node Packages

```bash
cd Frontend
npm install
```

#### 2. Development Server

```bash
npm run dev
# Opens on http://localhost:5173
```

#### 3. Build for Production

```bash
npm run build
# Output in dist/
```

---

## Database Setup

### PostgreSQL Local Installation

#### Windows

```powershell
# Using Chocolatey
choco install postgresql

# Or download from https://www.postgresql.org/download/windows/
```

#### Linux

```bash
sudo apt-get install postgresql postgresql-contrib
sudo systemctl start postgresql
sudo systemctl enable postgresql
```

#### macOS

```bash
brew install postgresql
brew services start postgresql
```

### Initialize Database

```bash
# Connect to PostgreSQL
psql -U postgres

# Create database
CREATE DATABASE newsshelf;

# Create user (optional)
CREATE USER newsshelf_user WITH PASSWORD 'your_password';
ALTER ROLE newsshelf_user CREATEDB;

# Exit
\q
```

### Import News Data

```bash
cd RecService
python src/scripts/import_news.py
```

---

## RabbitMQ Setup

### Local Installation

#### Windows

1. Download from https://www.rabbitmq.com/download.html
2. Install using installer
3. Enable management plugin:
   ```
   rabbitmq-plugins enable rabbitmq_management
   ```
4. Access management UI: http://localhost:15672 (guest/guest)

#### Linux

```bash
sudo apt-get install rabbitmq-server
sudo systemctl start rabbitmq-server
sudo rabbitmq-plugins enable rabbitmq_management
```

#### macOS

```bash
brew install rabbitmq
brew services start rabbitmq
rabbitmq-plugins enable rabbitmq_management
```

---

## Environment Variables

### UserService (.env or appsettings.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "SigningKey": "your-256-bit-secret-key-here-minimum-32-chars",
    "Issuer": "newsshelf-api",
    "Audience": "newsshelf-app",
    "ExpirationMinutes": 60
  },
  "RabbitMq": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  }
}
```

### SearchService (.env or appsettings.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*",
  "RabbitMq": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=newsshelf;User Id=postgres;Password=postgres;"
  }
}
```

### RecService (.env)

```
DATABASE_URL=postgresql://postgres:postgres@localhost:5432/newsshelf
RABBITMQ_HOST=localhost
RABBITMQ_PORT=5672
RABBITMQ_USER=guest
RABBITMQ_PASSWORD=guest
PYTHONUNBUFFERED=1
LOG_LEVEL=INFO
```

### Frontend (.env.local)

```
VITE_API_URL=http://localhost:5001
VITE_SEARCH_API_URL=http://localhost:5002
VITE_REC_API_URL=http://localhost:8001
```

---

## Local Testing Workflow

### 1. Start Services

Terminal 1: PostgreSQL

```bash
# Ensure PostgreSQL is running
psql -U postgres -d newsshelf -c "SELECT 1;"
```

Terminal 2: RabbitMQ

```bash
# Ensure RabbitMQ is running
rabbitmqctl status
```

Terminal 3: UserService

```bash
cd UserService.Api
dotnet run
# Runs on http://localhost:5001
```

Terminal 4: SearchService

```bash
cd SearchService.Api
dotnet run
# Runs on http://localhost:5002
```

Terminal 5: RecService

```bash
cd RecService
source venv/bin/activate  # or venv\Scripts\activate on Windows
python src/main.py
# Runs on http://localhost:8001
```

Terminal 6: Frontend

```bash
cd Frontend
npm run dev
# Runs on http://localhost:5173
```

### 2. Test Registration

```bash
curl -X POST http://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!@#",
    "displayName": "Test User"
  }'
```

### 3. Test Login

```bash
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!@#"
  }'
```

### 4. Test Search

```bash
curl http://localhost:5002/api/news/search?query=technology
```

### 5. Test Recommendations

```bash
curl http://localhost:8001/api/v1/recommendations?userId=1
```

---

## IDE Configuration

### Visual Studio 2022

1. File > Open > Folder (select NewsShelf)
2. Install extensions:
   - C# Dev Kit
   - .NET Runtime Manager
3. Open Solutions:
   - `UserService.UserService.Api.csproj`
   - Project structure auto-loads

### Visual Studio Code

1. Install extensions:

   - C# Dev Kit
   - Python
   - REST Client
   - Thunder Client

2. Launch configurations in `.vscode/launch.json`:

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "UserService",
      "type": "coreclr",
      "request": "launch",
      "program": "${workspaceFolder}/UserService/bin/Debug/net8.0/NewsShelf.UserService.Api.dll",
      "args": [],
      "cwd": "${workspaceFolder}/UserService",
      "stopAtEntry": false,
      "serverReadyAction": {
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)",
        "uriFormat": "{0}/swagger",
        "action": "openExternally"
      }
    },
    {
      "name": "RecService",
      "type": "python",
      "request": "launch",
      "module": "uvicorn",
      "args": ["src.main:app", "--reload"],
      "jinja": true,
      "cwd": "${workspaceFolder}/RecService"
    }
  ]
}
```

### PyCharm

1. Open project
2. Configure Python interpreter: `RecService/venv/bin/python`
3. Mark folders as sources:
   - RecService/src

---

## Common Issues & Solutions

### Issue: RabbitMQ Connection Refused

**Solution:**

```bash
# Check if RabbitMQ is running
rabbitmqctl status

# Start RabbitMQ
rabbitmq-server

# Or via Docker
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.12-management
```

### Issue: Database Connection Error

**Solution:**

```bash
# Check PostgreSQL
psql -U postgres -d newsshelf

# Verify connection string
# Windows: (local)\newsshelf
# Linux: localhost:5432
# Set correct password in appsettings.json
```

### Issue: Module Not Found (Python)

**Solution:**

```bash
# Activate virtual environment
source venv/bin/activate

# Reinstall requirements
pip install -r requirements.txt

# Verify installation
pip list
```

### Issue: Port Already in Use

**Solution:**

```bash
# Windows - Find and kill process
netstat -ano | findstr :5001
taskkill /PID <PID> /F

# Linux/Mac
lsof -i :5001
kill -9 <PID>
```

### Issue: CORS Errors

**Solution:** Update `Program.cs`:

```csharp
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

app.UseCors("AllowAll");
```

---

## Performance Optimization

### Database

- Create indexes on frequently searched columns
- Use connection pooling
- Monitor slow queries with `pg_stat_statements`

### API Services

- Enable response caching
- Use paging for large result sets
- Implement rate limiting
- Use async/await for I/O operations

### Message Queue

- Batch messages when possible
- Use prefetch limits
- Monitor queue depth
- Clean up dead-letter queues

### Frontend

- Use production build: `npm run build`
- Enable gzip compression
- Implement lazy loading
- Optimize images

---

**Last Updated**: December 2024
**Version**: 1.0.0
