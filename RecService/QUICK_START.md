# 🚀 QUICK START GUIDE - Recommendation Service

## За 5 хвилин від нуля до роботи

### Кроки

#### 1️⃣ Перейти в папку проекту

```bash
cd RecService
```

#### 2️⃣ Запустити Docker Compose

```bash
docker-compose up -d
```

#### 3️⃣ Дочекатись ініціалізації (30 сек)

Контейнери будуть завантажуватись, БД буде ініціалізована, дані імпортовані.

#### 4️⃣ Перевірити здоров'я сервісу

```bash
curl http://localhost:8001/health
```

Повинен відповісти:

```json
{
  "status": "healthy",
  "database": "connected"
}
```

#### 5️⃣ Отримати рекомендації

```bash
curl "http://localhost:8001/api/v1/recommendations/1?count=5"
```

## 🌐 Доступні сервіси

| Сервіс           | URL                                | Опис                                      |
| ---------------- | ---------------------------------- | ----------------------------------------- |
| API              | http://localhost:8001              | REST API для рекомендацій                 |
| Swagger Docs     | http://localhost:8001/api/v1/docs  | Interactive API docs                      |
| ReDoc            | http://localhost:8001/api/v1/redoc | Alternative API docs                      |
| RabbitMQ Console | http://localhost:15672             | Message queue management                  |
| PostgreSQL       | localhost:5432                     | Database (user: postgres, pass: postgres) |

## 🧪 Тестування

### Отправити тестовий event

```bash
python src/tests/producer.py single 1 10
```

Сервіс автоматично оберне це в рекомендації.

### Або симулювати поведінку користувача

```bash
python src/tests/producer.py simulate 2 20
```

Це відправить 20 подій від користувача 2.

## 📊 Основні API endpoints

### 1. Отримати рекомендації

```bash
GET /api/v1/recommendations/{user_id}?count=10&threshold=0.3
```

### 2. Отримати інтереси користувача

```bash
GET /api/v1/recommendations/{user_id}/interests
```

### 3. Отримати популярні новини

```bash
GET /api/v1/recommendations/popular/news?count=10
```

## 🔍 Перевірка логів

### API logs

```bash
docker logs rec_api -f
```

### Consumer logs

```bash
docker logs rec_consumer -f
```

### Local logs

```bash
tail -f logs/app.log
```

## 🛑 Зупинити сервіси

```bash
docker-compose down
```

## 🔄 Перезапустити все

```bash
docker-compose restart
```

## 🧹 Очистити все (включно з БД)

```bash
docker-compose down -v
docker-compose up -d
```

## 💡 Порядок запуску

Сервіси стартують в такому порядку (автоматично):

1. **PostgreSQL** - чекає 10 сек на ініціалізацію
2. **RabbitMQ** - готується до прийому подій
3. **API** - запускає ініціалізацію БД:
   - Створює таблиці
   - Імпортує новини з JSON
   - Генерує embeddings
4. **Consumer** - слухає черги RabbitMQ

## ✅ Як перевірити, що все працює

```bash
# 1. Перевірити контейнери
docker ps

# 2. Перевірити health
curl http://localhost:8001/health

# 3. Відправити test event
python src/tests/producer.py single 1 10

# 4. Отримати рекомендації
curl http://localhost:8001/api/v1/recommendations/1

# 5. Перевірити логи
docker logs rec_api
docker logs rec_consumer
```

## 🐛 Типові проблеми

### "Connection refused" на port 8001

```bash
# Контейнер ще запускається, чекайте 30 сек
docker logs rec_api -f
```

### "Database connection error"

```bash
# Postgres ще не готов, перезапустіть
docker-compose restart postgres
docker-compose restart recommendation-api
```

### "No embeddings found"

```bash
# Запустіть генерацію вручну
docker exec rec_api python -m src.scripts.generate_embeddings
```

## 📚 Детальніше

Для полної документації див. [README.md](README.md)

## 🎯 Приклади використання

```bash
# Python
python examples.py

# Curl - Рекомендації для користувача 1
curl "http://localhost:8001/api/v1/recommendations/1"

# Curl - Інтереси користувача 1
curl "http://localhost:8001/api/v1/recommendations/1/interests"

# Curl - Популярні новини
curl "http://localhost:8001/api/v1/recommendations/popular/news"
```

## 📞 Потрібна допомога?

1. Перевірте логи: `docker logs rec_api`
2. Прочитайте README.md
3. Проверьте examples.py
4. Дивіться код в `src/api/recommend.py`

---

**Успіхів! 🚀**
