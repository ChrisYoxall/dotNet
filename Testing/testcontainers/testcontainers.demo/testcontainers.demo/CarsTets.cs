
using System.Net.Http.Json;
using Dapper;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using testcontainers.demo.TestIsolation;

namespace testcontainers.demo;

/* Note use of Microsoft.AspNetCore.Mvc.Testing to build our ASP.NET Core application and allow us to inject our
 container as a dependency, specifically, the container’s connection string. We can also migrate our database and
 set the initial state of our database. From there, it’s as straightforward as calling our endpoints. Very nice!
   
 This seems a nice approach to testing ASP.NET Core applications, as the HTTP protocol can be incredibly complex and
 nuanced. Additionally, using Testcontainers, you can be confident that dependencies will behave as intended.
 
 Another example of using Microsoft.AspNetCore.Mvc.Testing with Testcontainers is at https://www.milanjovanovic.tech/blog/testcontainers-integration-testing-using-docker-in-dotnet
 and the Microsoft documentation at https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests is worth
 a read also.
   
*/

public class WebAppWithDatabase(DatabaseFixture fixture) 
    : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    [Fact]
    public async Task Get_Information_From_Database_Endpoint()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(host => {
                // database connection from TestContainers
                host.UseSetting(
                    "ConnectionStrings:database", 
                    fixture.ConnectionString
                );
            });

        var client = factory.CreateClient();
        var actual = await client.GetFromJsonAsync<Car>("/database?make=Honda");

        Assert.Equal(expected: "Civic", actual?.Model);
    }

    public async Task InitializeAsync()
    {
        var connection = new NpgsqlConnection(fixture.ConnectionString);
        // let's migrate a table here and insert values
        //
        // Note: if you're using EF Core, this is where you'd run your database migrations
        // there are also other migration frameworks like FluentMigrator or Flywheel that you might try.
        // For the sake of simplicity, executing SQL here is fine.
        await connection.ExecuteAsync(
            // lang=sql
            """
            DO $$
            BEGIN
                IF NOT EXISTS (SELECT FROM pg_catalog.pg_tables WHERE schemaname = 'public' AND tablename = 'cars') THEN
                    CREATE TABLE Cars (
                        id SERIAL PRIMARY KEY,
                        make VARCHAR(255),
                        model VARCHAR(255),
                        year INT
                    );

                    INSERT INTO Cars (make, model, year) VALUES
                    ('Toyota', 'Corolla', 2020),
                    ('Honda', 'Civic', 2020),
                    ('Ford', 'Focus', 2020);
                END IF;
            END $$;
            """
        );
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
