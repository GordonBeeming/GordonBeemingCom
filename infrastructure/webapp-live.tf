resource "azurerm_linux_web_app" "live" {
  name                = "live-${var.appname}-${var.environment_name}-${random_integer.ri.result}"
  location            = data.azurerm_resource_group.main.location
  resource_group_name = data.azurerm_resource_group.main.name
  service_plan_id     = azurerm_service_plan.appserviceplan.id
  https_only          = true
  site_config {
    minimum_tls_version = "1.2"

    application_stack {
      dotnet_version = "6.0"
    }
  }
  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.app.id]
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT" = "Production"
  }

  connection_string {
    name  = "DefaultConnection"
    type  = "SQLAzure"
    value = "Server=tcp:${azurerm_mssql_server.main.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.live.name};Persist Security Info=false;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Integrated Security=false;User Id=${azurerm_mssql_server.main.administrator_login};Password=${azurerm_mssql_server.main.administrator_login_password};"
  }

  virtual_network_subnet_id = azurerm_subnet.main.id
}

resource "azurerm_app_service_certificate" "live" {
  name                = "live-${var.environment_name}-domain-cert"
  resource_group_name = data.azurerm_resource_group.main.name
  location            = data.azurerm_resource_group.main.location
  pfx_blob            = pkcs12_from_pem.live_domain_pfx.result
  password            = random_uuid.live_pfx_pass.result
}

resource "time_sleep" "live_wait_for_txt" {
  depends_on = [cloudflare_record.live_txt_verify]

  create_duration = "15s"
}

resource "azurerm_app_service_custom_hostname_binding" "live" {
  hostname            = var.live_hostname
  app_service_name    = azurerm_linux_web_app.live.name
  resource_group_name = data.azurerm_resource_group.main.name

  depends_on = [
    time_sleep.live_wait_for_txt
  ]
}

resource "azurerm_app_service_certificate_binding" "live" {
  hostname_binding_id = azurerm_app_service_custom_hostname_binding.live.id
  certificate_id      = azurerm_app_service_certificate.live.id
  ssl_state           = "SniEnabled"
}

data "dns_a_record_set" "live_app_ip_address" {
  host = azurerm_linux_web_app.live.default_hostname
}

output "live-app-ip" {
  value = data.dns_a_record_set.live_app_ip_address.addrs[0]
}

output "live-url" {
  value = "https://${azurerm_linux_web_app.live.default_hostname}"
}

output "live-custom-url" {
  value = "https://${var.live_hostname}"
}
