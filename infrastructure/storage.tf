data "azurerm_storage_account" "shared" {
  name                = var.appname
  resource_group_name = var.resource_group_name
}
