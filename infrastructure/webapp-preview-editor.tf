resource "azurerm_linux_web_app" "editor_preview" {
  name                = "editor-preview-${var.appname}-${var.environment_name}-${random_integer.ri.result}"
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
      docker_image     = var.editor_preview_container_image
      docker_image_tag = var.editor_preview_container_image_tag
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
    "ProductionInstance":"false"
    "ASPNETCORE_URLS":"http://+:80"
    "WebSiteUrl": "https://${var.preview_hostname}"
  }

  connection_string {
    name  = "DefaultConnection"
    type  = "SQLAzure"
    value = "Server=tcp:${azurerm_mssql_server.main.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.preview.name};Persist Security Info=false;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Integrated Security=false;User Id=${azurerm_mssql_server.main.administrator_login};Password=${azurerm_mssql_server.main.administrator_login_password};"
  }

  virtual_network_subnet_id = azurerm_subnet.main.id
}

resource "azurerm_app_service_certificate" "editor_preview" {
  name                = "editor-preview-${var.environment_name}-domain-cert"
  resource_group_name = data.azurerm_resource_group.main.name
  location            = data.azurerm_resource_group.main.location
  pfx_blob            = pkcs12_from_pem.editor_preview_domain_pfx.result
  password            = random_uuid.editor_preview_pfx_pass.result
}

resource "time_sleep" "editor_preview_wait_for_txt" {
  depends_on = [cloudflare_record.editor_preview_txt_verify]

  create_duration = "15s"
}

resource "azurerm_app_service_custom_hostname_binding" "editor_preview" {
  hostname            = var.editor_preview_hostname
  app_service_name    = azurerm_linux_web_app.editor_preview.name
  resource_group_name = data.azurerm_resource_group.main.name

  depends_on = [
    time_sleep.editor_preview_wait_for_txt
  ]
}

resource "azurerm_app_service_certificate_binding" "editor_preview" {
  hostname_binding_id = azurerm_app_service_custom_hostname_binding.editor_preview.id
  certificate_id      = azurerm_app_service_certificate.editor_preview.id
  ssl_state           = "SniEnabled"
}

data "dns_a_record_set" "editor_preview_app_ip_address" {
  host = azurerm_linux_web_app.editor_preview.default_hostname
}

variable "editor_preview_container_image" {
  type = string
}

variable "editor_preview_container_image_tag" {
  type = string
}

output "editor-preview-app-ip" {
  value = data.dns_a_record_set.editor_preview_app_ip_address.addrs[0]
}

output "editor-preview-url" {
  value = "https://${azurerm_linux_web_app.editor_preview.default_hostname}"
}

output "editor-preview-custom-url" {
  value = "https://${var.editor_preview_hostname}"
}
