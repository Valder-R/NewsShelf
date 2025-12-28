# 🚀 NewsShelf - Deployment Guide

## Docker Compose Setup

### Prerequisites

- Docker 20.10+
- Docker Compose 2.0+
- 8GB RAM (minimum)
- 20GB disk space

### Quick Start

```bash
# Navigate to project root
cd /path/to/NewsShelf

# Start all services
docker-compose up -d

# Check status
docker-compose ps

# View logs
docker-compose logs -f frontend
docker-compose logs -f user-service
docker-compose logs -f search-service
docker-compose logs -f rec-service
```

### Access Points

| Service                | URL                               | Username | Password |
| ---------------------- | --------------------------------- | -------- | -------- |
| Frontend               | http://localhost:3000             | -        | -        |
| User Service API       | http://localhost:5001/swagger     | -        | -        |
| Search Service API     | http://localhost:5002/swagger     | -        | -        |
| Recommendation Service | http://localhost:8001/api/v1/docs | -        | -        |
| RabbitMQ Management    | http://localhost:15672            | guest    | guest    |
| PostgreSQL             | localhost:5432                    | postgres | postgres |

### Troubleshooting

```bash
# Restart a specific service
docker-compose restart user-service

# Rebuild a service
docker-compose up -d --build user-service

# Remove all containers and volumes
docker-compose down -v

# Check service logs
docker logs newsshelf_user_service

# Enter container
docker exec -it newsshelf_postgres psql -U postgres -d newsshelf
```

---

## Kubernetes Deployment

### Prerequisites

- Kubernetes 1.24+
- kubectl 1.24+
- 2 nodes minimum (4GB RAM each)
- Container registry access (Docker Hub, ECR, etc.)

### Build and Push Images

```bash
# Build all images
docker build -t <registry>/user-service:latest UserService/
docker build -t <registry>/search-service:latest SearchService/
docker build -t <registry>/rec-service:latest RecService/
docker build -t <registry>/frontend:latest Frontend/

# Push to registry
docker push <registry>/user-service:latest
docker push <registry>/search-service:latest
docker push <registry>/rec-service:latest
docker push <registry>/frontend:latest
```

### Deploy to Kubernetes

```bash
# Create namespace and deploy
chmod +x k8s/deploy.sh
./k8s/deploy.sh

# Monitor deployment
kubectl get pods -n newsshelf -w
kubectl describe pod -n newsshelf <pod-name>

# Check services
kubectl get svc -n newsshelf

# View ingress
kubectl get ingress -n newsshelf
```

### Port Forwarding for Development

```bash
# Frontend
kubectl port-forward -n newsshelf svc/frontend 3000:80

# API Gateway
kubectl port-forward -n newsshelf svc/api-gateway 5000:80

# PostgreSQL
kubectl port-forward -n newsshelf svc/postgres 5432:5432

# RabbitMQ
kubectl port-forward -n newsshelf svc/rabbitmq 5672:5672
```

### Scaling Services

```bash
# Scale user service to 5 replicas
kubectl scale deployment user-service -n newsshelf --replicas=5

# Check HPA status
kubectl get hpa -n newsshelf
kubectl describe hpa user-service-hpa -n newsshelf
```

### View Logs

```bash
# Live logs from pod
kubectl logs -n newsshelf -l app=user-service -f

# Previous logs (if crashed)
kubectl logs -n newsshelf <pod-name> --previous

# All logs from a deployment
kubectl logs -n newsshelf -l app=user-service --tail=100
```

### Database Initialization

```bash
# Port forward to PostgreSQL
kubectl port-forward -n newsshelf svc/postgres 5432:5432

# In another terminal, connect
psql -h localhost -U postgres -d newsshelf

# Check migrations
SELECT * FROM news WHERE id LIMIT 5;
```

### Troubleshooting Kubernetes

```bash
# Check cluster status
kubectl cluster-info
kubectl get nodes

# Debug pod issues
kubectl describe pod -n newsshelf <pod-name>
kubectl logs -n newsshelf <pod-name>
kubectl exec -it -n newsshelf <pod-name> -- /bin/sh

# Check resource usage
kubectl top nodes
kubectl top pods -n newsshelf

# Check events
kubectl get events -n newsshelf --sort-by='.lastTimestamp'

# Delete and redeploy
kubectl delete deployment user-service -n newsshelf
kubectl apply -f k8s/user-service.yaml
```

### Production Considerations

1. **Image Registry**

   - Use private registry (ECR, GCR, Azure ACR)
   - Enable image scanning
   - Use image pull secrets

