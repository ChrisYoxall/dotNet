
using System.Net;
using CosmosDb.Client;
using Microsoft.Azure.Cosmos;


/* These notes were created while following the Learn Path:
 
    - Designing and Implementing Cloud-Native Applications Using Microsoft Azure Cosmos DB
    - https://learn.microsoft.com/en-us/training/courses/dp-420t00?source=learn#course-syllabus
    
Best practices at https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/best-practice-dotnet


 
Each instance of the CosmosClient class has a few features that are already implemented on your behalf:
   
       - Instances are already thread-safe
       - Instances efficiently manage connections
       - Instances cache addresses when operating in direct mode
   
Because of this behavior, each time you destroy and recreate and instance within a single .NET AppDomain, the new
instances lose the benefits of the caching and connection management.


 COULDN'T DO SOME OF THESE IN THE LEARN PATH AS THE LAB WAS NOT AVAILABLE


The Azure Cosmos DB for NoSQL SDK team recommends that you use a single instance per AppDomain for the lifetime of
the  application. This small change to your setup allows for better SDK client-side performance and more efficient
connection management.

There is an emulator for development: https://learn.microsoft.com/en-us/azure/cosmos-db/emulator


Use built-in iterators instead of LINQ methods. For example ToFeedIterator<T> 


*/

var endpoint = "https://free-tier-cosmosdb.documents.azure.com:443/";
var key =  Environment.GetEnvironmentVariable("COSMOS_KEY"); // export COSMOS_KEY=xxxxx

// Create logical client-side representation of the Azure Cosmos DB for NoSQL account.
// The SDK won't initially connect to the account until you perform an operation.

// Options allows you to configure the client. Some default assumptions that the SDK makes can be overridden, for
// example the default consistency level, connecting to the first writable (primary) region, and retry options.
CosmosClientOptions options = new();
CosmosClient client = new (endpoint, key, options);

var account = await client.ReadAccountAsync();

Console.WriteLine($"Account ID: {account.Id}");
    

Database database = client.GetDatabase("sql-db");

Container container = database.GetContainer("sql-container");

Console.WriteLine($"Container: {container.Id}");

Product saddle = new()
{
    id = "027D0B9A-F9D9-4C96-8213-C8546C4AAE71",
    categoryId = "26C74104-40BC-4541-8EF5-9892F7F03D72",
    name = "LL Road Seat/Saddle",
    price = 27.12d,
    tags = new string[] 
    {
        "brown",
        "weathered"
    }
};

try
{
    ItemResponse<Product> response = await container.CreateItemAsync<Product>(saddle); // will fail if item already exists
    HttpStatusCode status = response.StatusCode;
    double requestUnits = response.RequestCharge;

    Product item = response.Resource;
}
catch(CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
{
    // Add logic to handle conflicting ids
}
catch(CosmosException ex) 
{
    // Add general exception handling logic
}

