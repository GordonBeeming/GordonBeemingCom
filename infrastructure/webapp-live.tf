resource "azurerm_linux_web_app" "live" {
  name                = "live-${var.appname}-${var.environment_name}-${random_integer.ri.result}"
  location            = data.azurerm_resource_group.main.location
  resource_group_name = data.azurerm_resource_group.main.name
  service_plan_id     = azurerm_service_plan.appserviceplan.id
  https_only          = true
  client_affinity_enabled = true

  site_config {
    minimum_tls_version = "1.2"
    always_on = true
    http2_enabled = true
    
    application_stack {
      docker_image     = var.live_container_image
      docker_image_tag = var.live_container_image_tag
    }
  }
  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.app.id]
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT" = "Production"
    "ApplicationInsights__InstrumentationKey" = azurerm_application_insights.tracking.instrumentation_key
    "BlobStorageUrl" = "https://${azurerm_storage_account.content.name}.blob.core.windows.net/"
    "DOCKER_REGISTRY_SERVER_URL"="https://ghcr.io"
    "AZURE_CLIENT_ID": azurerm_user_assigned_identity.app.client_id
    "ProductionInstance":"true"
    "ASPNETCORE_URLS":"http://+:80"
  }

  connection_string {
    name  = "DefaultConnection"
    type  = "SQLAzure"
    value = "Server=tcp:${azurerm_mssql_server.main.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.live.name};Persist Security Info=false;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Integrated Security=false;User Id=${azurerm_mssql_server.main.administrator_login};Password=${azurerm_mssql_server.main.administrator_login_password};"
  }

  virtual_network_subnet_id = azurerm_subnet.main.id
  
  depends_on = [azurerm_linux_web_app.preview]
}

resource "azurerm_app_service_certificate" "live" {
  name                = "live-${var.environment_name}-domain-cert"
  resource_group_name = data.azurerm_resource_group.main.name
  location            = data.azurerm_resource_group.main.location
  pfx_blob            = pkcs12_from_pem.live_domain_pfx.result
  password            = random_uuid.live_pfx_pass.result
  
  depends_on = [azurerm_app_service_certificate.preview]
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
    time_sleep.live_wait_for_txt, azurerm_app_service_custom_hostname_binding.preview
  ]
}

resource "azurerm_app_service_certificate_binding" "live" {
  hostname_binding_id = azurerm_app_service_custom_hostname_binding.live.id
  certificate_id      = azurerm_app_service_certificate.live.id
  ssl_state           = "SniEnabled"
  
  depends_on = [azurerm_app_service_certificate_binding.preview]
}

data "dns_a_record_set" "live_app_ip_address" {
  host = azurerm_linux_web_app.live.default_hostname
}

variable "live_container_image" {
  type = string
}

variable "live_container_image_tag" {
  type = string
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
