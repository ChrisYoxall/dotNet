// Top-level statements allows you to write executable code directly at the root of a file, eliminating the need for
// wrapping your code in a class or method. Refer https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/program-structure/top-level-statements

// ASP.NET Core 6.0 introduced minimal APIs. Refer https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api
// This project isn't a minimal API project as it uses controllers.


// DEV CERT CURRENTLY NOT TRUSTED - SEE LINUX SECTION OF https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl


var builder = WebApplication.CreateBuilder(args);

// ADD SERVICES TO THE CONTAINER

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// See Enable HTTP logging below
//builder.Services.AddHttpLogging(_ => { });

// Add console logger https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Enable HTTP logging https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-logging
// This needs a setting in appsettings.json to work.
// Very verbose so turned off currently
//builder.Services.AddHttpLogging(o => { });

var app = builder.Build();

// Part of HTTP logging
//app.UseHttpLogging();


// CONFIGURE THE HTTP REQUEST PIPELINE

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

#pragma warning disable ASP0014
app.UseEndpoints(
    endpoints => endpoints.MapControllers() // Maps attributes on the controllers, like, [Route], [HttpGet], etc.
);
#pragma warning restore ASP0014

// Writes all routes to the debug output
// var actionProvider = app.Services.GetRequiredService<IActionDescriptorCollectionProvider>();
// var routes = actionProvider.ActionDescriptors.Items
//     .Where(x => x.AttributeRouteInfo != null);
//
// foreach(var route in routes)
// {
//     Debug.WriteLine($"{route.AttributeRouteInfo?.Template}");
// }


app.Run();