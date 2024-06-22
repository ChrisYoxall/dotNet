using Dapper;
using JasperFx.Core;
using Marten;
using Npgsql;
using Weasel.Core;
using Xunit.Abstractions;

namespace testcontainers.demo.TestIsolation;

/* This is a variant to the ContainerPerClass example. The tests themselves are the same.
 
 xUnit allows you to create “collections”, a logically-grouped set of tests that share resources. Here we want to
 share our container across our collection. It is just an example of another way to xUnit isolation, with the isolation
 levels available being per test, per class, and per collection.
 
 Tests here are the same as in the previous tests to show the reuse of containers across two test classes. The tests
 are identical to the earlier examples, but now they run for a total of 6 times. Have the same drawbacks as the
 ContainerPerClass example.
 
 */

[CollectionDefinition(nameof(DatabaseCollection))]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>;

public static class ContainerPerCollection
{
    [Collection(nameof(DatabaseCollection))]
    public class First(DatabaseFixture fixture, ITestOutputHelper output) : IDisposable
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
        public void Dispose() => output.WriteLine(fixture.ContainerId);
    }
    
    [Collection(nameof(DatabaseCollection))]
    public class Second(DatabaseFixture fixture, ITestOutputHelper output) : IDisposable
    {
        [Fact]
        public async Task Database_Can_Run_Query()
        {
            await using NpgsqlConnection connection = new(fixture.ConnectionString);
            await connection.OpenAsync();
            output.WriteLine("Hi! 👋");
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
        public void Dispose() => output.WriteLine(fixture.ContainerId);
    }
}