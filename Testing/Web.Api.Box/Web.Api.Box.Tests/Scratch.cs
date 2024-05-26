using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Abstractions;

namespace Web.Api.Box.Tests;

public class Scratch
{
    
    private readonly ITestOutputHelper _testOutputHelper;

    public Scratch(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void List()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };
        var sum = list.Sum();
        _testOutputHelper.WriteLine(sum.ToString());
        
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
    public void DoSomething()
    {
        var numbers = new List<int>() { 1, 2, 3 };
        var p = new Person("Chris", 51);
    }
    
    
    internal record Person(string Name, int Age)
    {

    }
    

    [Fact]
    public void Linq()
    {
        // Note use of collection expression here, i.e. not using new string[] { ... }
        string[] fruits = ["banana", "mango", "orange", "lemon", "grape", "apple"];

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
        
        await using var stream = await client.GetStreamAsync("https://jsonplaceholder.typicode.com/posts");
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
        [property: JsonPropertyName("body")] string Body
    );

    [Fact]
    public async Task Teaser()
    {
        // What will this code output?
        
        var tasks = Enumerable.Range(0, 2)
            .Select(_ => Task.Run(() => _testOutputHelper.WriteLine("*")));
        
        await Task.WhenAll(tasks);
        
        _testOutputHelper.WriteLine($"{tasks.Count()} stars!");
        
        // It will end up printing 4 stars, two during 'WhenAll' and two during 'tasks.count()' as both of
        // those commands trigger tasks to be enumerated. When writing this Rider identified multiple
        // enumeration and allowed me to convert the enumerable to a list to avoid this.
    }
      
}