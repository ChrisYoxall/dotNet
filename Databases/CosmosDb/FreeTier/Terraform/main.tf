resource "azurerm_resource_group" "rg" {
  name     = "cosmos-db-free-rg"
  location = "australiaeast"
}

# Free tier: https://learn.microsoft.com/en-us/azure/cosmos-db/free-tier

resource "azurerm_cosmosdb_account" "account" {
  name                       = "free-tier-cosmosdb"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  offer_type                 = "Standard"
  kind                       = "GlobalDocumentDB"
  automatic_failover_enabled = false
  free_tier_enabled          = true
  geo_location {
    location          = azurerm_resource_group.rg.location
    failover_priority = 0
  }
  consistency_policy {
    consistency_level       = "BoundedStaleness"
    max_interval_in_seconds = 300
    max_staleness_prefix    = 100000
  }
  capacity {
    total_throughput_limit = 1000
  }

}

resource "azurerm_cosmosdb_sql_database" "main" {
  name                = "sql-db"
  resource_group_name = azurerm_resource_group.rg.name
  account_name        = azurerm_cosmosdb_account.account.name
  throughput          = 400 # If using multiple DBs, keep total here under 1000 to stay in free tier
}

resource "azurerm_cosmosdb_sql_container" "example" {
  name                  = "sql-container"
  resource_group_name   = azurerm_resource_group.rg.name
  account_name          = azurerm_cosmosdb_account.account.name
  database_name         = azurerm_cosmosdb_sql_database.main.name
  partition_key_paths   = ["/id"]
  partition_key_version = 1
  throughput            = 400 # Need to keep total throughput to 1000 or less to stay in free tier

  indexing_policy {
    indexing_mode = "consistent"

    included_path {
      path = "/*"
    }

    included_path {
      path = "/included/?"
    }

    excluded_path {
      path = "/excluded/?"
    }
  }

  unique_key {
    paths = ["/definition/idlong", "/definition/idshort"]
  }
}
