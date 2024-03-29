
# ARM_CLIENT_ID
resource "github_actions_environment_secret" "live-ARM_CLIENT_ID" {
  repository       = data.github_repository.main.id
  environment      = github_repository_environment.live.environment
  secret_name      = "ARM_CLIENT_ID"
  plaintext_value  = azuread_application.main.application_id
}

# AZURE_WEBAPP_NAME
resource "github_actions_environment_secret" "editor-live-AZURE_WEBAPP_NAME" {
  repository       = data.github_repository.main.id
  environment      = github_repository_environment.live.environment
  secret_name      = "EDITOR_AZURE_WEBAPP_NAME"
  plaintext_value  = azurerm_linux_web_app.editor_live.name
}

resource "github_actions_environment_secret" "live-AZURE_WEBAPP_NAME" {
  repository       = data.github_repository.main.id
  environment      = github_repository_environment.live.environment
  secret_name      = "SITE_AZURE_WEBAPP_NAME"
  plaintext_value  = azurerm_linux_web_app.live.name
}

# AZURE_WEBAPP_RG
resource "github_actions_environment_secret" "live-AZURE_WEBAPP_RG" {
  repository       = data.github_repository.main.id
  environment      = github_repository_environment.live.environment
  secret_name      = "AZURE_WEBAPP_RG"
  plaintext_value  = data.azurerm_resource_group.main.name
}
