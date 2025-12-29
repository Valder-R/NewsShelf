#!/bin/bash

# Clean up Kubernetes resources

set -e

echo "🧹 Cleaning up Kubernetes resources..."

echo "Deleting namespace newsshelf..."
kubectl delete namespace newsshelf --ignore-not-found

echo "✅ Cleanup complete!"
