# 👨‍💻 LOCAL DEVELOPMENT GUIDE

## Налаштування локального оточення для розробки

### Prerequisites

- Python 3.11 або вищих
- PostgreSQL 15+
- RabbitMQ 3+
- Git
- Visual Studio Code (optional)

### 1. Встановлення залежностей

```bash
# Перейти в папку
cd RecService

# Створити віртуальне оточення
python -m venv venv

# Активувати (Linux/Mac)
source venv/bin/activate

# Активувати (Windows)
venv\Scripts\activate

# Встановити залежності
pip install -r requirements.txt

# Додати dev зависимості
pip install pytest pytest-asyncio pytest-cov black flake8 mypy
```

### 2. Налаштування PostgreSQL

#### Локально

```bash
# Встановити PostgreSQL (якщо не встановлено)
# Ubuntu/Debian
sudo apt-get install postgresql postgresql-contrib

# macOS
brew install postgresql

# Запустити сервер
postgres -D /usr/local/var/postgres

# Створити базу даних
createdb -U postgres recommendations

# Перевірити підключення
psql -U postgres -d recommendations
```

#### Через Docker

```bash
# Запустити тільки PostgreSQL
docker run --name postgres_local \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=recommendations \
  -p 5432:5432 \
  -v postgres_data:/var/lib/postgresql/data \
  postgres:15
```

### 3. Налаштування RabbitMQ

#### Локально

```bash
# Ubuntu/Debian
sudo apt-get install rabbitmq-server
sudo systemctl start rabbitmq-server

# macOS
brew install rabbitmq
brew services start rabbitmq

# Перевірити
rabbitmq-diagnostics ping
```

#### Через Docker

```bash
docker run --name rabbitmq_local \
  -p 5672:5672 \
  -p 15672:15672 \
  rabbitmq:3.13-management-alpine
```

### 4. Налаштування .env для локальної розробки

```bash
# .env.local
DATABASE_URL=postgresql://postgres:postgres@localhost:5432/recommendations
RABBITMQ_URL=amqp://guest:guest@localhost:5672/
LOG_LEVEL=DEBUG
EMBEDDING_MODEL_NAME=sentence-transformers/all-MiniLM-L6-v2
```

### 5. Ініціалізація БД

```bash
# Створити таблиці
python -m src.scripts.create_tables

# Імпортувати новини
python -m src.scripts.import_news

# Генерувати embeddings (довгий процес, може зайняти декілька хвилин)
python -m src.scripts.generate_embeddings
```

### 6. Запуск сервісів локально

#### Terminal 1 - API Server

```bash
# Активувати venv
source venv/bin/activate

# Запустити API
uvicorn src.main:app --reload --port 8001

# --reload: автоматичне перезавантаження при зміні коду
```

#### Terminal 2 - RabbitMQ Consumer

```bash
# Активувати venv
source venv/bin/activate

# Запустити consumer
python -m src.queue.consumer
```

#### Terminal 3 - Test Producer (optional)

```bash
# Активувати venv
source venv/bin/activate

# Відправляти тестові события
python src/tests/producer.py simulate 1 10
```

## 🧪 Розробка

### Структура коду

```
src/
├── api/          # REST endpoints
├── ml/           # Machine learning models
├── models/       # SQLAlchemy models
├── queue/        # Message queue consumers
├── scripts/      # Utility scripts
├── services/     # Business logic
├── tests/        # Tests
├── config.py     # Configuration
├── db.py         # Database setup
├── logger.py     # Logging
├── main.py       # FastAPI app
└── schemas.py    # Pydantic models
```

### Додавання нової функції

1. **Criar модель (якщо потрібна)**

```python
# src/models/new_model.py
from sqlalchemy import Column, String
from src.db import Base

class NewModel(Base):
    __tablename__ = "new_table"
    # ... columns
```

2. **Criar сервіс**

```python
# src/services/new_service.py
from src.logger import logger

def new_function():
    logger.info("Doing something")
    # ...
```

3. **Додати endpoint**

```python
# src/api/recommend.py
@router.get("/new-endpoint")
def new_endpoint():
    return {"message": "success"}
```

4. **Написати тести**

```python
# src/tests/test_new_feature.py
def test_new_function():
    # ...
    assert result == expected
```

5. **Запустити тести**

```bash
pytest src/tests/test_new_feature.py -v
```

