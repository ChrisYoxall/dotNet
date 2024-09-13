using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Honeycomb.Services;
using Microsoft.AspNetCore.Mvc;

namespace Honeycomb.Controllers;

[ApiController]
[Route("[controller]")]
public class DiceController : ControllerBase
{

    private readonly ILogger<DiceController> _logger;
    private readonly HttpClient _httpClient;
    private readonly ActivitySource _activitySource;

    public DiceController(ILogger<DiceController> logger, HttpClient httpClient, Instrumentation instrumentation)
    {
        _logger = logger;
        _httpClient = httpClient;
        _activitySource = instrumentation.ActivitySource;
    }
    
    /* Can add tags which are key-value attributes to the current activity. Tags are indexed and searchable in Honeycomb.
     
     Another way to add information is Span Events which get displayed as little circles on the span in Honeycomb.
     Ensure the 'Metadata: Annotation Type' is set appropriately in the 'Data Settings'. Once you send your first Span
     Events through Honeycomb will likely identity the correct field to use. Each event can also have attribues which
     are key-value pairs.  */

    [HttpGet("RollDice", Name = "RollDice")]
    public async Task<ActionResult<string>> RollDice([FromQuery] string? player)
    {
        Activity.Current?.SetTag(TraceTags.ControllerMethod, nameof(RollDice));
        Activity.Current?.AddEvent(new ActivityEvent("Start"));
        
        player ??= "Anonymous player";
        
        Activity.Current?.AddTag("player.name", player);
        
        Activity.Current?.AddEvent(new ActivityEvent("Rolling"));

        var result = await RollDice();
        
        _logger.LogInformation("{player} has rolled: {result}", player, result);

        Activity.Current?.AddEvent(new ActivityEvent("Complete"));

        return  Ok(result.ToString(CultureInfo.InvariantCulture));
    }
    
    [HttpGet("RollDice2", Name = "RollDice2")]
    public async Task<ActionResult<string>> RollDice2([FromQuery] string? player)
    {
        Activity.Current?.SetTag(TraceTags.ControllerMethod, nameof(RollDice2));
        Activity.Current?.AddEvent(new ActivityEvent("Start"));
        
        player ??= "Anonymous player";
        
        Activity.Current?.AddTag("player.name", player);

        Activity.Current?.AddEvent(new ActivityEvent("Rolling"));

        // Call an external API to get a random number. Will create a span as AddHttpClientInstrumentation has
        // been added to the OpenTelemetry configuration.
        var response = await _httpClient.GetStringAsync("http://www.randomnumberapi.com/api/v1.0/random?min=1&max=6&count=1");
        var result = JsonSerializer.Deserialize<int[]>(response)?.FirstOrDefault() ?? 0;
        
        _logger.LogInformation("{player} has rolled: {result}", player, result);
        
        Activity.Current?.AddEvent(new ActivityEvent("Complete"));

        return  Ok(result.ToString(CultureInfo.InvariantCulture));
    }

    private async Task<int> RollDice()
    {
        // Create new activity to represent the operation of rolling the dice.
        // It is recommended to create activities only when doing operations that are worth measuring independently.
        // Too many activities makes it harder to visualize.
        using (_activitySource.StartActivity("Rolling the dice"))
        {
            await Task.Delay(200);

            return Random.Shared.Next(1, 7);
        }
    }
}