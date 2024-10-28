namespace Asynchronous;

interface IWebInfo
{
    Task<int> GetWebsiteLength();
}

public class WebInfo : IWebInfo
{
     public Task<int> GetWebsiteLength()
     {
         return Task.FromResult(5);
     }
}

public class WebInfoAsync : IWebInfo
{
    public async Task<int> GetWebsiteLength()
    {
        return await Task.FromResult(5);
    }
}