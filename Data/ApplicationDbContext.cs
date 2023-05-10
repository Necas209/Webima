using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Webima.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Webima.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Bilhete> Bilhetes { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<CategoriaPreferida> CategoriasPreferidas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<Filme> Filmes { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Sala> Salas { get; set; }
        public DbSet<Sessao> Sessoes { get; set; }
        public DbSet<Utilizador> Utilizadores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CategoriaPreferida>()
                .HasKey(u => new { IdCliente = u.ClienteId, IdCat = u.CategoriaId });
            modelBuilder.Entity<Compra>()
                .HasKey(u => new { IdCliente = u.ClienteId, IdBil = u.BilheteId, u.DataCompra });
        }
    }
}