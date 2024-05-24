// Bicep module for Azure Resource Group

param location string = 'eastus' // Default location for the resource group
param resourceGroupName string = 'myResourceGroup' // Default name for the resource group

resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
}

output resourceGroupId string = resourceGroup.id
