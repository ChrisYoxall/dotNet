namespace Web.Api.Box.Entities;


public sealed record Thing
{
    
    public int Size { get; }

    public Thing(int Size)
    {
        if (Size <= 0)
        {
            throw new ArgumentException("Value must be positive", nameof(Size));
        }
        
        this.Size = Size;
    }
}