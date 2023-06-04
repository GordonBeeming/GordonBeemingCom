# GordonBeemingCom

This is the source code for profile and blog site [https://gordonbeeming.com](https://gordonbeeming.com). Feel free to use pieces of it that might be useful to you.

## Database Changes

Once new tables are added to the database context, you will need to run the following command to generate the migration files:
    
    dotnet tool restore
    cd src
    dotnet dotnet-ef migrations add {{ NAME OF MIGRATION }} --context AppDbContext --project 'GordonBeemingCom.Database' --startup-project 'GordonBeemingCom'

