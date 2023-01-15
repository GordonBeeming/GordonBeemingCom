# Create a CSR and generate a CA certificate
resource "tls_private_key" "preview" {
  algorithm = "RSA"
}

resource "tls_cert_request" "preview" {
  private_key_pem = tls_private_key.preview.private_key_pem

  subject {
    common_name  = var.preview_hostname
    organization = "Gordon Beeming"
  }
}

resource "cloudflare_origin_ca_certificate" "preview" {
  csr                = tls_cert_request.preview.cert_request_pem
  hostnames          = [var.preview_hostname]
  request_type       = "origin-rsa"
  requested_validity = 5475
  provider = cloudflare.cacert
}

resource "cloudflare_record" "preview" {
  zone_id         = var.cloudflare_zone_id
  name            = var.preview_hostname
  value           = data.dns_a_record_set.preview_app_ip_address.addrs[0]
  type            = "A"
  ttl             = 1
  proxied         = true
  allow_overwrite = false
  provider = cloudflare.default
}

resource "cloudflare_record" "preview_txt_verify" {
  zone_id         = var.cloudflare_zone_id
  name            = "asuid.${var.preview_hostname}"
  value           = azurerm_linux_web_app.preview.custom_domain_verification_id
  type            = "TXT"
  ttl             = 1
  proxied         = false
  allow_overwrite = false
  provider = cloudflare.default
}

resource "random_uuid" "preview_pfx_pass" {
}

resource "pkcs12_from_pem" "preview_domain_pfx" {
  password        = random_uuid.preview_pfx_pass.result
  cert_pem        = cloudflare_origin_ca_certificate.preview.certificate
  private_key_pem = tls_private_key.preview.private_key_pem
}

variable "preview_dns_record" {
  type = string
}

variable "preview_hostname" {
  type = string
}
