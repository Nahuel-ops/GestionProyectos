using BackendProyectos.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendProyectos.Data
{
    public class BaseDeDatos : DbContext
    {
        public BaseDeDatos(DbContextOptions<BaseDeDatos> options) : base(options)
        {
        }

        public DbSet<Jefe> Jefes { get; set; }
        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Regla 1: Jefe no tiene el mismo nombre
            modelBuilder.Entity<Jefe>()
                .HasIndex(j => j.Nombre)
                .IsUnique();

            // Regla 2: Proyectos no pueden tener mismo nombre
            modelBuilder.Entity<Proyecto>()
                .HasIndex(p => p.Nombre)
                .IsUnique();

            // Regla 3: Jefe solo puede estar a cargo de 1 proyecto (1 a 1)
            modelBuilder.Entity<Proyecto>()
                .HasIndex(p => p.IdJefe)
                .IsUnique();
        }
    }
}
