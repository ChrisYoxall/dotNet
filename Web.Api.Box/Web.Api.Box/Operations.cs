using Web.Api.Box.Entities;

namespace Web.Api.Box;

/* Why use a delegate for log writer and not an interface? Delegates and interfaces are very similar. They
 both define a contract. A delegate can be seen as an interface with only one method and without an interface
 name. So if you have only one method you may prefer to use delegates instead of interfaces. I tend to use
 delegates quite often as such an approach better accomplishes S and I from the SOLID principles. */
public delegate void WriteLog(string message);

public static class Operations
{
    public static Dictionary<string, Thing> FillBox(
        IBox box,
        IDictionary<string, Thing> things,
        WriteLog writeLog)
    {
        var rest = new Dictionary<string, Thing>();
        
        box.Open();
        writeLog("The box is opened.");
        
        foreach (var (label, thing) in things)
        {
            if (!box.PutInside(thing, label))
            {
                rest.Add(label, thing);
            }
        }
        
        box.Close();
        writeLog("The box is closed.");
        
        return rest;
    }
}