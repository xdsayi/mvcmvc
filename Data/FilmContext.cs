using Microsoft.EntityFrameworkCore;

namespace mvcmvc.Data
{
    public class FilmContext : DbContext
    {
        public FilmContext(DbContextOptions<FilmContext> options) : base(options) { }

        public DbSet<Film> Films { get; set; } = null!;
    }
}