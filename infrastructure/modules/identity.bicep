// Bicep module for Azure identity resources

resource main 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: 'userAssignedIdentity'
  location: resourceGroup().location
  properties: {}
}

output azureApp string = main.name
output appClientId string = main.clientId
output miClientId string = main.principalId
