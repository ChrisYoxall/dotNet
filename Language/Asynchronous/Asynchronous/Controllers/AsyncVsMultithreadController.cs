using ILogger = Serilog.ILogger;
using Microsoft.AspNetCore.Mvc;

namespace Asynchronous.Controllers;

/* Example from: https://code-maze.com/csharp-async-vs-multithreading/
   
   When performing blocking operations between the methods, we should use asynchronous programming. In scenarios where
   we have a fixed thread pool or when we need vertical scalability in the application, we use asynchronous programming.
   
   When we have independent functions performing independent tasks, we should use multithreading. One good example
   of this will be downloading multiple files from multiple tabs in a browser. Each download will run in its own thread.*/

[ApiController]
[Route("[controller]")]
public class AsyncVsMultithreadController :ControllerBase
{
    private readonly ILogger _logger;

    public AsyncVsMultithreadController(ILogger logger)
    {
        _logger = logger.ForContext<AsyncCodeController>();
    }
    
        
    [HttpGet("ExecuteAsyncFunctions")]
    public async Task<IActionResult> ExecuteAsyncFunctions()
    {
        /* These operations will start on the same thread but continue on different threads.
           
           So, why is this happening? Because once the thread hits the awaiting operation in FirstAsync, the thread is 
           freed from that method and returned to the thread pool. Once the operation is completed and the method has
           to continue, a new thread is assigned to it from a thread pool. The same process is repeated for the
           SecondAsync and ThirdAsync as well. */
        
        var firstAsync = FirstAsync();
        var secondAsync = SecondAsync();
        var thirdAsync = ThirdAsync();
        await Task.WhenAll(firstAsync, secondAsync, thirdAsync);
        
        return Ok();
    }
    
    [HttpGet("ExecuteMultithreading")]
    public IActionResult ExecuteMultithreading()
    {
        /* We can clearly see the execution of multithreaded methods on different threads as expected. But also, they
         are keeping the same threads for the continuation compared to the async methods.
        
            From this example, we can see the main difference – Multithreading is a programming technique for executing 
            operations running on multiple threads (also called workers) where we use different threads and block them
            until the job is done. Asynchronous programming is the concurrent execution of multiple tasks (here the
            assigned thread is returned back to a thread pool once the await keyword is reached in the method). */
        
        var t1 = new Thread(new ThreadStart(FirstMethod));
        var t2 = new Thread(new ThreadStart(SecondMethod));
        var t3 = new Thread(new ThreadStart(ThirdMethod));
        
        t1.Start();
        t2.Start();
        t3.Start();
        
        return Ok();
    }
    
    
    async Task FirstAsync()
    {
        _logger.Information("First Async Method on Thread with Id: " + Environment.CurrentManagedThreadId);
        await Task.Delay(1000);
        _logger.Information("First Async Method Continuation on Thread with Id: " + Environment.CurrentManagedThreadId);
    }

    async Task SecondAsync()
    {
        _logger.Information("Second Async Method on Thread with Id: " + Environment.CurrentManagedThreadId);
        await Task.Delay(1000);
        _logger.Information("Second Async Method Continuation on Thread with Id: " + Environment.CurrentManagedThreadId);
    }

    async Task ThirdAsync()
    {
        _logger.Information("Third Async Method on Thread with Id: " + Environment.CurrentManagedThreadId);
        await Task.Delay(1000);
        _logger.Information("Third Async Method Continuation on Thread with Id: " + Environment.CurrentManagedThreadId);
    }
    
    void FirstMethod()
    {
        _logger.Information("First Method on Thread with Id: " + Environment.CurrentManagedThreadId);
        Thread.Sleep(1000);
        _logger.Information("First Method Continuation on Thread with Id: " + Environment.CurrentManagedThreadId);
    }
    void SecondMethod()
    {
        _logger.Information("Second Method on Thread with Id: " + Environment.CurrentManagedThreadId);
        Thread.Sleep(1000);
        _logger.Information("Second Method Continuation on Thread with Id: " + Environment.CurrentManagedThreadId);
    }
    void ThirdMethod()
    {
        _logger.Information("Third Method on Thread with Id: " + Environment.CurrentManagedThreadId);
        Thread.Sleep(1000);
        _logger.Information("Third Method Continuation on Thread with Id: " + Environment.CurrentManagedThreadId);
    }
    
}