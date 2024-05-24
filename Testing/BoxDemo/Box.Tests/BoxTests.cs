
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoFixture;
using Xunit.Abstractions;

namespace Box.Tests;

using Entities;

/* Typical way to describe test setup is to use the Arrange, Act, Assert pattern.
 
AutoFixture can be used to make the arrangement phase of a test more concise. It can create instances of classes
with random values, or you can customise the set-up so they are not totally random. */

public class BoxTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public BoxTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void CanPutThingThatFitsIntoOpenBox()
    {
        var box = new Box(10);
        var thing = new Thing(5);
        
        box.Open();
        
        Assert.True(box.PutInside(thing, "thing"));
    }
    
    [Fact]
    public void CantPutThingThatFitsIntoClosedBox()
    {
        var box = new Box(10);
        var thing = new Thing(5);
        
        Assert.False(box.IsOpen);
        Assert.False(box.PutInside(thing, "thing"));
    }
    
    [Fact]
    public void UseAutoFixtureToCreateThings()
    {
        var fixture = new Fixture();
        var random = new Random();

        const int boxSize = 100;

        var things = fixture.Build<Thing>()
            .FromFactory(() =>new Thing(random.Next(2, 10)))
            .CreateMany<Thing>(10);

        var box = new Box(boxSize);
        box.Open();

        var thingsSize = 0;
        foreach (var thing in things)
        {
            box.PutInside(thing, thingsSize.ToString());
            thingsSize += thing.Size;
        }
        
        Assert.Equal(box.GetAvailableSpace(), boxSize - thingsSize);
    }

    [Fact]
    public void Scratch()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };
        var sum = list.Sum();
        _testOutputHelper.WriteLine(sum.ToString());
        
        
        var p = new Post(1, 1, "My Title", "My Body");
        
    }

    [Fact]
    public void Dictionaries()
    {
        var sentence = "The big bad wolf and the big bad sheep didn't have much in common with each other.";
        
        var words = sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var wordCount = new Dictionary<string, int>();
        // Count the number of times each word appears in the sentence
        foreach (var word in words)
        {
            var lower = word.ToLower();
            if (!wordCount.TryAdd(lower, 1))
            {
                wordCount[lower]++;
            }
        }
        
        foreach (var (word, count) in wordCount)
        {
            _testOutputHelper.WriteLine($"{word}: {count}");
        }

        var stuff = new Dictionary<int, string>()
        {
            { 1, "One" },
            { 2, "Two" },
            { 3, "Three" }
        };
        
        foreach (var (index, num) in stuff)
        {
            _testOutputHelper.WriteLine($"{index}: {num}");
        }
    }

    [Fact]
    public void Linq()
    {
        string[] fruits = { "banana", "mango", "orange", "lemon", "grape", "apple" };

        var query =
            fruits.Select((fruit, index) =>
                new { index, str = fruit.Substring(0, index) });

        foreach (var obj in query)
            _testOutputHelper.WriteLine("{0}", obj);

        // sort the fruits alphabetically
        var sortedFruits = fruits.OrderBy(fruit => fruit);
        foreach (var fruit in sortedFruits)
            _testOutputHelper.WriteLine(fruit);
    }

    [Fact]
    public async void MakeHttpCall()
    {
        using HttpClient client = new();
        //client.DefaultRequestHeaders.Accept.Clear();
        await ProcessPostsAsync(client);
    }

    private async Task ProcessPostsAsync(HttpClient client)
    {
        // Uses https://jsonplaceholder.typicode.com/ which is a free fake and reliable API for testing and prototyping
        
        
        // Make request and output the result as string
        //var json = await client.GetStringAsync("https://jsonplaceholder.typicode.com/todos/1");
        //_testOutputHelper.WriteLine(json);
        
        await using Stream stream = await client.GetStreamAsync("https://jsonplaceholder.typicode.com/posts");
        var posts = await JsonSerializer.DeserializeAsync<List<Post>>(stream);
        
        if (posts != null)
            foreach (var post in posts)
                _testOutputHelper.WriteLine(post.ToString());
    }

    /* Can use either ( ) or { } to define a record:
     
        - The parentheses syntax () is more concise and suitable for simple records with a small number of properties.
        - The curly braces syntax {} provides more flexibility, allowing for additional members, methods, and customization of property access modifiers and initialization.    */
    private record Post(
        [property: JsonPropertyName("userId")] int UserId,
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("body")] string Body);
    
  


}