// Bicep module for Cloudflare settings

resource cloudflareSettings 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'cloudflareSettings'
  location: 'global'
  properties: {
    zoneType: 'Public'
    recordSets: [
      {
        name: '@'
        type: 'A'
        ttl: 3600
        aRecords: [
          {
            ipv4Address: '192.0.2.1'
          }
        ]
      }
      {
        name: 'www'
        type: 'CNAME'
        ttl: 3600
        cnameRecord: {
          cname: 'example.com'
        }
      }
    ]
  }
}
