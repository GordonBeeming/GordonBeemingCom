// Bicep module for Azure Application Insights

resource appInsights 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: '${appName}-${environmentName}-appinsights-${uniqueString(resourceGroup().id)}'
  location: resourceGroup().location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Bluefield'
    Request_Source: 'rest'
    RetentionInDays: 30
    WorkspaceResourceId: logAnalyticsWorkspace.id
  }
}

output instrumentationKey string = appInsights.properties.InstrumentationKey
