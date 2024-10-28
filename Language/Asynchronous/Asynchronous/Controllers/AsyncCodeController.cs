using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;

/* A thread is a low-level tool for creating concurrency. It's limitations include:
 
    - Although it’s easy to start a thread and pass data into it, getting a return value back is not straightforward.
        
    - Cant get a thread to start something else when complete. Instead, you must Join it (blocking your own thread in
            the process).
            
    - Threads are expensive to create and destroy. They consume a lot of memory and CPU cycles. The thread pool
            helps with this, but comes with its own complexities.
        
Tasks are a higher-level abstraction that solve these problems. They represent a concurrent operation that might or
might not be backed by a thread. They are lightweight, easy to create, and can be chained together to create complex
workflows. They are built on top of threads, but are not threads themselves. 

Task and Task<TResult> types both represent an operation that may not have completed yet. The 'async' contextual keyword
is used to define an asynchronous method that can use the 'await' to pause the execution of the method until the awaited
task completes.

The Task types were introduced in Framework 4.0 as part of the parallel programming library. However, they have since
been enhanced (through the use of awaiters) to play equally well in more general concurrency scenarios and are backing
types for C#’s asynchronous methods.

When execution reaches the await expression, you have two possibilities:
        
    - If the awaited task is already completed, the method continues executing synchronously. If the
        operation failed, and it captured an exception to represent that failure, the exception is thrown. Otherwise,
        any result from the operation is obtained
        
    - If the awaited task is not yet completed, the method waits asynchronously for the operation to complete and then
        continues in an appropriate context. This asynchronous waiting really means the method isn’t executing at all.
        A continuation is attached to the asynchronous operation, and the method returns. The async infrastructure makes
        sure that the continuation executes on the right thread: typically, either a thread-pool thread (where it
        doesn't matter which thread is used) or the UI thread where that makes sense. This depends on the synchronization
        context and can also be controlled using Task.ConfigureAwait
                
The idiomatic way of representing failures in .NET is via exceptions. When you await an asynchronous operation that’s
failed, it may have failed a long time ago on a completely different thread. The regular synchronous way of propagating
exceptions up the stack doesn’t occur naturally. Instead, the async/await infrastructure takes steps to make the experience
of handling asynchronous failures as similar as possible to synchronous failures.

If you think of failure as another kind of result, it makes sense that exceptions and return values are handled similarly.
Task and Task<TResult> indicate failures in multiple ways:

    - The Status of a task becomes Faulted when the asynchronous operation has failed (and IsFaulted returns true).
    - The Exception and Result properties return an AggregateException that contains all the (potentially multiple)
        exceptions that caused the task to fail
    - The Wait() method throws an AggregateException if the task ends up in a faulted state.
   
When you await a task, if it’s either faulted or canceled, an exception will be thrown but not the AggregateException.
Instead, for convenience (in most cases), the first exception within the AggregateException is thrown.   
   
The awaitable pattern is used to determine types that can be used with the await operator. The type must have a GetAwaiter
method which returns an object having an IsComplete and GetResult() method and implementing the INotifyCompletion interface.
   
*/

namespace Asynchronous.Controllers;

