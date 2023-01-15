# Create a CSR and generate a CA certificate
resource "tls_private_key" "live" {
  algorithm = "RSA"
  
  depends_on = [tls_private_key.preview]
}

resource "tls_cert_request" "live" {
  private_key_pem = tls_private_key.live.private_key_pem

  subject {
    common_name  = var.live_hostname
    organization = "Gordon Beeming"
  }
  
  depends_on = [tls_cert_request.preview]
}

resource "cloudflare_origin_ca_certificate" "live" {
  csr                = tls_cert_request.live.cert_request_pem
  hostnames          = [var.live_hostname]
  request_type       = "origin-rsa"
  requested_validity = 5475
  provider = cloudflare.cacert
  
  depends_on = [cloudflare_origin_ca_certificate.preview]
}

resource "cloudflare_record" "live" {
  zone_id         = var.cloudflare_zone_id
  name            = var.live_hostname
  value           = data.dns_a_record_set.live_app_ip_address.addrs[0]
  type            = "A"
  ttl             = 1
  proxied         = true
  allow_overwrite = false
  provider = cloudflare.default
  
  depends_on = [cloudflare_record.preview]
}

resource "cloudflare_record" "live_txt_verify" {
  zone_id         = var.cloudflare_zone_id
  name            = "asuid.${var.live_hostname}"
  value           = azurerm_linux_web_app.live.custom_domain_verification_id
  type            = "TXT"
  ttl             = 1
  proxied         = false
  allow_overwrite = false
  provider = cloudflare.default
  
  depends_on = [cloudflare_record.preview_txt_verify]
}

resource "random_uuid" "live_pfx_pass" {
}

resource "pkcs12_from_pem" "live_domain_pfx" {
  password        = random_uuid.live_pfx_pass.result
  cert_pem        = cloudflare_origin_ca_certificate.live.certificate
  private_key_pem = tls_private_key.live.private_key_pem
  
  depends_on = [pkcs12_from_pem.preview_domain_pfx]
}

variable "live_dns_record" {
  type = string
}

variable "live_hostname" {
  type = string
}
