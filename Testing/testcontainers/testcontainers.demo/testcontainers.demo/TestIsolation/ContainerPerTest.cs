using Dapper;
using JasperFx.Core;
using Marten;
using Npgsql;
using Testcontainers.PostgreSql;
using Weasel.Core;
using Xunit.Abstractions;

namespace testcontainers.demo.TestIsolation;

/* Note that Marten (https://martendb.io/) and Dapper (https://github.com/DapperLib/Dapper) are used in this
 example, which meant the Marten & Dapper Nuget packages have been added.
 
 Also added the Testcontainers.PostgreSql Nuget package, which is a Testcontainers.NET implementation for PostgreSQL. 
 
 Npgsql (https://www.npgsql.org) appears from a quick search to be the most commonly used ADO.NET Data Provider
 for PostgreSQL. I'm assuming it was installed to this project as part of Marten. 
 
 This file creates and runs isolated containers for each test, ensuring no other process can mutate the dependency. The
 main drawback here is it may be slow as the number of tests increases.  */

public class DatabaseContainerPerTest(ITestOutputHelper output) : IAsyncLifetime
{
    // This is called for each test, since each test instantiates a new class instance
    private readonly PostgreSqlContainer _container = 
        new PostgreSqlBuilder()
        .Build();

    private string _connectionString = string.Empty;

    [Fact]
    public async Task Database_Can_Run_Query()
    {
        await using NpgsqlConnection connection = new(_connectionString);
        await connection.OpenAsync();

        const int expected = 1;
        var actual = await connection.QueryFirstAsync<int>("SELECT 1");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Database_Can_Select_DateTime()
    {
        await using NpgsqlConnection connection = new(_connectionString);
        await connection.OpenAsync();

        var actual = await connection.QueryFirstAsync<DateTime>("SELECT NOW()");
        Assert.IsType<DateTime>(actual);
    }

    [Fact]
    public async Task Can_Store_Document_With_Marten()
    {
        await using NpgsqlConnection connection = new(_connectionString);
        var store = DocumentStore.For(options => {
            options.Connection(_connectionString);
            options.AutoCreateSchemaObjects = AutoCreate.All;
        });

        int id;
        {
            await using var session = store.IdentitySession();
            var person = new Person("Khalid");
            session.Store(person);
            await session.SaveChangesAsync();

            id = person.Id;
        }

        {
            await using var session = store.QuerySession();
            var person = session.Query<Person>().FindFirst(p => p.Id == id);
            Assert.NotNull(person);
        }
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        _connectionString = _container.GetConnectionString();
        output.WriteLine(_container.Id);
    }

    public Task DisposeAsync() => _container.StopAsync();
}