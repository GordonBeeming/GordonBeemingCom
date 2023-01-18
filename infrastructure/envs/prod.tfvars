deploy_region = "westeurope"
appname = "gordonbeemingcom"
resource_group_name = "gordonbeemingcom-prod-rg"
environment_name = "prod"

# cloudflare_api_token = local env variable
# cloudflare_service_key = local env variable
# cloudflare_zone_id = local env variable
# github_token = local env variable
preview_hostname = "preview.gordonbeeming.com"
preview_dns_record = "preview"
live_hostname = "gordonbeeming.com"
live_dns_record = "@"

preview_container_image = "ghcr.io/gordonbeeming/gordonbeemingcom"
preview_container_image_tag = "main"
live_container_image = "ghcr.io/gordonbeeming/gordonbeemingcom"
live_container_image_tag = "main"