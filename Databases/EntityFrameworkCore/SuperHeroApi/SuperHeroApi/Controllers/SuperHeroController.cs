using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperHeroApi.Data;
using SuperHeroApi.Entities;

// Example from https://www.youtube.com/watch?v=b8fFRX0T38M
// Note as this is a demo it's a fat controller, i.e. all the code here. This is not a recommended practice.

namespace SuperHeroApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        // Would normally have this in a service
        private readonly DataContext _context;
        public SuperHeroController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllHeroes()
        {
            // var heroes = new List<SuperHero>()
            // {
            //     new() { Id = 1, Name = "Superman", FirstName = "Clark", LastName = "Kent", Place = "Metropolis" },
            //     new() { Id = 2, Name = "Spider-Man", FirstName = "Peter", LastName = "Parker", Place = "New York" },
            //     new() { Id = 3, Name = "Batman", FirstName = "Bruce", LastName = "Wayne", Place = "Gotham" },
            //     new() { Id = 4, Name = "Iron Man", FirstName = "Tony", LastName = "Stark", Place = "Malibu" },
            //     new() { Id = 5, Name = "Hulk", FirstName = "Bruce", LastName = "Banner", Place = "Rio de Janeiro" },
            // };
            
            var heroes = await _context.SuperHeroes.ToListAsync();

            return Ok(heroes);
        }
        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SuperHero>> GetHero(int id)
        {
            var hero = await _context.SuperHeroes.FindAsync(id);
            if (hero is null)
                return NotFound("Hero not found.");

            return Ok(hero);
        }
        
        // Should create a DTO here so concerns such as Id from SuperHero are not exposed, but this is a quick demo
        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> AddHero(SuperHero hero)
        {
            _context.SuperHeroes.Add(hero);
            await _context.SaveChangesAsync();

            return Ok(await _context.SuperHeroes.ToListAsync());
        }
        
        [HttpPut]
        public async Task<ActionResult<List<SuperHero>>> UpdateHero(SuperHero updatedHero)
        {
            var dbHero = await _context.SuperHeroes.FindAsync(updatedHero.Id);
            if (dbHero is null)
                return NotFound("Hero not found.");
            
            dbHero.Name = updatedHero.Name;
            dbHero.FirstName = updatedHero.FirstName;
            dbHero.LastName = updatedHero.LastName;
            dbHero.Place = updatedHero.Place;
            
            await _context.SaveChangesAsync();

            return Ok(await _context.SuperHeroes.ToListAsync());
        }
        
        // Should probably add ("{id:int}") to the route to ensure the id is an int as with the GetHero method
        // This shows it's not strictly necessary, but it's often considered a good practice
        [HttpDelete]
        public async Task<ActionResult<List<SuperHero>>> DeleteHero(int id)
        {
            var dbHero = await _context.SuperHeroes.FindAsync(id);
            if (dbHero is null)
                return NotFound("Hero not found.");

            _context.SuperHeroes.Remove(dbHero);
            await _context.SaveChangesAsync();

            return Ok(await _context.SuperHeroes.ToListAsync());
        }

    }
}
