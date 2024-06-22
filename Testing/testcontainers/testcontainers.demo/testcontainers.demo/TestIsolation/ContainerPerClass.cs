using Dapper;
using JasperFx.Core;
using Marten;
using Npgsql;
using Weasel.Core;
using Xunit.Abstractions;

namespace testcontainers.demo.TestIsolation;

/* This is variant of the ContainerPerClass example. The tests themselves are the same.
 
 xUnit has a generic interface called IClassFixture, which allows us to define dependencies across all tests within
 a test class.
 
 The obvious drawbacks to this strategy are that tests may inadvertently interfere with other tests assumptions. However,
 some techniques might help get the most out of this approach.
   
 Grouping “read-based” tests in these classes will net you the most performance while keeping confidence levels high.
 Additionally, you can use the constructor and IDisposable interface to clean up and reset the state within a shared
 container, but this may limit parallelization. Finally, you can take an app-based approach to isolation by creating
 tenants using database schemas. All these techniques will vary based on your particular use case.  */

public class DatabaseContainerPerTestClass(DatabaseFixture fixture, ITestOutputHelper output) 
    : IClassFixture<DatabaseFixture>, IDisposable
{
    [Fact]
    public async Task Database_Can_Run_Query()
    {
        await using NpgsqlConnection connection = new(fixture.ConnectionString);
        await connection.OpenAsync();

        const int expected = 1;
        var actual = await connection.QueryFirstAsync<int>("SELECT 1");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Database_Can_Select_DateTime()
    {
        await using NpgsqlConnection connection = new(fixture.ConnectionString);
        await connection.OpenAsync();

        var actual = await connection.QueryFirstAsync<DateTime>("SELECT NOW()");
        Assert.IsType<DateTime>(actual);
    }

    [Fact]
    public async Task Can_Store_Document_With_Marten()
    {
        await using NpgsqlConnection connection = new(fixture.ConnectionString);
        var store = DocumentStore.For(options => {
            options.Connection(fixture.ConnectionString);
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
            var person = session.Query<Person>().FindFirst(p => p.Id  == id);
            Assert.NotNull(person);
        }
    }

    public void Dispose()
        => output.WriteLine(fixture.ContainerId);
}

