using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Webima.Models;

namespace Webima.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Bilhete> Bilhetes { get; set; }
        public virtual DbSet<Categoria> Categoria { get; set; }
        public virtual DbSet<CliCat> CliCats { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<Compra> Compras { get; set; }
        public virtual DbSet<Filme> Filmes { get; set; }
        public virtual DbSet<Funcionario> Funcionarios { get; set; }
        public virtual DbSet<Sala> Salas { get; set; }
        public virtual DbSet<Sessao> Sessoes { get; set; }
        public virtual DbSet<Utilizador> Utilizadores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CliCat>()
                .HasKey(u => new { u.IdCliente, u.IdCat });
            modelBuilder.Entity<Compra>()
                .HasKey(u => new { u.IdCliente, u.IdBil, u.DataCompra });
        }
    }
}
