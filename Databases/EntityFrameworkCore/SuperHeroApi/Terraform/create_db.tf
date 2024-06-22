
terraform {

  required_version = ">=1.8.5"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.109.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~>3.6.2"
    }
    http = {
      source  = "hashicorp/http"
      version = "3.4.3"
    }
  }

}

provider "azurerm" {
  features {}
}

output "admin_password" {
  value     = random_password.admin_password.result
  sensitive = true
}



resource "azurerm_resource_group" "rg" {
  name     = "sql-demo-rg"
  location = "Australia East"
}

resource "random_password" "admin_password" {
  length           = 20
  special          = true
  override_special = "#!()[]*"
  min_numeric      = 1
  min_upper        = 1
  min_lower        = 1
  min_special      = 1
}


// az sql db list-editions --location australiaeast --output table

resource "azurerm_mssql_server" "server" {
  name                          = "yoxallmssqlserver"
  resource_group_name           = azurerm_resource_group.rg.name
  location                      = azurerm_resource_group.rg.location
  administrator_login           = "chrisyoxall"
  administrator_login_password  = random_password.admin_password.result
  version                       = "12.0"
  public_network_access_enabled = true

  azuread_administrator {
    login_username = "chris.yoxall_gmail.com#EXT#@chrisyoxallgmail.onmicrosoft.com"
    object_id      = "777cc4ac-cf3b-4a16-8524-634499462b83"
  }

  # lifecycle {
  #   prevent_destroy = true
  # }
}

data "http" "icanhazip" {
  url = "https://api.ipify.org"
}

resource "azurerm_mssql_firewall_rule" "example" {
  name             = "allow-my-ip"
  server_id        = azurerm_mssql_server.server.id
  start_ip_address = data.http.icanhazip.response_body
  end_ip_address   = data.http.icanhazip.response_body
}

resource "azurerm_mssql_database" "db" {
  name                        = "superheroes"
  server_id                   = azurerm_mssql_server.server.id
  min_capacity                = 1
  auto_pause_delay_in_minutes = "60"
  sku_name                    = "GP_S_Gen5_2"
  max_size_gb                 = "1"
  storage_account_type        = "Zone"
  collation                   = "SQL_Latin1_General_CP1_CI_AS"

  # lifecycle {
  #   prevent_destroy = true
  # }
}


output "public_ip" {
  value = chomp(data.http.icanhazip.response_body)
}

