
resource "azuread_application" "main" {
  display_name = "${var.appname}-${var.environment_name}-${random_integer.ri.result}"
}

resource "azuread_service_principal" "main" {
  application_id = azuread_application.main.application_id
}

resource "azurerm_role_assignment" "rg-owner" {
  scope                = data.azurerm_resource_group.main.id
  role_definition_name = "Contributor"
  principal_id         = azuread_service_principal.main.object_id
}

resource "azurerm_user_assigned_identity" "app" {
  resource_group_name = data.azurerm_resource_group.main.name
  location            = data.azurerm_resource_group.main.location

  name = "${var.environment_name}-app-user-${random_integer.ri.result}"
}

output "azure_app" {
  value = azuread_application.main.display_name
}

output "app_client_id" {
  value = azuread_application.main.application_id
}

output "mi_client_id" {
  value = azurerm_user_assigned_identity.app.client_id
}
