using Microsoft.EntityFrameworkCore;
using SuperHeroApi.Entities;

namespace SuperHeroApi.Data;

public class DataContext: DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    
    public DbSet<SuperHero> SuperHeroes { get; set; }
}