[ApiController]
[Route("[controller]")]
public class AsyncCodeController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AsyncCodeController> _logger;

    public AsyncCodeController(HttpClient httpClient, ILogger<AsyncCodeController> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    // Can call an asynchronous method from a synchronous method, but can't await it. As the async method
    // returns once the await keyword is encountered, this method will end up returning before the async method
    // completes (probably) and will print details of the task that is still running.
    [HttpGet("NoAsyncOnCallingMethod")]
    public IActionResult NoAsyncOnCallingMethod()
    {
        _logger.LogInformation($"Status of task is: {GetPageLengthAsync().Status}");
        
        return Ok();
    }
    
    [HttpGet("AsyncOnCallingMethod")]
    public async Task<IActionResult> AsyncOnCallingMethod()
    {
        var pageLengthViaAsync = GetPageLengthAsync();
        
        _logger.LogInformation($"The 'GetPageLengthAsync' method has returned. The task status is: {pageLengthViaAsync.Status}");
        
        _logger.LogInformation($"Length using blocking method is : {GetPageLengthBlocking()}");
        
        _logger.LogInformation($"Length using async method is : {await pageLengthViaAsync}");
        
        return Ok();
    }
    
    // Highlight that the calling method will return to here and continue executing when first await is encountered.
    [HttpGet("CallMethodWithTwoAsyncOperations")]
    public async Task<IActionResult> CallMethodWithTwoAsyncOperations()
    {
        var task = MethodWithTwoAsyncMethods();
        
        _logger.LogInformation("Will be here as soon as first await in calling method is hit.");

        await task;
        
        return Ok();
    }
    
    [HttpGet("HandleAggregateException")]
    public IActionResult HandleAggregateException()
    {
        try
        {
            // Start multiple tasks that will throw exceptions
            Task[] tasks =
            [
                Task.Run(() => ProcessOrder(1)),
                Task.Run(() => ProcessOrder(2)),
                Task.Run(() => ProcessOrder(3))
            ];

            // If you do 'await Task.WhenAll(tasks)', the await keyword will cause the AggregateException to be
            // unwrapped, and the individual exceptions will be thrown. This is by design as it makes handling
            // exceptions in asynchronous code very similar to handling them in synchronous code. To ensure the
            // AggregateException is thrown do this...
            Task.WaitAll(tasks);
        }
        catch (AggregateException ae)
        {
            Console.WriteLine("Aggregate Exception Caught:");
            foreach (var innerException in ae.InnerExceptions)
            {
                Console.WriteLine($"- {innerException.Message}");
            }
        }
        
        return Ok();
    }
    
    // This method demonstrates how to handle different exception types in an AggregateException. It's not that 
    // different from the previous example, but just shows the use of Flatten() & Handle() methods.
    [HttpGet("HandleAggregateExceptionWithFlatten")]
    public IActionResult HandleAggregateExceptionWithFlatten()
    {
        try
        {
            // Start multiple tasks that will throw exceptions
            Task[] tasks =
            [
                Task.Run(() => ProcessOrder(1)),
                Task.Run(() => ProcessOrder(2)),
                Task.Run(() => ProcessOrder(3))
            ];
            
            Task.WaitAll(tasks);
        }
        catch (AggregateException ae)
        {
            ae.Flatten().Handle(ex =>
            {
                switch (ex)
                {
                    case InvalidOperationException ioe:
                        Console.WriteLine($"Handled Invalid Operation: {ioe.Message}");
                        return true;
                    case ArgumentException arg:
                        Console.WriteLine($"Handled Argument Exception: {arg.Message}");
                        return true;
                    case TimeoutException te:
                        Console.WriteLine($"Handled Timeout: {te.Message}");
                        return true;
                    default:
                        // Return false for unhandled exception types
                        return false;
                }
            });
        }
        
        return Ok();
    }


    private string GetPageLengthBlocking()
    {
        Task<string> responseTask = _httpClient.GetStringAsync("https://www.example.com");
        
        // Using Result or Wait will block the current thread until the task is completed
        string response = responseTask.Result;
        
        return response.Length.ToString();
    }
    
    // Rules for async methods:
    // - Must have the async keyword in the method signature to use await. The whole point it to use await.
    // - When using async the return type must void, Task or Task<T>. Void for compatibility with event handlers.
    // - Key thing here is that this method returns as soon as the await keyword is encountered. A continuation
    //      is scheduled to run when the task completes to continue executing the steps after the await keyword.
    // - Parameters can't be ref or out. This makes sense as not all the async method may have run when control
    //      returns to the caller, meaning the parameter may be set in the caller after it is used.
    private async Task<string> GetPageLengthAsync()
    {
        Task<string> responseTask = _httpClient.GetStringAsync("https://www.example.com");
        
        string response = await responseTask;
        
        return response.Length.ToString();
    }
    
    private async Task MethodWithTwoAsyncMethods()
    {
        await _httpClient.GetStringAsync("https://www.example.com");
        _logger.LogInformation("First async method has completed");
        
        await _httpClient.GetStringAsync("https://www.google.com");
        _logger.LogInformation("Second async method has completed");
    }

    private void ProcessOrder(int orderId)
    {
        switch (orderId)
        {
            case 1:
                throw new InvalidOperationException($"Order {orderId} is invalid");
            case 2:
                throw new ArgumentException($"Order {orderId} has invalid arguments");
            case 3:
                throw new TimeoutException($"Order {orderId} processing timed out");
            default:
                _logger.LogInformation($"Order {orderId} processed successfully");
                break;
        }
    }
    
}