
data "github_repository" "main" {
  full_name = "${data.github_user.current.username}/GordonBeemingCom"
}

resource "azuread_application_federated_identity_credential" "preview" {
  application_object_id = azuread_application.main.object_id
  display_name          = "gh-${var.environment_name}-${random_integer.ri.result}-${var.appname}-preview"
  description           = "Deployments for ${var.appname}"
  audiences             = ["api://AzureADTokenExchange"]
  issuer                = "https://token.actions.githubusercontent.com"
  subject               = "repo:${data.github_user.current.username}/${data.github_repository.main.name}:environment:${github_repository_environment.preview.environment}"
}
