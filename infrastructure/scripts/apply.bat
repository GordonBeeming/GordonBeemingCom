cd ..
terraform apply -auto-approve %* "envs/prod.tfplan"
cd scripts