#!/bin/bash
set -e

echo "🔨 Building image..."
docker build --no-cache -f sales-visits-be/Dockerfile -t salesvisits.azurecr.io/sales-visits-be:latest .

echo "🔐 Logging into ACR..."
az acr login --name salesvisits

echo "📤 Pushing image to ACR..."
docker push salesvisits.azurecr.io/sales-visits-be:latest

echo "🚀 Deploying to Container App..."
az containerapp update \
  --name sales-visits-be \
  --resource-group container-apps \
  --image salesvisits.azurecr.io/sales-visits-be:latest \
  --set-env-vars DEPLOY_TIME="$(date +%s)"

echo "✅ Done! Verifying revision..."
az containerapp revision list \
  --name sales-visits-be \
  --resource-group container-apps \
  -o table
