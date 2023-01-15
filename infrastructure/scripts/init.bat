cd ..
terraform init -reconfigure -var-file="envs/prod.tfvars" -backend-config="envs/prod.tfbackend" %*
cd scripts