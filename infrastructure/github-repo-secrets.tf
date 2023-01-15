
# ARM_TENANT_ID
resource "github_actions_secret" "ARM_TENANT_ID" {
  repository       = data.github_repository.main.id
  secret_name      = "ARM_TENANT_ID"
  plaintext_value  = data.azurerm_client_config.current.tenant_id
}

# ARM_SUBSCRIPTION_ID
resource "github_actions_secret" "ARM_SUBSCRIPTION_ID" {
  repository       = data.github_repository.main.id
  secret_name      = "ARM_SUBSCRIPTION_ID"
  plaintext_value  = data.azurerm_client_config.current.subscription_id
}
