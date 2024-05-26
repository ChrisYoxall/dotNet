


using Moq;
using Web.Api.Box.Entities;

namespace Web.Api.Box.Tests;

/* Moq allows for either implicit verification:

    - add '.Verifiable()' to the setup
    - after the test use either '.VerifyAll()' on each mock or pass alls mocks to Mock.Verify()

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
        
        var mockSequence = new MockSequence();
        
        boxMock.InSequence(mockSequence).Setup(b => b.Open());
        writeLogMock.Setup(wl => wl("The box is opened."));
        boxMock.InSequence(mockSequence).Setup(b => b.PutInside(new Thing(5), "thing1")).Returns(true);
        boxMock.InSequence(mockSequence).Setup(b => b.PutInside(new Thing(3), "thing2")).Returns(true);
        boxMock.InSequence(mockSequence).Setup(b => b.Close());
        writeLogMock.Setup(wl => wl("The box is closed."));
        
        var things = new Dictionary<string, Thing>
        {
            { "thing1", new Thing(5) },
            { "thing2", new Thing(3) }
        };
        
        Operations.FillBox(boxMock.Object, things,writeLogMock.Object);
        
    }
}