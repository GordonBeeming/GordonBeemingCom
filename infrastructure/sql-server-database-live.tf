resource "azurerm_mssql_database" "live" {
  name           = "live-${var.environment_name}-${random_integer.ri.result}-db"
  server_id      = azurerm_mssql_server.main.id
  collation      = "SQL_Latin1_General_CP1_CI_AS"
  max_size_gb    = 2
  sku_name       = "Basic"
    
  tags = {
    environment = var.environment_name
  }
}
