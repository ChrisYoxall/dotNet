# Entity Framework Core #

Good information at https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-8.0/whatsnew


## Database Setup ##

There is a Terraform file that can be used to create a MS SQL Server instance in Azure.

Alternatively, create a container based on the official image from the [Microsoft Artifact Registry](https://mcr.microsoft.com/):

```bash
docker pull mcr.microsoft.com/mssql/server:2022-latest
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=1P@ssw0rd!" \
   -p 1433:1433 --name sql01 --hostname sql01 \
   -d --rm\
   mcr.microsoft.com/mssql/server:2022-latest
```
Give it a couple of seconds to start-up then try:

```bash
docker exec -t sql01 cat /var/opt/mssql/log/errorlog | grep connection
```

Shell into the container by doing:

    docker exec -it sql01 /bin/bash

Once inside the container, connect locally with sqlcmd, using its full path.

    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "1P@ssw0rd!"


To connect to the master database using the Rider IDE, use the following connection string:

    Server=localhost,1433;Database=master;User Id=sa;Password=1P@ssw0rd!;TrustServerCertificate=True;


## Entity Framework Core CLI Tools ##

From https://learn.microsoft.com/en-us/ef/core/cli/ it says:

- Adding the Microsoft.EntityFrameworkCore.Tools Nuget package allows migration commands to be run from the Visual Studio Package
Manager Console
- The EF Core CLI tools are installed using the 'dotnet tool install dotnet-ef' command

I prefer the second option as I use Rider (so no Package Manager Console) and also prefer to install tools locally rather than
globally for version management.

To install a tool for local access only (for the current directory and subdirectories), you must add the tool to a tool manifest
file. The manifest file is called 'dotnet-tools.json' and is inside a directory called '.config'. To create a tool manifest file
if it does not already exist do:

    dotnet new tool-manifest

To install the EF Core CLI tools locally (i.e. don't use the --global flag), run:

    dotnet tool install dotnet-ef

View installed tools by doing:

    dotnet tool list

Check the manifest file into version control. In the future to restore the tools can simply do:

    dotnet tool restore

To uninstall the EF Core CLI tools, run:
    
    dotnet tool uninstall dotnet-ef

To run the EF Core CLI tools, use the following command:

    dotnet ef (or dotnet dotnet-ef)

More information on local installation at https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install

## Migrations ##

The above commands can be run from the solution folder in case other projects also need to use the EF Core CLI tools. For
the commands here change to the project file that contains the DbContext.

To create a migration run:

    dotnet ef migrations add Initial

Then to apply it:

    dotnet ef database update

