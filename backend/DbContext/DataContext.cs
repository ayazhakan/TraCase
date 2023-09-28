// Models/AppDbContext.cs
using Microsoft.EntityFrameworkCore;

public class DataContext : DbContext
{
  
    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to sqlite database
        options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
    }


    public DbSet<User> Users { get; set; }
    // Diğer DbSet'ler buraya eklenebilir.
}
