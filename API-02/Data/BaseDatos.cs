using Contratos.Models;
using Microsoft.EntityFrameworkCore;
using WebApiWeb.Models;

namespace Contratos.Data
{
    public class BaseDatos : DbContext
    {
        public BaseDatos(DbContextOptions<BaseDatos> options) : base(options)
        {
        }

        public DbSet<Jefe> Jefes { get; set; }

        public DbSet<Proyecto> Proyectos { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Validaciones a nivel base de datos
            modelBuilder.Entity<Jefe>()
                .HasIndex(j => j.Nombre)
                .IsUnique();

            modelBuilder.Entity<Proyecto>()
                .HasIndex(p => p.Nombre)
                .IsUnique();

            // Un jefe solo puede estar a cargo de un proyecto
            modelBuilder.Entity<Proyecto>()
                .HasIndex(p => p.IdJefe)
                .IsUnique();

            modelBuilder.Entity<Proyecto>()
                .HasOne(p => p.Jefe)
                .WithMany()
                .HasForeignKey(p => p.IdJefe)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
