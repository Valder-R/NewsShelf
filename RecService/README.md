# Recommendation Service (RecService)

Мікросервіс для генерації персоналізованих рекомендацій новин на основі поведінки користувачів, використовуючи машинне навчання та embedding моделей.

## 📋 Описание

Сервіс рекомендацій є частиною мікросервісної архітектури **NewsShelf** і забезпечує:

- ✅ Персоналізовані рекомендації новин для користувачів
- ✅ Обробку подій активності користувачів через RabbitMQ
- ✅ Генерацію семантичних embeddings для новин
- ✅ Розрахунок схожості текстів з використанням sentence-transformers
- ✅ REST API для отримання рекомендацій
- ✅ Трекування інтересів користувачів за категоріями

## 🏗️ Архітектура

```
┌─────────────────────────────────────────────────────┐
│         Other Microservices (UserService, etc)      │
└─────────────────┬───────────────────────────────────┘
                  │ RabbitMQ Events
                  ▼
        ┌─────────────────────┐
        │  Recommendation API │ (Port 8001)
        │   - REST Endpoints  │
        │   - Health Check    │
        └────────┬────────────┘
                 │
         ┌───────┴────────┐
         ▼                ▼
    ┌─────────┐      ┌────────────┐
    │Consumer │      │PostgreSQL  │
    │  (Async)│      │  Database  │
    └────┬────┘      └────────────┘
         │
         │ Processes events
         ▼
    ┌─────────────┐
    │Embedding    │
    │Generation   │
    │& Storage    │
    └─────────────┘
```

## 🚀 Быстрый старт

### Prerequisites

- Docker & Docker Compose
- Python 3.11+ (для локальной разработки)
- Git

### Запуск сервиса

#### 1️⃣ С Docker Compose (рекомендуется)

```bash
cd RecService
docker-compose up -d
```

Сервіс буде доступен на `http://localhost:8001`

#### 2️⃣ Локально (для разработки)

```bash
# Установить зависимости
pip install -r requirements.txt

# Установить переменные окружения
export DATABASE_URL="postgresql://postgres:postgres@localhost:5432/recommendations"
export RABBITMQ_URL="amqp://guest:guest@localhost:5672/"

# Запустить API
uvicorn src.main:app --reload --port 8001

# В другом терминале - запустить Consumer
python -m src.queue.consumer
```

## 📚 API Endpoints

### Health Check

```bash
GET /health
```

Response:

```json
{
  "status": "healthy",
  "database": "connected",
  "message": "Recommendation Service is running"
}
```

### Получить рекомендации для пользователя

```bash
GET /api/v1/recommendations/{user_id}?count=10&threshold=0.3
```

Query Parameters:

- `count` (int, 1-50): Количество рекомендаций (default: 10)
- `threshold` (float, 0.0-1.0): Порог схожести (default: 0.3)

Response:

```json
{
  "user_id": 1,
  "recommendations": [
    {
      "news_id": 5,
      "title": "Новина про AI",
      "category": "Technology",
      "short_description": "...",
      "authors": "...",
      "similarity_score": 0.85,
      "link": "..."
    }
  ],
  "total_count": 10
}
```

### Получить интересы пользователя

```bash
GET /api/v1/recommendations/{user_id}/interests
```

Response:

```json
{
  "user_id": 1,
  "interests": {
    "Technology": 0.6,
    "Sports": 0.4
  },
  "total_activities": 25
}
```

### Получить популярные новины

```bash
GET /api/recommendations/popular/news?count=10
```

### API Documentation

Swagger документация доступна на:

- http://localhost:8001/api/v1/docs
- http://localhost:8001/api/v1/redoc

## 🧪 Тестирование

### Запуск unit тестов

```bash
pytest src/tests/test_recommendation_service.py -v
```

### Запуск интеграционных тестов

```bash
pytest src/tests/test_api.py -v
```

### Запуск всех тестов

```bash
pytest -v
```

## 📤 Отправка событий через RabbitMQ

### Методом 1: Использование test producer

```bash
# Отправить одно событие
cd src/tests
python producer.py single 1 10

# Симулировать поведение пользователя
python producer.py simulate 1 20

# Отправить пакет событий
python producer.py batch 1 5 10 15 20
```

### Методом 2: Прямая отправка через RabbitMQ

```python
import pika
import json

connection = pika.BlockingConnection(
    pika.ConnectionParameters(host="rabbitmq")
)
channel = connection.channel()
channel.queue_declare(queue="news_events", durable=True)

event = {
    "event": "news_viewed",
    "user_id": 1,
    "news_id": 10
}

channel.basic_publish(
    exchange="",
    routing_key="news_events",
    body=json.dumps(event)
)
connection.close()
```

## 📊 RabbitMQ Management Console

Доступен на: http://localhost:15672

- Користувач: `guest`
- Пароль: `guest`

## 🗂️ Структура проекта

```
RecService/
├── src/
│   ├── api/
│   │   └── recommend.py          # REST endpoints
│   ├── ml/
│   │   └── embedding_model.py    # Embedding model wrapper
│   ├── models/
│   │   ├── news.py               # News model
│   │   └── user_activity.py      # User activity model
│   ├── queue/
│   │   └── consumer.py           # RabbitMQ consumer
│   ├── scripts/
│   │   ├── init_db.py            # Database initialization
│   │   ├── import_news.py        # Import news from JSON
│   │   ├── generate_embeddings.py # Generate embeddings
│   │   └── create_tables.py      # Create database tables
│   ├── services/
│   │   └── recommendation_service.py  # Business logic
│   ├── tests/
│   │   ├── producer.py           # Test RabbitMQ producer
│   │   ├── test_api.py           # API tests
│   │   └── test_recommendation_service.py  # Service tests
│   ├── config.py                 # Configuration with Pydantic
│   ├── db.py                     # Database connection
│   ├── logger.py                 # Logging setup
│   ├── main.py                   # FastAPI app
│   └── schemas.py                # Pydantic models
├── docker/
│   └── postgres/
│       └── init.sql              # PostgreSQL init script
├── data/
│   └── News_Category.json        # News dataset
├── docker-compose.yml            # Docker Compose config
├── Dockerfile                    # Docker image
├── requirements.txt              # Python dependencies
├── .env                          # Environment variables
└── README.md                     # This file
```

