cd ..
terraform plan -input=false -var-file="envs/prod.tfvars" -out="envs/prod.tfplan" %*
cd scripts