@description('Environment suffix — e.g. prod or staging')
param env string = 'prod'

@description('Azure region')
param location string = resourceGroup().location

@description('Admin SQL password')
@secure()
param sqlAdminPassword string

@description('JWT signing secret (min 32 chars)')
@secure()
param jwtSecret string

@description('Bcrypt hash of the admin password for the API')
@secure()
param adminPasswordHash string

var prefix = 'vitalphoto-${env}'

// ── Storage Account ──────────────────────────────────────────────────────────
resource storage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: replace('${prefix}storage', '-', '')
  location: location
  sku: { name: 'Standard_LRS' }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: true
  }
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' = {
  parent: storage
  name: 'default'
}

resource photosContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  parent: blobService
  name: 'photos'
  properties: { publicAccess: 'Blob' }
}

// ── CDN ───────────────────────────────────────────────────────────────────────
resource cdnProfile 'Microsoft.Cdn/profiles@2023-05-01' = {
  name: '${prefix}-cdn'
  location: 'Global'
  sku: { name: 'Standard_Microsoft' }
}

resource cdnEndpoint 'Microsoft.Cdn/profiles/endpoints@2023-05-01' = {
  parent: cdnProfile
  name: '${prefix}-cdn-ep'
  location: 'Global'
  properties: {
    originHostHeader: '${storage.name}.blob.core.windows.net'
    origins: [{
      name: 'blob-origin'
      properties: { hostName: '${storage.name}.blob.core.windows.net' }
    }]
    deliveryPolicy: {
      rules: [{
        name: 'CacheImages'
        order: 1
        conditions: [{
          name: 'UrlFileExtension'
          parameters: {
            typeName: 'DeliveryRuleUrlFileExtensionMatchConditionParameters'
            operator: 'Equal'
            matchValues: ['webp', 'jpg', 'jpeg', 'png']
          }
        }]
        actions: [{
          name: 'CacheExpiration'
          parameters: {
            typeName: 'DeliveryRuleCacheExpirationActionParameters'
            cacheBehavior: 'SetIfMissing'
            cacheType: 'All'
            cacheDuration: '365.00:00:00'
          }
        }]
      }]
    }
  }
}

// ── SQL Server & Database ─────────────────────────────────────────────────────
resource sqlServer 'Microsoft.Sql/servers@2023-02-01-preview' = {
  name: '${prefix}-sql'
  location: location
  properties: {
    administratorLogin: 'sqladmin'
    administratorLoginPassword: sqlAdminPassword
    minimalTlsVersion: '1.2'
  }
}

resource sqlFirewall 'Microsoft.Sql/servers/firewallRules@2023-02-01-preview' = {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: { startIpAddress: '0.0.0.0', endIpAddress: '0.0.0.0' }
}

resource sqlDb 'Microsoft.Sql/servers/databases@2023-02-01-preview' = {
  parent: sqlServer
  name: 'VitalPhotography'
  location: location
  sku: { name: 'Basic', tier: 'Basic' }
}

// ── App Service ───────────────────────────────────────────────────────────────
resource appPlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: '${prefix}-plan'
  location: location
  sku: { name: 'B1', tier: 'Basic' }
  kind: 'linux'
  properties: { reserved: true }
}

resource appService 'Microsoft.Web/sites@2023-01-01' = {
  name: '${prefix}-api'
  location: location
  properties: {
    serverFarmId: appPlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      appSettings: [
        { name: 'ASPNETCORE_ENVIRONMENT',             value: 'Production' }
        { name: 'Jwt__Secret',                        value: jwtSecret }
        { name: 'Jwt__Issuer',                        value: 'vital-photography' }
        { name: 'Jwt__Audience',                      value: 'vital-photography-admin' }
        { name: 'Jwt__ExpiryMinutes',                 value: '60' }
        { name: 'Admin__PasswordHash',                value: adminPasswordHash }
        { name: 'AzureBlob__ConnectionString',        value: 'DefaultEndpointsProtocol=https;AccountName=${storage.name};AccountKey=${storage.listKeys().keys[0].value};EndpointSuffix=core.windows.net' }
        { name: 'AzureBlob__ContainerName',           value: 'photos' }
        { name: 'AzureBlob__CdnBaseUrl',              value: 'https://${cdnEndpoint.properties.hostName}' }
        { name: 'ConnectionStrings__DefaultConnection', value: 'Server=${sqlServer.properties.fullyQualifiedDomainName};Database=VitalPhotography;User Id=sqladmin;Password=${sqlAdminPassword};Encrypt=true;' }
      ]
    }
  }
}

// ── Static Web App (frontend) ─────────────────────────────────────────────────
resource staticWebApp 'Microsoft.Web/staticSites@2023-01-01' = {
  name: '${prefix}-web'
  location: location
  sku: { name: 'Free', tier: 'Free' }
  properties: {}
}

// ── Outputs ───────────────────────────────────────────────────────────────────
output apiUrl        string = 'https://${appService.properties.defaultHostName}'
output cdnUrl        string = 'https://${cdnEndpoint.properties.hostName}'
output staticWebUrl  string = 'https://${staticWebApp.properties.defaultHostname}'
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName
