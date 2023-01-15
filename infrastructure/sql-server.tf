resource "random_uuid" "dbserverpass" {
}
resource "azurerm_mssql_server" "main" {
  name                         = "server-${var.environment_name}-${random_integer.ri.result}"
  resource_group_name          = data.azurerm_resource_group.main.name
  location                     = data.azurerm_resource_group.main.location
  version                      = "12.0"  
  minimum_tls_version          = "1.2"

  administrator_login          = "server-${random_integer.ri.result}"
  administrator_login_password = random_uuid.dbserverpass.result
  
  public_network_access_enabled = true
    
  azuread_administrator {
    login_username = "app${var.environment_name}"
    object_id      = azurerm_user_assigned_identity.app.principal_id
  }

  tags = {
    environment = var.environment_name
  }
}

resource "azurerm_mssql_virtual_network_rule" "sql_net" {
  name      = "sql-vnet-rule"
  server_id = azurerm_mssql_server.main.id
  subnet_id = azurerm_subnet.main.id
}