using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Api.Box.Controllers;
using Xunit.Abstractions;

namespace Web.Api.Box.Tests.OtherExamples.AutoFixture;



public class AutoMock
{
    // Used to write messages to the test output
    private readonly ITestOutputHelper _testOutputHelper;
    
    public AutoMock(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    public void SutIsController()
    {
        // How can unit tests be decoupled from Dependency Injection mechanics? It's common for tests to break when the
        // constructor of a class changes. AutoMock (a combination of AutoFixture & Moq) can be used to create a mock of a
        // class and inject it into the class under test.
        // Imagine this is the very fist test you write for a new controller. Initially you probably wouldn't know
        // you needed a logger so this would just be
        //var sut = new BoxController();
        //Assert.IsAssignableFrom<AutoMock>(sut);
        
        // See next test for a solution
        
        // This shows how you would have to come back later once the logger was added.
        var mockLogger = new Mock<ILogger<BoxController>>();
        var sut = new BoxController(mockLogger.Object);
        Assert.IsAssignableFrom<ControllerBase>(sut);
    }
    
    [Fact]
    public void CrateControllerWithAutoMoq()
    {
        var fixture = new Fixture()
            .Customize(new AutoMoqCustomization { ConfigureMembers = true } );
        
        // Creates the controller with a logger injected. If the test had been written like this initially
        // there is no need to come back once the logger or any other injected dependencies are added.
        var sut = fixture.Build<BoxController>()
            .OmitAutoProperties()
            .Create();
        
        // OmitAutoProperties disables the assignment of values to properties.
        // WithAutoProperties forces the assignment of values to properties, even if it had been disabled on the fixture
        
        Assert.IsAssignableFrom<ControllerBase>(sut);
        
    }
    
    [Fact]
    public void AutoFixtureMethods()
    {
        var fixture = new Fixture();
        
        // Create requested type using current customizations and default configurations
        fixture.Create<Car>();
        
        // Build can be used to add one-time customizations to be used for the creation of the next variable. Once t
        // the object is created, the customizations are lost
        fixture.Build<Car>()
            .With(c => c.Model, "Ford")
            .With(c => c.Make, "Focus")
            .With(c => c.ProductionYear, 2010)
            .Create();
        
        // Build.With & Build.Without can be used to add or remove customizations to be used for the creation of the
        // next variable. Once the object is created, the customizations are lost
        
        // Build.FromFactory can be used to create a new instance of a type each time it is requested. Used
        // to describe exactly how to build complex types.
        
        // Inject will always return the same instance of a type when it is requested
        
        // Freeze will create the entity and return it, and also injects it so that object will always be returned.
    }
    
    // Use the AutoData attribute to generate data. 
    [Theory, AutoData]
    public void CanCreateBox(int boxSize)
    {
        _testOutputHelper.WriteLine($"AutoFixture.Xunit2 was used to specify a size of {boxSize}");
        
        var b = new Entities.Box(boxSize);
        
        Assert.Equal(boxSize, b.Size);
    }

    private class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute() : base(() => 
            new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true } )){}
    }
    
    [Theory]
    [AutoMoqData]
    public void CanSetTestDoubleWithAutoFixtureAutoMoq([Frozen] Mock<IEngine> mockEngine, Car sut)
    {
        sut.Start();

        Assert.NotNull(sut.EngineModel);
        mockEngine.Verify(engine => engine.Start(), Times.Once);
    }
    
    public class Car
    {
        private readonly IEngine _engine;

        public Car(string model, string make, int productionYear, IEngine engine)
        {
            Model = model;
            Make = make;
            ProductionYear = productionYear;
            _engine = engine;
        }


        public string Model { get; }

        public string Make { get; }

        public int ProductionYear { get; }

        public string EngineModel => _engine.Model;


        public void Start() => _engine.Start();
    }

    public interface IEngine
    {
        string Model { get; }

        void Start();
    }
    
}