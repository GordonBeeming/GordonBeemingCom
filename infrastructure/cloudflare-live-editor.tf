# Create a CSR and generate a CA certificate
resource "tls_private_key" "editor_live" {
  algorithm = "RSA"
  
  depends_on = [tls_private_key.editor_preview]
}

resource "tls_cert_request" "editor_live" {
  private_key_pem = tls_private_key.editor_live.private_key_pem

  subject {
    common_name  = var.editor_live_hostname
    organization = "Gordon Beeming"
  }
  
  depends_on = [tls_cert_request.editor_preview]
}

resource "cloudflare_origin_ca_certificate" "editor_live" {
  csr                = tls_cert_request.editor_live.cert_request_pem
  hostnames          = [var.editor_live_hostname]
  request_type       = "origin-rsa"
  requested_validity = 5475
  provider = cloudflare.cacert
  
  depends_on = [cloudflare_origin_ca_certificate.editor_preview]
}

resource "cloudflare_record" "editor_live" {
  zone_id         = var.cloudflare_zone_id
  name            = var.editor_live_hostname
  value           = data.dns_a_record_set.editor_live_app_ip_address.addrs[0]
  type            = "A"
  ttl             = 1
  proxied         = true
  allow_overwrite = false
  provider = cloudflare.default
  
  depends_on = [cloudflare_record.editor_preview]
}

resource "cloudflare_record" "editor_live_txt_verify" {
  zone_id         = var.cloudflare_zone_id
  name            = "asuid.${var.editor_live_hostname}"
  value           = azurerm_linux_web_app.editor_live.custom_domain_verification_id
  type            = "TXT"
  ttl             = 1
  proxied         = false
  allow_overwrite = false
  provider = cloudflare.default
  
  depends_on = [cloudflare_record.editor_preview_txt_verify]
}

resource "random_uuid" "editor_live_pfx_pass" {
}

resource "pkcs12_from_pem" "editor_live_domain_pfx" {
  password        = random_uuid.editor_live_pfx_pass.result
  cert_pem        = cloudflare_origin_ca_certificate.editor_live.certificate
  private_key_pem = tls_private_key.editor_live.private_key_pem
  
  depends_on = [pkcs12_from_pem.editor_preview_domain_pfx]
}

variable "editor_live_dns_record" {
  type = string
}

variable "editor_live_hostname" {
  type = string
}
