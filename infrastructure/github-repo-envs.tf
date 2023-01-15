
resource "github_repository_environment" "preview" {
  environment  = "preview"
  repository   = data.github_repository.main.id
}

resource "github_repository_environment" "live" {
  environment  = "live"
  repository   = data.github_repository.main.id
  reviewers {
    users = [ data.github_user.current.id ]
  }  
}
