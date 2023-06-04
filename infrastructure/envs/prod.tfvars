deploy_region = "westeurope"
appname = "gordonbeemingcom"
resource_group_name = "gordonbeemingcom-prod-rg"
environment_name = "prod"

# cloudflare_api_token = local env variable
# cloudflare_service_key = local env variable
# cloudflare_zone_id = local env variable
# github_token = local env variable
editor_preview_hostname = "editor.preview.gordonbeeming.com"
editor_preview_dns_record = "editor.preview"
preview_hostname = "preview.gordonbeeming.com"
preview_dns_record = "preview"
editor_live_hostname = "editor.gordonbeeming.com"
editor_live_dns_record = "editor"
live_hostname = "gordonbeeming.com"
live_dns_record = "@"

editor_preview_container_image = "ghcr.io/gordonbeeming/gordonbeemingcomeditor"
editor_preview_container_image_tag = "main"
preview_container_image = "ghcr.io/gordonbeeming/gordonbeemingcom"
preview_container_image_tag = "main"
editor_live_container_image = "ghcr.io/gordonbeeming/gordonbeemingcomeditor"
editor_live_container_image_tag = "main"
live_container_image = "ghcr.io/gordonbeeming/gordonbeemingcom"
live_container_image_tag = "main"