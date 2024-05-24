// Bicep module for Azure SQL Server and databases

param location string = 'eastus' // Default location
param sqlServerName string = 'sqlServer-${uniqueString(resourceGroup().id)}'
param sqlDatabaseName string = 'sqlDatabase-${uniqueString(resourceGroup().id)}'
param sqlServerAdminLogin string
param sqlServerAdminPassword string

resource sqlServer 'Microsoft.Sql/servers@2020-11-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlServerAdminLogin
    administratorLoginPassword: sqlServerAdminPassword
    version: '12.0'
    minimumTlsVersion: '1.2'
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2020-11-01-preview' = {
  name: '${sqlServer.name}/${sqlDatabaseName}'
  location: location
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: '2147483648'
    sku: {
      name: 'Basic'
    }
  }
  dependsOn: [
    sqlServer
  ]
}

output sqlServerName string = sqlServer.name
output sqlDatabaseName string = sqlDatabase.name
