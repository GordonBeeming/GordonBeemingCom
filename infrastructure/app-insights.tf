resource "azurerm_log_analytics_workspace" "analytics" {
  name                = "${var.appname}-${var.environment_name}-workspace-${random_integer.ri.result}"
  location            = data.azurerm_resource_group.main.location
  resource_group_name = data.azurerm_resource_group.main.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_application_insights" "tracking" {
  name                = "${var.appname}-${var.environment_name}-appinsights-${random_integer.ri.result}"
  location            = data.azurerm_resource_group.main.location
  resource_group_name = data.azurerm_resource_group.main.name
  workspace_id        = azurerm_log_analytics_workspace.analytics.id
  application_type    = "web"
}