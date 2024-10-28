using Moq;
using Web.Api.Box.Entities;

namespace Web.Api.Box.Tests;

/* Moq allows for either implicit verification:

    - add '.Verifiable()' to the setup
    - after the test use '.VerifyAll()'

or explicit verification:

    - after the test call '.verify()' on the mock to check individual invocations.
    - this is more verbose but allows for more detailed checks.

Good blog post at https://docs.educationsmediagroup.com/unit-testing-csharp/moq/verifications  */

public class OperationsTests
{
    [Fact]
    public void PutThingsInsideBox()
    {
        var writeLogMock = new Mock<WriteLog>(MockBehavior.Strict);
        var boxMock = new Mock<IBox>(MockBehavior.Strict);
        
        boxMock.Setup(b => b.Open()).Verifiable(Times.Once);
        boxMock.Setup(b => b.PutInside(new Thing(5), "thing1")).Returns(true).Verifiable(Times.Once);
        boxMock.Setup(b => b.PutInside(new Thing(3), "thing2")).Returns(true).Verifiable(Times.Once);
        boxMock.Setup(b => b.Close()).Verifiable(Times.Once);
        
        writeLogMock.Setup(wl => wl("The box is opened.")).Verifiable(Times.Once);
        writeLogMock.Setup(wl => wl("The box is closed.")).Verifiable(Times.Once);
        
        var things = new Dictionary<string, Thing>
        {
            { "thing1", new Thing(5) },
            { "thing2", new Thing(3) }
        };
        
        Operations.FillBox(boxMock.Object, things, writeLogMock.Object);
        
        // Checks that above mocks were called as expected
        boxMock.VerifyAll();
        writeLogMock.VerifyAll();
    }
}