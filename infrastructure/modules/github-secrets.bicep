// Bicep module for managing GitHub secrets and environment settings

resource githubSecrets 'Microsoft.KeyVault/vaults/secrets@2021-06-01-preview' = {
  name: 'githubSecrets'
  properties: {
    value: 'secretValue'
  }
}

output githubSecretsId string = githubSecrets.id
