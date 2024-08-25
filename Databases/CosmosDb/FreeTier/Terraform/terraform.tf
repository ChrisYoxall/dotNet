terraform {

  required_version = ">=1.9.5"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.0.1"
    }
  }
}

provider "azurerm" {
  features {}
  subscription_id = "e3768f6b-3a0d-4016-9a03-1b027121d30e"
}