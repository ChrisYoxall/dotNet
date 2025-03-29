

Going through [this MS Learn pathway](https://learn.microsoft.com/en-us/training/paths/deploy-cloud-native-applications-to-azure-container-apps/)

and maybe the table at bottom of https://learn.microsoft.com/en-us/azure/container-apps/start-serverless-containers


Do: https://learn.microsoft.com/en-us/azure/container-apps/managed-identity-image-pull?tabs=bash&pivots=portal

This tutorial looks comprehensive: https://minkovski-d.medium.com/hands-on-azure-container-apps-101-deploying-a-scalable-go-backend-8048b2c155f6




# ACA Demo #

Shows how to get a container running on ACA


## Azure Container Apps ##

Container Apps run in the context of a 'Container App Environment': https://learn.microsoft.com/en-us/azure/container-apps/environment

Container Apps Environments determine the functionality and billing. Can run either consumption or dedicated profiles. See https://learn.microsoft.com/en-us/azure/container-apps/structure

A Container App Environment must run in a VNet, but by default this is an automatically generated Azure one. See https://learn.microsoft.com/en-us/azure/container-apps/networking



## Terraform ##

Note how the Terraform is configured to create a container application running Nginx. This allows the Terraform to be applied
to create all the resources. If we wanted to configure the Terraform to run our application it would need to be in multiple
parts as first the container registry would need to be created so we can build and push an image, then the container application
could be configured to read that image.

Note that for a live system it's likely that the infrastructure is maintained with Terraform and new image versions
would be deployed using other mechanisms (we will use an Azure CLI command). However, you do need to specify an image to use
in the Terraform so an easy work-around is to use a publicly available image. The downside is that you do also need
some ignore blocks in the Terraform.


## Build and push image ##

Build by doing (note that 'yoxalldemoaca' is then name of the container registry specified in the Terraform):

- docker build -t yoxalldemoaca.azurecr.io/aca-demo:0.1.0 .

To run it (and bind the app to 8081 inside the container):

- docker run --name aca-demo --rm -p 8080:8081 -e ASPNETCORE_HTTP_PORTS=8081 yoxalldemoaca.azurecr.io/aca-demo:0.1.0

Without specifying ASPNETCORE_HTTP_PORTS 8080 will be used.

Should now be able to see the forecast at http://localhost:8080/weatherforecast

By default, since ASPNETCORE_ENVIRONMENT is not set inside the container, the app will not bind map the OpenAPI endpoint
(as specified in program.cs). A request to http://localhost:8080/openapi/v1.json will result in a 404 error. If you
pass '-e ASPNETCORE_ENVIRONMENT=Development' as another environment variable to the docker run command above you will
then be able to view the OpenApi document.

Login to the ACR:

- az acr login --name yoxalldemoaca

Now push the image to the ACR:

- docker push yoxalldemoaca.azurecr.io/aca-demo:0.1.0


## Health Checks ##

Easy enough to create a health check endpoint from scratch, but they are built into .NET https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks
which means it's a more standardised approach to leverage that functionality.

Good video at https://www.youtube.com/watch?v=p2faw9DCSsY

That video recommends https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks which seem to be commonly used. The code at least provides
examples of how to write health checks even if you don't use the packages themselves.

