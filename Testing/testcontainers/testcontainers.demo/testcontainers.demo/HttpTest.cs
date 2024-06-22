using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace testcontainers.demo;

// This is essentially the initial .NET demo from https://dotnet.testcontainers.org

public class HttpTest : IAsyncLifetime
{
    private readonly IContainer _container;
    
    public HttpTest()
    {
        _container = new ContainerBuilder()
            // Set the image for the container to "testcontainers/helloworld:1.1.0".
            .WithImage("testcontainers/helloworld:1.1.0")
            // Bind port 8080 of the container to a random port on the host.
            .WithPortBinding(8080, true)
            // Wait until the HTTP endpoint of the container is available.
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(8080)))
            // Build the container configuration.
            .Build();
    }
    
    [Fact]
    public async Task Can_Call_Uuid_Endpoint()
    {
        var httpClient = new HttpClient();
        var requestUri =
            new UriBuilder(
                Uri.UriSchemeHttp,
                _container.Hostname,
                _container.GetMappedPublicPort(8080),
                "uuid"
            ).Uri;
        var guid = await httpClient.GetStringAsync(requestUri);
        Assert.True(Guid.TryParse(guid, out _));
    }
    
    [Fact]
    public async Task Can_Call_Ping_Endpoint()
    {
        var httpClient = new HttpClient();
        var requestUri =
            new UriBuilder(
                Uri.UriSchemeHttp,
                _container.Hostname,
                _container.GetMappedPublicPort(8080),
                "ping"
            ).Uri;
        var response = await httpClient.GetStringAsync(requestUri);
        Assert.Equal("PONG", response);
    }
    
    public Task InitializeAsync() => _container.StartAsync();
    
    public Task DisposeAsync()  => _container.DisposeAsync().AsTask();
}