using Microsoft.EntityFrameworkCore;

namespace Asgard.Odin.Model.Database;

public class OdinContext : DbContext
{
    public OdinContext(DbContextOptions<OdinContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        
    }
}