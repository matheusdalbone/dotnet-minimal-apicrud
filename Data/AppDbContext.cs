using ApiCrud.Estudantes;
using Microsoft.EntityFrameworkCore;

namespace ApiCrud.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Estudante> Estudantes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost\\sqlexpress;Initial Catalog=MinimalAPI;Integrated Security=True;Encrypt=False");
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        }
    }
}
