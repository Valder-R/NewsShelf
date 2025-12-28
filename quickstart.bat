@echo off
REM NewsShelf Quick Start Script for Windows
REM This script sets up and runs all services locally

setlocal enabledelayedexpansion

echo.
echo ================================
echo NewsShelf Quick Start
echo ================================
echo.

REM Check prerequisites
echo Checking prerequisites...

where docker >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Docker is required but not installed.
    exit /b 1
)
echo [OK] Docker installed

where docker-compose >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Docker Compose is required but not installed.
    exit /b 1
)
echo [OK] Docker Compose installed

where git >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Git is required but not installed.
    exit /b 1
)
echo [OK] Git installed

REM Get script directory
set SCRIPT_DIR=%~dp0

REM Change to project directory
cd /d "%SCRIPT_DIR%"

REM Stop existing containers
echo.
echo Stopping any existing containers...
docker-compose down 2>nul || true

REM Start services
echo.
echo Starting services...
docker-compose up -d

REM Wait for services to be healthy
echo.
echo Waiting for services to be healthy...

REM Wait for PostgreSQL
echo   Waiting for PostgreSQL...
set /a count=0
:wait_postgres
set /a count+=1
if %count% gtr 30 (
    echo   ERROR: PostgreSQL did not become ready
    goto :error
)
docker exec newsshelf_postgres pg_isready -U postgres >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo     Attempt %count%/30...
    timeout /t 2 /nobreak >nul
    goto :wait_postgres
)
echo   [OK] PostgreSQL is ready

REM Wait for RabbitMQ
echo   Waiting for RabbitMQ...
set /a count=0
:wait_rabbitmq
set /a count+=1
if %count% gtr 30 (
    echo   ERROR: RabbitMQ did not become ready
    goto :error
)
docker exec newsshelf_rabbitmq rabbitmq-diagnostics ping >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo     Attempt %count%/30...
    timeout /t 2 /nobreak >nul
    goto :wait_rabbitmq
)
echo   [OK] RabbitMQ is ready

REM Wait for User Service
echo   Waiting for User Service...
set /a count=0
:wait_user_service
set /a count+=1
if %count% gtr 30 (
    echo   ERROR: User Service did not become ready
    goto :error
)
curl -s http://localhost:5001/health >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo     Attempt %count%/30...
    timeout /t 2 /nobreak >nul
    goto :wait_user_service
)
echo   [OK] User Service is ready

REM Wait for Search Service
echo   Waiting for Search Service...
set /a count=0
:wait_search_service
set /a count+=1
if %count% gtr 30 (
    echo   ERROR: Search Service did not become ready
    goto :error
)
curl -s http://localhost:5002/health >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo     Attempt %count%/30...
    timeout /t 2 /nobreak >nul
    goto :wait_search_service
)
echo   [OK] Search Service is ready

REM Wait for Rec Service
echo   Waiting for Rec Service...
set /a count=0
:wait_rec_service
set /a count+=1
if %count% gtr 30 (
    echo   ERROR: Rec Service did not become ready
    goto :error
)
curl -s http://localhost:8001/health >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo     Attempt %count%/30...
    timeout /t 2 /nobreak >nul
    goto :wait_rec_service
)
echo   [OK] Rec Service is ready

REM Wait for Frontend
echo   Waiting for Frontend...
set /a count=0
:wait_frontend
set /a count+=1
if %count% gtr 30 (
    echo   ERROR: Frontend did not become ready
    goto :error
)
curl -s http://localhost:3000 >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo     Attempt %count%/30...
    timeout /t 2 /nobreak >nul
    goto :wait_frontend
)
echo   [OK] Frontend is ready

REM Print summary
echo.
echo ================================
echo All services are running!
echo ================================
echo.

echo Access Points:
echo.
echo Frontend:
echo   http://localhost:3000
echo.
echo APIs:
echo   User Service: http://localhost:5001/swagger
echo   Search Service: http://localhost:5002/swagger
echo   Rec Service: http://localhost:8001/api/v1/docs
echo.
echo Message Queue:
echo   RabbitMQ Management: http://localhost:15672 ^(guest/guest^)
echo.
echo Database:
echo   PostgreSQL: localhost:5432 ^(postgres/postgres^)
echo.

echo Useful Commands:
echo.
echo   View logs:
echo     docker-compose logs -f [service]
echo.
echo   Stop services:
echo     docker-compose down
echo.
echo   Stop and remove data:
echo     docker-compose down -v
echo.
echo   Restart a service:
echo     docker-compose restart [service]
echo.
echo   View all containers:
echo     docker-compose ps
echo.

echo Test API Endpoints:
echo.
echo   Register:
echo     curl -X POST http://localhost:5001/api/auth/register ^
echo       -H "Content-Type: application/json" ^
echo       -d "{\"email\":\"test@example.com\",\"password\":\"Test123!@#\",\"displayName\":\"Test\"}"
echo.
echo   Login:
echo     curl -X POST http://localhost:5001/api/auth/login ^
echo       -H "Content-Type: application/json" ^
echo       -d "{\"email\":\"test@example.com\",\"password\":\"Test123!@#\"}"
echo.
echo   Search News:
echo     curl "http://localhost:5002/api/news/search?query=technology"
echo.
echo   Get Recommendations:
echo     curl "http://localhost:8001/api/v1/recommendations?userId=1"
echo.

echo Ready to go! [Success]
echo.
pause
goto :end

:error
echo.
echo An error occurred during startup.
echo Please check the logs and try again.
echo.
pause
exit /b 1

:end