## 🔧 Конфигурация

### Переменные окружения (.env)

```bash
# Database
DATABASE_URL=postgresql://postgres:postgres@postgres:5432/recommendations

# RabbitMQ
RABBITMQ_URL=amqp://guest:guest@rabbitmq:5672/
QUEUE_NAME=news_events

# API
API_TITLE=Recommendation Service API
API_VERSION=1.0.0
RECOMMENDATIONS_COUNT=10
SIMILARITY_THRESHOLD=0.3

# Logging
LOG_LEVEL=INFO
```

## 🔍 Мониторинг

### Logs

```bash
# API logs
docker logs rec_api -f

# Consumer logs
docker logs rec_consumer -f

# RabbitMQ logs
docker logs rec_rabbitmq -f

# PostgreSQL logs
docker logs rec_postgres -f
```

### Local logs

```bash
# Application logs
tail -f logs/app.log

# Error logs
tail -f logs/error.log
```

## 📈 Производительность

### Оптимизация

- ✅ Embedding кэширование в БД
- ✅ Batch обработка событий
- ✅ Connection pooling для PostgreSQL
- ✅ Асинхронная обработка событий RabbitMQ
- ✅ Health checks для автоматического перезапуска

### Масштабируемость

- 📊 Горизонтальное масштабирование через несколько consumer инстансов
- 🔄 Load balancing через RabbitMQ queue
- 💾 PostgreSQL можно горизонтально масштабировать с репликацией

## 🤝 Интеграция с другими сервисами

### Отправка событий

```bash
# UserService отправляет события о просмотрах новин
curl -X POST http://rabbitmq:5672 \
  -d '{"event": "news_viewed", "user_id": 1, "news_id": 10}'
```

### Получение рекомендаций

```bash
# SearchService или Frontend запрашивает рекомендации
curl http://recommendation-api:8001/api/v1/recommendations/1?count=5
```

## 🐛 Troubleshooting

### 1. "Connection refused" к RabbitMQ

```bash
# Проверить статус контейнера
docker ps | grep rabbitmq

# Перезапустить
docker-compose restart rabbitmq
```

### 2. "Database connection error"

```bash
# Проверить PostgreSQL
docker logs rec_postgres

# Пересоздать базу
docker-compose down -v
docker-compose up -d
```

### 3. "No embeddings generated"

```bash
# Запустить генерацию embeddings вручную
docker exec rec_api python -m src.scripts.generate_embeddings
```

### 4. "Consumer не обрабатывает события"

```bash
# Проверить логи consumer
docker logs rec_consumer -f

# Перезапустить
docker-compose restart recommendation-consumer
```

## 📝 Логирование

Сервис использует **loguru** для структурированного логирования:

- Консоль: color-formatted logs
- `logs/app.log`: Все логи
- `logs/error.log`: Только ошибки

## 🚦 Health Check

Сервис включает health check endpoints для Docker и Kubernetes:

```bash
# Basic health check
curl http://localhost:8001/health

# Docker health check
docker ps | grep rec_api  # Должен показать "healthy"
```

## 📚 Документація

- [Pydantic Settings](https://docs.pydantic.dev/latest/concepts/pydantic_settings/)
- [SQLAlchemy ORM](https://docs.sqlalchemy.org/en/20/)
- [Sentence Transformers](https://www.sbert.net/)
- [FastAPI](https://fastapi.tiangolo.com/)
- [RabbitMQ Python Client](https://pika.readthedocs.io/)

## 🤖 Machine Learning

### Embedding Model

- **Model**: `sentence-transformers/all-MiniLM-L6-v2`
- **Dimensions**: 384
- **Use case**: Semantic similarity between news articles
- **Performance**: Fast inference (~10ms per article)

### Recommendation Algorithm

1. **User Profiling**: Aggregate embeddings from viewed articles
2. **Candidate Generation**: All unviewed articles with embeddings
3. **Scoring**: Cosine similarity between user profile and candidates
4. **Ranking**: Sort by similarity score
5. **Filtering**: Apply similarity threshold
6. **Fallback**: Return popular news if no user activity

## 📦 Deployment

### Docker Swarm

```bash
docker stack deploy -c docker-compose.yml newsshelf
```

### Kubernetes

```bash
kubectl apply -f k8s-manifest.yaml
```

### Cloud (AWS, GCP, Azure)

Docker образ можно опубліковати в реєстр та розгорнути через:

- AWS ECS
- Google Cloud Run
- Azure Container Instances

## 📄 License

MIT License - See LICENSE file

## 👨‍💻 Розробка

### Додавання нової 功能

1. Create branch: `git checkout -b feature/my-feature`
2. Write tests
3. Implement feature
4. Run tests: `pytest`
5. Submit pull request

### Code Style

- PEP 8
- Type hints
- Docstrings for all functions
- Loguru for logging

## 📞 Контакти

Будь-які питання чи пропозиції:

- GitHub Issues
- Documentation
- Code comments

---

**Created with ❤️ for NewsShelf Microservices**
