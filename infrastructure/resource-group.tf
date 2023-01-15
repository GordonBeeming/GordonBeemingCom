
data "azurerm_resource_group" "main" {
  name     = var.resource_group_name
}

output "main_resource_group" {
  value = data.azurerm_resource_group.main.name
}
