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

    [HttpGet("RollDice", Name = "RollDice")]
    public async Task<ActionResult<string>> RollDice([FromQuery] string? player)
    {
        Activity.Current?.SetTag(TraceTags.ControllerMethod, nameof(RollDice));
        
        player ??= "Anonymous player";
        
        var result = await RollDice();
        
        _logger.LogInformation("{player} is rolling the dice: {result}", player, result);

        return  Ok(result.ToString(CultureInfo.InvariantCulture));
    }
    
    [HttpGet("RollDice2", Name = "RollDice2")]
    public async Task<ActionResult<string>> RollDice2([FromQuery] string? player)
    {
        Activity.Current?.SetTag(TraceTags.ControllerMethod, nameof(RollDice2));
        
        player ??= "Anonymous player";
        
        // Call an external API to get a random number. Will create a span as AddHttpClientInstrumentation has
        // been added to the OpenTelemetry configuration.
        var response = await _httpClient.GetStringAsync("http://www.randomnumberapi.com/api/v1.0/random?min=1&max=6&count=1");
        var result = JsonSerializer.Deserialize<int[]>(response)?.FirstOrDefault() ?? 0;
        
        _logger.LogInformation("{player} is rolling the dice: {result}", player, result);

        return  Ok(result.ToString(CultureInfo.InvariantCulture));
    }

    private async Task<int> RollDice()
    {
        // It is recommended to create activities only when doing operations that are worth measuring independently.
        // Too many activities makes it harder to visualize.
        using (_activitySource.StartActivity("Rolling the dice"))
        {
            await Task.Delay(200);

            return Random.Shared.Next(1, 7);
        }
    }
}