#!/bin/bash

# NewsShelf Quick Start Script
# This script sets up and runs all services locally

set -e

echo "📦 NewsShelf Quick Start"
echo "========================"

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Check prerequisites
echo -e "${BLUE}Checking prerequisites...${NC}"

command -v docker &> /dev/null || { echo -e "${RED}Docker is required but not installed.${NC}"; exit 1; }
command -v docker-compose &> /dev/null || { echo -e "${RED}Docker Compose is required but not installed.${NC}"; exit 1; }
command -v git &> /dev/null || { echo -e "${RED}Git is required but not installed.${NC}"; exit 1; }

echo -e "${GREEN}✓ Docker installed${NC}"
echo -e "${GREEN}✓ Docker Compose installed${NC}"
echo -e "${GREEN}✓ Git installed${NC}"

# Get script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# Change to project directory
cd "$SCRIPT_DIR"

# Stop existing containers
echo -e "${BLUE}Stopping any existing containers...${NC}"
docker-compose down 2>/dev/null || true

# Start services
echo -e "${BLUE}Starting services...${NC}"
docker-compose up -d

# Wait for services to be healthy
echo -e "${BLUE}Waiting for services to be healthy...${NC}"

# Wait for PostgreSQL
echo "  Waiting for PostgreSQL..."
for i in {1..30}; do
  if docker exec newsshelf_postgres pg_isready -U postgres &> /dev/null; then
    echo -e "${GREEN}  ✓ PostgreSQL is ready${NC}"
    break
  fi
  echo "    Attempt $i/30..."
  sleep 2
done

# Wait for RabbitMQ
echo "  Waiting for RabbitMQ..."
for i in {1..30}; do
  if docker exec newsshelf_rabbitmq rabbitmq-diagnostics ping &> /dev/null; then
    echo -e "${GREEN}  ✓ RabbitMQ is ready${NC}"
    break
  fi
  echo "    Attempt $i/30..."
  sleep 2
done

# Wait for User Service
echo "  Waiting for User Service..."
for i in {1..30}; do
  if curl -s http://localhost:5001/health > /dev/null; then
    echo -e "${GREEN}  ✓ User Service is ready${NC}"
    break
  fi
  echo "    Attempt $i/30..."
  sleep 2
done

# Wait for Search Service
echo "  Waiting for Search Service..."
for i in {1..30}; do
  if curl -s http://localhost:5002/health > /dev/null; then
    echo -e "${GREEN}  ✓ Search Service is ready${NC}"
    break
  fi
  echo "    Attempt $i/30..."
  sleep 2
done

# Wait for Rec Service
echo "  Waiting for Rec Service..."
for i in {1..30}; do
  if curl -s http://localhost:8001/health > /dev/null; then
    echo -e "${GREEN}  ✓ Rec Service is ready${NC}"
    break
  fi
  echo "    Attempt $i/30..."
  sleep 2
done

# Wait for Frontend
echo "  Waiting for Frontend..."
for i in {1..30}; do
  if curl -s http://localhost:3000 > /dev/null; then
    echo -e "${GREEN}  ✓ Frontend is ready${NC}"
    break
  fi
  echo "    Attempt $i/30..."
  sleep 2
done

# Print summary
echo ""
echo -e "${GREEN}================================${NC}"
echo -e "${GREEN}✓ All services are running!${NC}"
echo -e "${GREEN}================================${NC}"
echo ""

echo "📍 Access Points:"
echo ""
echo -e "${BLUE}Frontend:${NC}"
echo "  http://localhost:3000"
echo ""
echo -e "${BLUE}APIs:${NC}"
echo "  User Service: http://localhost:5001/swagger"
echo "  Search Service: http://localhost:5002/swagger"
echo "  Rec Service: http://localhost:8001/api/v1/docs"
echo ""
echo -e "${BLUE}Message Queue:${NC}"
echo "  RabbitMQ Management: http://localhost:15672 (guest/guest)"
echo ""
echo -e "${BLUE}Database:${NC}"
echo "  PostgreSQL: localhost:5432 (postgres/postgres)"
echo ""

echo "📋 Useful Commands:"
echo ""
echo "  View logs:"
echo "    docker-compose logs -f <service>"
echo ""
echo "  Stop services:"
echo "    docker-compose down"
echo ""
echo "  Stop and remove data:"
echo "    docker-compose down -v"
echo ""
echo "  Restart a service:"
echo "    docker-compose restart <service>"
echo ""
echo "  View all containers:"
echo "    docker-compose ps"
echo ""

echo "🧪 Test API Endpoints:"
echo ""
echo "  Register:"
echo "    curl -X POST http://localhost:5001/api/auth/register \\"
echo "      -H 'Content-Type: application/json' \\"
echo "      -d '{\"email\":\"test@example.com\",\"password\":\"Test123!@#\",\"displayName\":\"Test\"}'"
echo ""
echo "  Login:"
echo "    curl -X POST http://localhost:5001/api/auth/login \\"
echo "      -H 'Content-Type: application/json' \\"
echo "      -d '{\"email\":\"test@example.com\",\"password\":\"Test123!@#\"}'"
echo ""
echo "  Search News:"
echo "    curl 'http://localhost:5002/api/news/search?query=technology'"
echo ""
echo "  Get Recommendations:"
echo "    curl 'http://localhost:8001/api/v1/recommendations?userId=1'"
echo ""

echo -e "${GREEN}Ready to go! 🚀${NC}"
