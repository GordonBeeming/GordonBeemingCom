cd ..
terraform init -reconfigure -var-file="envs/prod.tfvars" -backend-config="envs/prod.tfbackend"
terraform apply -destroy -auto-approve -input=false -var-file="envs/prod.tfvars"

rem az group delete --resource-group "gordonbeemingcom-prod-rg" --yes
cd scripts