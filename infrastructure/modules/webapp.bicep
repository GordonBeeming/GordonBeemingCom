// Bicep module for Azure Linux Web App configurations

param name string
param location string
param resourceGroupName string
param servicePlanId string
param httpsOnly bool = true
param clientAffinityEnabled bool = true
param dockerImage string
param dockerImageTag string
param appSettings object
param connectionStrings object
param identityIds array

resource webApp 'Microsoft.Web/sites@2021-02-01' = {
  name: name
  location: location
  properties: {
    serverFarmId: servicePlanId
    httpsOnly: httpsOnly
    clientAffinityEnabled: clientAffinityEnabled
    siteConfig: {
      appSettings: [
        {
          name: 'DOCKER_REGISTRY_SERVER_URL'
          value: 'https://ghcr.io'
        }
        {
          name: 'WEBSITES_ENABLE_APP_SERVICE_STORAGE'
          value: 'false'
        }
      ]
      linuxFxVersion: 'DOCKER|${dockerImage}:${dockerImageTag}'
      alwaysOn: true
    }
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      identityIds: {}
    }
  }
}

output webAppId string = webApp.id
output webAppUrl string = webApp.defaultHostName
