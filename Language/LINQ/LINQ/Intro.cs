namespace LINQ;

// Patrick God - LINQ tutorial: https://www.youtube.com/watch?v=Kf9YiRkj-m4

public class Intro
{
    private static readonly List<Game> Games =
    [
        new Game { Title = "The Legend of Zelda", Genre = "Adventure", ReleaseYear = 1986, Rating = 9.5, Price = 60 },
        new Game { Title = "Super Mario Bros.", Genre = "Platformer", ReleaseYear = 1985, Rating = 9.2, Price = 50 },
        new Game { Title = "Elden Ring", Genre = "RPG", ReleaseYear = 2022, Rating = 9.8, Price = 70 },
        new Game { Title = "Fallout 4", Genre = "Open World", ReleaseYear = 2015, Rating = 8.7, Price = 35 },
        new Game { Title = "Tetris", Genre = "Puzzle", ReleaseYear = 1984, Rating = 8.9, Price = 10 }
    ];
    
    
    [Fact]
    public void TestDataExists()
    {
        Assert.Equal(5, Games.Count);
    }

    [Fact]
    public void GetGameTitles()
    {
        var titles = Games.Select(g => g.Title);
        
        Assert.Equal(["The Legend of Zelda", "Super Mario Bros.", "Elden Ring", "Fallout 4", "Tetris"], titles);
    }
    
    [Fact]
    public void GetOpenWorldGames()
    {
        var openWorldGames = Games.Where(g => g.Genre == "Open World");
        
        Assert.Single(openWorldGames);
    }
    
    [Fact]
    public void GetGamesReleasedAfter()
    {
        var shouldBeTrue = Games.Any(g => g.ReleaseYear > 2019);
        var shouldBeFalse = Games.Any(g => g.ReleaseYear > 2023);
        
        Assert.True(shouldBeTrue);
        Assert.False(shouldBeFalse);
    }
    
    [Fact]
    public void SortByYear()
    {
        var ascending = Games.OrderBy(g => g.ReleaseYear);
        var descending = Games.OrderByDescending(g => g.ReleaseYear);
        
        Assert.Equal(1984, ascending.First().ReleaseYear);
        Assert.Equal(2022, descending.First().ReleaseYear);
    }

    [Fact]
    public void FindAveragePrice()
    {
        var averagePrice = Games.Average(g => g.Price);
        
        var totalPrice = Games.Sum(g => g.Price);
        
        Assert.Equal((double)totalPrice/Games.Count, averagePrice);
    }
    
    [Fact]
    public void GetHighestAndLowestRating()
    {
        var highestRating = Games.Max(g => g.Rating);
        var lowestRating = Games.Min(g => g.Rating);
        
        Assert.Equal(9.8, highestRating);
        Assert.Equal(8.7, lowestRating);
    }
    
    [Fact]
    public void GroupByGenre()
    {
        // Debug and look at this. Get the games split into groups by genre.
        var groupedByGenre = Games.GroupBy(g => g.Genre).ToList();
        
        // Get the first game grouped under the RPG genre.
        var game = groupedByGenre.First(kvp => kvp.Key == "RPG");
        Assert.Equal("Elden Ring", game.First().Title);
        Assert.Equal("RPG", game.Key);
        Assert.Single(game);
    }

    [Fact]
    public void GetCheapestAdventureGameFromBefore2000()
    {
        var cheapestAdventureGame = Games
            .Where(g => g.Genre == "Adventure" && g.ReleaseYear < 2000)
            .OrderBy(g => g.Price)
            .Select(g => $"Game: {g.Title} - Price: {g.Price}")
            .First();
        
        Assert.Equal("Game: The Legend of Zelda - Price: 60", cheapestAdventureGame);
    }

    [Fact]
    public void Pagination()
    {
        var pageTwo = Games.Skip(2).Take(2);
        var pageThree = Games.Skip(4).Take(2);
        
        Assert.Equal(2, pageTwo.Count());
        Assert.Single(pageThree);
    }

    [Fact]
    public void GetDistinctValues()
    {
        var listWithDuplicates = new List<string> { "RPG", "Adventure", "RPG", "Platformer", "Adventure" };
        var distinctGenres = listWithDuplicates.Distinct();
        
        Assert.Equal(["RPG", "Adventure", "Platformer"], distinctGenres);
    }

    [Fact]
    public void GenerateObjects()
    {
        var range = Enumerable.Range(1, 50)
            .Select(i => new Game
            {
                Title = $"Game {i}",
                Genre = "Test",
                ReleaseYear = 2000 + i,
                Rating = i,
                Price = i * 10
            })
            .ToList();
    
        Assert.Equal(50, range.Count);
        Assert.Equal("Game 1", range.First().Title);
        Assert.Equal(2050, range.Last().ReleaseYear);
        Assert.Equal(500, range.Last().Price);
    }
}