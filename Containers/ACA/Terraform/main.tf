/* Create a container app running in default VNET */

terraform {

  required_version = ">=1.11.0"

  required_providers {

    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>4.26.0"
    }

  }
}

provider "azurerm" {
  subscription_id = "e3768f6b-3a0d-4016-9a03-1b027121d30e"
  features {}
}

resource "azurerm_resource_group" "aca-demo" {
  name     = "aca-demo-rg"
  location = "Australia East"
}

resource "azurerm_user_assigned_identity" "identity" {
  name                = "aca-demo-identity"
  location            = azurerm_resource_group.aca-demo.location
  resource_group_name = azurerm_resource_group.aca-demo.name
}

resource "azurerm_container_registry" "acr" {
  name                = "yoxalldemoaca"
  resource_group_name = azurerm_resource_group.aca-demo.name
  location            = azurerm_resource_group.aca-demo.location
  sku                 = "Standard"
  admin_enabled       = false
}

resource "azurerm_role_assignment" "ra" {
  scope                = azurerm_container_registry.acr.id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_user_assigned_identity.identity.principal_id
}

resource "azurerm_log_analytics_workspace" "la" {
  name                = "aca-demo-la"
  location            = azurerm_resource_group.aca-demo.location
  resource_group_name = azurerm_resource_group.aca-demo.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_container_app_environment" "cae" {
  name                       = "demo-cae"
  location                   = azurerm_resource_group.aca-demo.location
  resource_group_name        = azurerm_resource_group.aca-demo.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.la.id

  workload_profile {
    name                  = "Consumption"
    workload_profile_type = "Consumption"
  }
}

resource "azurerm_container_app" "ca" {
  name                         = "demo-ca"
  container_app_environment_id = azurerm_container_app_environment.cae.id
  resource_group_name          = azurerm_resource_group.aca-demo.name
  revision_mode                = "Single"

  identity {
    type = "UserAssigned"
    identity_ids = [
      azurerm_user_assigned_identity.identity.id
    ]
  }

  registry {
    server   = azurerm_container_registry.acr.login_server
    identity = azurerm_user_assigned_identity.identity.id
  }

  ingress {
    external_enabled           = true
    allow_insecure_connections = false
    target_port                = 80
    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
  }

  template {
    container {
      name   = "demo-app"
      image  = "docker.io/nginx:1.27.4-alpine"
      cpu    = 0.25
      memory = "0.5Gi"
    }
  }
}

