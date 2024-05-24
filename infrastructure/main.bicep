// Main Bicep file to define core Azure infrastructure

module appInsights './modules/app-insights.bicep' = {
  name: 'appInsightsModule'
}

module appServicePlan './modules/app-service-plan.bicep' = {
  name: 'appServicePlanModule'
}

module cloudflare './modules/cloudflare.bicep' = {
  name: 'cloudflareModule'
}

module githubSecrets './modules/github-secrets.bicep' = {
  name: 'githubSecretsModule'
}

module identity './modules/identity.bicep' = {
  name: 'identityModule'
}

module network './modules/network.bicep' = {
  name: 'networkModule'
}

module resourceGroup './modules/resource-group.bicep' = {
  name: 'resourceGroupModule'
}

module sqlServer './modules/sql-server.bicep' = {
  name: 'sqlServerModule'
}

module storage './modules/storage.bicep' = {
  name: 'storageModule'
}

module webapp './modules/webapp.bicep' = {
  name: 'webappModule'
}
