#!/bin/bash

# NewsShelf Kubernetes Deployment Script

set -e

echo "🚀 Deploying NewsShelf to Kubernetes..."

# Check if kubectl is installed
if ! command -v kubectl &> /dev/null; then
    echo "❌ kubectl is not installed. Please install kubectl first."
    exit 1
fi

# Create namespace
echo "📦 Creating namespace..."
kubectl create namespace newsshelf --dry-run=client -o yaml | kubectl apply -f -

# Apply configurations in order
echo "🔧 Applying PostgreSQL configuration..."
kubectl apply -f k8s/postgres.yaml

echo "📬 Applying RabbitMQ configuration..."
kubectl apply -f k8s/rabbitmq.yaml

echo "👤 Applying User Service configuration..."
kubectl apply -f k8s/user-service.yaml

echo "🔍 Applying Search Service configuration..."
kubectl apply -f k8s/search-service.yaml

echo "🤖 Applying Recommendation Service configuration..."
kubectl apply -f k8s/rec-service.yaml

echo "🖥️  Applying Frontend configuration..."
kubectl apply -f k8s/frontend.yaml

echo "🌐 Applying Ingress and API Gateway..."
kubectl apply -f k8s/ingress.yaml

echo "📈 Applying HPA configuration..."
kubectl apply -f k8s/hpa.yaml

echo ""
echo "✅ Deployment complete!"
echo ""
echo "📊 Checking deployment status..."
kubectl get deployments -n newsshelf
echo ""
echo "🔗 Services:"
kubectl get services -n newsshelf
echo ""
echo "💡 Next steps:"
echo "  1. Wait for all pods to be ready: kubectl get pods -n newsshelf -w"
echo "  2. Check logs: kubectl logs -n newsshelf -l app=<service-name>"
echo "  3. Port forward frontend: kubectl port-forward -n newsshelf svc/frontend 3000:80"
echo "  4. Port forward API Gateway: kubectl port-forward -n newsshelf svc/api-gateway 5000:80"
