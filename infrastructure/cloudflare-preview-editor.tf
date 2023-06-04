# Create a CSR and generate a CA certificate
resource "tls_private_key" "editor_preview" {
  algorithm = "RSA"
}

resource "tls_cert_request" "editor_preview" {
  private_key_pem = tls_private_key.editor_preview.private_key_pem

  subject {
    common_name  = var.editor_preview_hostname
    organization = "Gordon Beeming"
  }
}

resource "cloudflare_origin_ca_certificate" "editor_preview" {
  csr                = tls_cert_request.editor_preview.cert_request_pem
  hostnames          = [var.editor_preview_hostname]
  request_type       = "origin-rsa"
  requested_validity = 5475
  provider = cloudflare.cacert
}

resource "cloudflare_record" "editor_preview" {
  zone_id         = var.cloudflare_zone_id
  name            = var.editor_preview_hostname
  value           = data.dns_a_record_set.editor_preview_app_ip_address.addrs[0]
  type            = "A"
  ttl             = 1
  proxied         = true
  allow_overwrite = false
  provider = cloudflare.default
}

resource "cloudflare_record" "editor_preview_txt_verify" {
  zone_id         = var.cloudflare_zone_id
  name            = "asuid.${var.editor_preview_hostname}"
  value           = azurerm_linux_web_app.editor_preview.custom_domain_verification_id
  type            = "TXT"
  ttl             = 1
  proxied         = false
  allow_overwrite = false
  provider = cloudflare.default
}

resource "random_uuid" "editor_preview_pfx_pass" {
}

resource "pkcs12_from_pem" "editor_preview_domain_pfx" {
  password        = random_uuid.editor_preview_pfx_pass.result
  cert_pem        = cloudflare_origin_ca_certificate.editor_preview.certificate
  private_key_pem = tls_private_key.editor_preview.private_key_pem
}

variable "editor_preview_dns_record" {
  type = string
}

variable "editor_preview_hostname" {
  type = string
}
