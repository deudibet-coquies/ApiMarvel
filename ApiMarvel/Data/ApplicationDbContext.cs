using ApiMarvel.Modelos;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiMarvel.Data
{

   // public class ApplicationDbContext : DbContext esto si no uso el identity
    public class ApplicationDbContext : IdentityDbContext<AppUsuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }


        public DbSet<Favorito> Favoritos { get; set; }

        public DbSet<AppUsuario> AppUsuario { get; set; }
    }
}
