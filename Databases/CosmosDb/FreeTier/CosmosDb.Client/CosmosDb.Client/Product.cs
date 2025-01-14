namespace CosmosDb.Client;

public class Product
{
    public string id { get; set; }
    public string name { get; set; }
    public string categoryId { get; set; }
    public double price { get; set; }
    public string[] tags { get; set; }
}