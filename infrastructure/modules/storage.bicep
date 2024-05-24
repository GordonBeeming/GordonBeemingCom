// Bicep module for Azure storage account and container

param storageAccountName string
param location string = resourceGroup().location
param accountTier string = 'Standard'
param replicationType string = 'LRS'
param containerName string = 'container'

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: '${accountTier}_${replicationType}'
  }
  kind: 'StorageV2'
}

resource storageContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  name: '${storageAccount.name}/default/${containerName}'
  dependsOn: [
    storageAccount
  ]
}
