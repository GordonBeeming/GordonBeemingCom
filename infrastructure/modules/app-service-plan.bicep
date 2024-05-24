// Bicep module for Azure App Service Plan

resource appServicePlan 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: '${appName}-${environmentName}-asp-${uniqueString}'
  location: resourceGroup().location
  properties: {
    reserved: true // Indicates Linux app service plan
  }
  sku: {
    name: 'B3'
    tier: 'Basic'
  }
  kind: 'linux'
  tags: {
    'Environment': environmentName
  }
}
