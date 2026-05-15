#!/usr/bin/env bash
# One-shot Azure provisioning for Vital Photography.
# Run once to create all resources. Re-running is safe (idempotent via Bicep).
#
# Prerequisites:
#   az login
#   az account set --subscription <your-subscription-id>
#
# Usage:
#   export SQL_PASSWORD="<strong-password>"
#   export JWT_SECRET="<32+-char-random-string>"
#   export ADMIN_HASH="<bcrypt hash of your admin password>"
#   bash infra/provision.sh

set -euo pipefail

RESOURCE_GROUP="vital-photography-rg"
LOCATION="westeurope"
DEPLOYMENT_NAME="vital-photography-$(date +%Y%m%d%H%M%S)"

echo "Creating resource group $RESOURCE_GROUP..."
az group create --name "$RESOURCE_GROUP" --location "$LOCATION"

echo "Deploying Bicep template..."
az deployment group create \
  --resource-group "$RESOURCE_GROUP" \
  --name            "$DEPLOYMENT_NAME" \
  --template-file   "$(dirname "$0")/bicep/main.bicep" \
  --parameters env=prod \
  --parameters sqlAdminPassword="$SQL_PASSWORD" \
  --parameters jwtSecret="$JWT_SECRET" \
  --parameters adminPasswordHash="$ADMIN_HASH"

echo ""
echo "Deployment complete. Outputs:"
az deployment group show \
  --resource-group "$RESOURCE_GROUP" \
  --name            "$DEPLOYMENT_NAME" \
  --query properties.outputs \
  --output table

echo ""
echo "Next steps:"
echo "  1. Copy 'apiUrl' → GitHub secret API_BASE_URL"
echo "  2. Copy 'cdnUrl' → GitHub secret BLOB_BASE_URL"
echo "  3. Copy 'staticWebUrl' → set as your custom domain in Azure portal"
echo "  4. Download the App Service publish profile → GitHub secret AZURE_APP_SERVICE_PUBLISH_PROFILE"
echo "  5. Get the Static Web Apps deployment token → GitHub secret AZURE_STATIC_WEB_APPS_TOKEN"
