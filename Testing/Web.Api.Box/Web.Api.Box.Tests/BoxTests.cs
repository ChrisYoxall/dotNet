using AutoFixture;
using AutoFixture.Xunit2;
using Xunit.Abstractions;

namespace Web.Api.Box.Tests;

using Entities;

/* Typical way to describe test setup is to use the Arrange, Act, Assert pattern.
 
AutoFixture can be used to make the arrangement phase of a test more concise. It can create instances of classes
with random values, or you can customise the set-up so they are not totally random. */

public class BoxTests
{
    // Used to write messages to the test output
    private readonly ITestOutputHelper _testOutputHelper;

    public BoxTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }


    [Theory, AutoData]
    public void CanCreateBox(int boxSize)
    {
        _testOutputHelper.WriteLine($"AutoFixture.Xunit2 was used to specify a size of {boxSize}");
        
        var b = new Box(boxSize);
        
        Assert.Equal(boxSize, b.Size);
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
        
        box.Close();
        
        Assert.Equal(box.GetAvailableSpace(), boxSize - thingsSize);
    }
}
