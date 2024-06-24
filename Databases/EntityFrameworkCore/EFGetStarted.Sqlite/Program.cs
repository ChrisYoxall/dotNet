
/* This example is taken from https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app

From the command to install dotnet-ef and create the database:

    dotnet new tool-manifest
    dotnet tool install dotnet-ef
    dotnet add package Microsoft.EntityFrameworkCore.Design
    dotnet ef migrations add InitialCreate
    dotnet ef database update
    
   
When running on Ubuntu 24.04, the database is created in the following location:
   /home/chris/.local/share/blogging.db 

So it can be opened in sqlite by doing:

    sqlite3 /home/chris/.local/share/blogging.db
   
*/

using EFGetStarted.Sqlite;


using var db = new BloggingContext();

// Note: This sample requires the database to be created before running.
Console.WriteLine($"Database path: {db.DbPath}.");

// Create
Console.WriteLine("Inserting a new blog");
db.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
db.SaveChanges();

// Read
Console.WriteLine("Querying for a blog");
var blog = db.Blogs
    .OrderBy(b => b.BlogId)
    .First();

// Update
Console.WriteLine("Updating the blog and adding a post");
blog.Url = "https://devblogs.microsoft.com/dotnet";
blog.Posts.Add(
    new Post { Title = "Hello World", Content = "I wrote an app using EF Core!" });
db.SaveChanges();

// Delete
Console.WriteLine("Delete the blog");
db.Remove(blog);
db.SaveChanges();

// Delete the database - not sure this is working
//File.Delete(db.DbPath);
