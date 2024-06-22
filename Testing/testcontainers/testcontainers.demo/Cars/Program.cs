using System.Data.Common;
using Dapper;
using Npgsql;

/* Simple web application that utilizes Dapper and a PostgreSQL to read Car records from a table. Most importantly,
 the application finds the connection string in the configuration instance, which reads from all the default
 locations. This connection string is not configured. */

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<DbConnection>(static sp => {
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("database");
    var connection = new NpgsqlConnection(connectionString);
    connection.Open();
    return connection;
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/database", async (DbConnection db, string? make) =>
{
    if (make is null) 
        return Results.NotFound();

    var car = await db.QueryFirstAsync<Car>(
        "select * from Cars where make = @make",
        new { make }
    );

    return car is not null 
        ? Results.Ok(car) 
        : Results.NotFound();
});

app.Run();


public class Car
{
    public int Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
}

// Make the implicit Program class public so test projects can access it
public partial class Program { }