### Стиль коду

```bash
# Format code
black src/

# Check style
flake8 src/

# Type checking
mypy src/

# Run all checks
black src/ && flake8 src/ && mypy src/
```

### Debugging

#### VS Code

Створіть `.vscode/launch.json`:

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "FastAPI",
      "type": "python",
      "request": "launch",
      "module": "uvicorn",
      "args": ["src.main:app", "--reload"],
      "jinja": true,
      "justMyCode": true
    },
    {
      "name": "Consumer",
      "type": "python",
      "request": "launch",
      "module": "src.queue.consumer",
      "justMyCode": true
    }
  ]
}
```

#### Логування

```python
from src.logger import logger

logger.debug("Debug message")
logger.info("Info message")
logger.warning("Warning message")
logger.error("Error message")
```

## 📊 Тестування

### Unit Tests

```bash
# Run all tests
pytest

# Run with coverage
pytest --cov=src

# Run specific test
pytest src/tests/test_recommendation_service.py::TestRecommendationService::test_process_news_view
```

### Integration Tests

```bash
# Run only API tests
pytest src/tests/test_api.py -v
```

### Test Database

Тесты автоматично використовують окрему test БД:

```python
@pytest.fixture
def db_session():
    Base.metadata.create_all(bind=engine)
    session = SessionLocal()
    yield session
    session.close()
    Base.metadata.drop_all(bind=engine)
```

## 📦 Менеджмент залежностей

### Додати нову залежність

```bash
# Встановити
pip install package-name

# Додати в requirements.txt
pip freeze > requirements.txt
```

### Оновити залежності

```bash
pip install --upgrade -r requirements.txt
```

## 🔍 Монітринг

### Logs

```bash
# Real-time logs
tail -f logs/app.log

# Errors only
grep ERROR logs/error.log

# Last 50 lines
tail -50 logs/app.log
```

### Database

```bash
# Connect to PostgreSQL
psql -U postgres -d recommendations

# List tables
\dt

# Query news
SELECT COUNT(*) FROM news;

# Query activities
SELECT * FROM user_activity LIMIT 10;
```

### RabbitMQ

```bash
# Check RabbitMQ status
sudo rabbitmqctl status

# List queues
sudo rabbitmqctl list_queues

# Management UI
# http://localhost:15672 (guest:guest)
```

## 🚀 Performance Optimization

### Profiling

```python
import cProfile
import pstats

profiler = cProfile.Profile()
profiler.enable()

# ... ваш код

profiler.disable()
stats = pstats.Stats(profiler)
stats.sort_stats('cumulative').print_stats(10)
```

### Load Testing

```bash
# Встановити locust
pip install locust

# Запустити load test
locust -f src/tests/locustfile.py
```

## 🐳 Docker для розробки

### Docker Compose для розробки

```yaml
version: "3.8"
services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"

  rabbitmq:
    image: rabbitmq:3-management
    ports:
      - "5672:5672"
      - "15672:15672"
```

### Запуск тільки залежностей

```bash
docker-compose up -d postgres rabbitmq
```

## 📚 Корисні посилання

- [FastAPI Documentation](https://fastapi.tiangolo.com/)
- [SQLAlchemy ORM](https://docs.sqlalchemy.org/)
- [Pydantic](https://docs.pydantic.dev/)
- [RabbitMQ Python](https://pika.readthedocs.io/)
- [Sentence Transformers](https://www.sbert.net/)
- [Pytest](https://docs.pytest.org/)

## 🤝 Contribution

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Write tests
4. Commit changes (`git commit -m 'Add amazing feature'`)
5. Push to branch (`git push origin feature/amazing-feature`)
6. Open Pull Request

## 🐛 Troubleshooting

### "ModuleNotFoundError: No module named 'src'"

```bash
# Переконайтесь, що ви в правильній папці
pwd  # Should show .../RecService

# Активуйте venv
source venv/bin/activate
```

### "Connection refused" до PostgreSQL

```bash
# Перевірте, чи запущена база
psql -U postgres -d postgres -c "SELECT 1"

# Запустіть (якщо потрібно)
postgres -D /usr/local/var/postgres
```

### "Connection refused" до RabbitMQ

```bash
# Перевірте статус
rabbitmq-diagnostics ping

# Запустіть (якщо потрібно)
rabbitmq-server
```

---

**Happy coding! 🎉**
