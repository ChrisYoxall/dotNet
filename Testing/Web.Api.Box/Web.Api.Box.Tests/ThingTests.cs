using Web.Api.Box.Entities;


namespace Web.Api.Box.Tests;


public class ThingTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void ThingMustHavePositiveSize(int size)
    {
        Assert.Throws<ArgumentException>(() => new Thing(size));
    }
}