

Going through [this MS Learn pathway](https://learn.microsoft.com/en-us/training/paths/deploy-cloud-native-applications-to-azure-container-apps/)

and maybe the table at bottom of https://learn.microsoft.com/en-us/azure/container-apps/start-serverless-containers


Do: https://learn.microsoft.com/en-us/azure/container-apps/managed-identity-image-pull?tabs=bash&pivots=portal

This tutorial looks comprehensive: https://minkovski-d.medium.com/hands-on-azure-container-apps-101-deploying-a-scalable-go-backend-8048b2c155f6




# ACA Demo #

Shows how to get a container running on ACA

## Terraform ##



## Build the Image ##

Building the image using Rider will result in an image 'aca.demo:dev' which will need to be renamed to match the Azure
Container Registry:

- docker build -t yoxalldemoaca.azurecr.io/aca-demo:0.1.0 .

To run it (and bind the app to 8081 inside the container):

- docker run --name aca-demo --rm -p 8080:8081 -e ASPNETCORE_HTTP_PORTS=8081 yoxalldemoaca.azurecr.io/aca-demo:0.1.0

Should now be able to see the forecast at http://localhost:8080/weatherforecast

By default, since ASPNETCORE_ENVIRONMENT is not set inside the container, the app will not bind map the OpenAPI endpoint
(as specified in program.cs). A request to http://localhost:8080/openapi/v1.json will result in a 404 error. If you
pass '-e ASPNETCORE_ENVIRONMENT=Development' as another environment variable to the docker run command above you will
then be able to view the OpenApi document.

Login to the ACR:

- az acr login --name yoxalldemoaca

Now push the image to the ACR:

- docker push yoxalldemoaca.azurecr.io/aca-demo:0.1.0

## Azure Container Apps ##

Container Apps run in the context of a 'Container App Environment': https://learn.microsoft.com/en-us/azure/container-apps/environment

Container Apps Environments determine the functionality and billing. Can run either consumption or dedicated profiles. See https://learn.microsoft.com/en-us/azure/container-apps/structure

A Container App Environment must run in a VNet, but by default this is an automatically generated Azure one. See https://learn.microsoft.com/en-us/azure/container-apps/networking



## Health Checks ##

Easy enough to create a health check endpoint, but they are built into .NET https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks
which means it's easier to leverage that functionality.

Good video at https://www.youtube.com/watch?v=p2faw9DCSsY

That video recommends https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks which seem to be commonly used. The code at least provides
examples of how to write health checks even if you don't use the packages themselves.