2. **Secrets Management**

   - Use Kubernetes Secrets for sensitive data
   - Or use HashiCorp Vault, AWS Secrets Manager
   - Rotate secrets regularly

3. **Persistent Data**

   - Use cloud storage (EBS, AzureDisk, GCE Persistent Disk)
   - Enable backup and replication
   - Test restore procedures

4. **Networking**

   - Use Network Policies for pod-to-pod communication
   - Enable HTTPS/TLS with cert-manager
   - Use Network Load Balancer for production

5. **Monitoring**

   - Install Prometheus for metrics
   - Install ELK stack for logs
   - Use Datadog or New Relic for APM

6. **Backup & Disaster Recovery**
   - Backup PostgreSQL regularly
   - Test recovery procedures
   - Document RTO and RPO

---

## Environment Configuration

### Development

```
ASPNETCORE_ENVIRONMENT=Development
DATABASE=localhost
RABBITMQ=localhost
DEBUG=true
```

### Staging

```
ASPNETCORE_ENVIRONMENT=Staging
DATABASE=staging-postgres.internal
RABBITMQ=staging-rabbitmq.internal
DEBUG=false
```

### Production

```
ASPNETCORE_ENVIRONMENT=Production
DATABASE=prod-postgres.db
RABBITMQ=prod-rabbitmq.broker
DEBUG=false
LOG_LEVEL=Warning
```

---

## Monitoring & Maintenance

### Daily Checks

```bash
# Check pod status
kubectl get pods -n newsshelf

# Check service health
curl http://localhost:5001/health
curl http://localhost:5002/health
curl http://localhost:8001/health

# Check database connection
kubectl exec -n newsshelf postgres-0 -- pg_isready
```

### Weekly Maintenance

```bash
# Check disk usage
df -h /data/

# Backup database
kubectl exec -n newsshelf postgres-0 -- pg_dump newsshelf > backup-$(date +%Y%m%d).sql

# Review logs for errors
kubectl logs -n newsshelf -l app=user-service --since=7d | grep ERROR
```

### Monthly Reviews

- Review and update dependencies
- Check for security vulnerabilities
- Review performance metrics
- Plan scaling if needed

---

## Cleanup & Removal

### Docker Compose

```bash
# Stop all services
docker-compose down

# Remove volumes
docker-compose down -v

# Remove everything (containers, networks, volumes)
docker-compose down -v
docker system prune -a
```

### Kubernetes

```bash
# Delete namespace (removes all resources)
kubectl delete namespace newsshelf

# Or run cleanup script
chmod +x k8s/cleanup.sh
./k8s/cleanup.sh
```

---

## Performance Tips

### Database

- Add indexes on frequently searched columns
- Use query optimization
- Monitor slow queries
- Connection pooling: 10-20 connections per service

### API Services

- Enable gzip compression
- Cache responses where applicable
- Use pagination for list endpoints
- Rate limiting on public endpoints

### Frontend

- Enable gzip compression in nginx
- Use CDN for static assets
- Implement lazy loading for routes
- Optimize images (use WebP format)

### Message Queue

- Monitor queue depth
- Set appropriate TTL for messages
- Use message batching where possible
- Monitor consumer lag

---

## Cost Optimization

### Cloud Deployment

- Use spot/preemptible instances for non-critical services
- Right-size resources based on metrics
- Use auto-scaling aggressively
- Monitor and optimize database usage
- Use managed services where possible (RDS, MQ, etc.)

### On-Premise

- Bin-pack services on nodes
- Use resource quotas to prevent over-allocation
- Monitor and optimize resource usage
- Plan capacity based on growth projections

---

## Emergency Procedures

### Service Failure Recovery

**If User Service is down:**

```bash
kubectl rollout restart deployment/user-service -n newsshelf
kubectl logs -n newsshelf -l app=user-service -f
```

**If Database is down:**

```bash
# Check pod status
kubectl describe pod -n newsshelf postgres-0

# Restore from backup
pg_restore -d newsshelf backup.sql
```

**If RabbitMQ is down:**

```bash
kubectl rollout restart deployment/rabbitmq -n newsshelf
# Messages may be lost, check logs for impact
```

### Data Recovery

```bash
# List backup files
ls -la /data/backups/

# Restore from backup
pg_restore -d newsshelf /data/backups/backup-20240101.sql

# Verify restore
psql -U postgres -d newsshelf -c "SELECT COUNT(*) FROM news;"
```

---

**Last Updated**: December 2024
**Version**: 1.0.0
