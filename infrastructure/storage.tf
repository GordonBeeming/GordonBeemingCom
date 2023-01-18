resource "azurerm_storage_account" "content" {
  name                = var.appname
  location            = data.azurerm_resource_group.main.location
  resource_group_name = var.resource_group_name

  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_storage_container" "binaries" {
  name                  = "binaries"
  storage_account_name  = azurerm_storage_account.content.name
  container_access_type = "private"

  lifecycle {
    prevent_destroy = true
  }
}

resource "azurerm_role_assignment" "preview_storage" {
  scope                = azurerm_storage_account.content.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = azurerm_user_assigned_identity.app.principal_id
}
