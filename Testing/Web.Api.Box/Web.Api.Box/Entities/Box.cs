namespace Web.Api.Box.Entities;

/* A Box has size, it can be opened, closed and things with unique labels can be put into if
 there is available space. */

public interface IBox
{
    int Size { get; }
    void Open();
    void Close();
    bool PutInside(Thing thing, string label);
}
public sealed record Box : IBox
{
    private readonly Dictionary<string, Thing> _thingsInside;
  
    public int Size { get; }
  
    public bool IsOpen { get; private set; }
        
    public Box(int Size)
    {
        this.Size = Size;
        _thingsInside = new Dictionary<string, Thing>();
    }

    public void Open()
    {
        IsOpen = true;
    }

    public void Close()
    {
        IsOpen = false;
    }

    public int GetAvailableSpace()
    {
        return Size - _thingsInside.Values.Sum(thing => thing.Size);
    }

    public bool CanPutInside(Thing thing, string label)
    {
        return IsOpen 
               && (GetAvailableSpace() - thing.Size) >= 0 
               && !_thingsInside.ContainsKey(label);
    }

    public bool PutInside(Thing thing, string label)
    {
        if (!CanPutInside(thing, label))
        {
            return false;
        }

        _thingsInside.Add(label, thing);

        return true;
    }
    

}