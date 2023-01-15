$SharedResourceGroup = "gordonbeemingcom-prod-rg"
$DeployRegion = "westeurope"
$SharedStorage = "gordonbeemingcom"
$SharedStorageContainer = "tfstate"

az group create --resource-group $SharedResourceGroup --location $DeployRegion

az storage account create --name $SharedStorage --resource-group $SharedResourceGroup --location $DeployRegion --access-tier Hot --allow-blob-public-access false --https-only true --kind StorageV2 --sku Standard_ZRS

az storage container create -n $SharedStorageContainer --account-name $SharedStorage --resource-group $SharedResourceGroup
