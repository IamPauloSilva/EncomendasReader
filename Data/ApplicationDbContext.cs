using Encomendas.Models;
using EncomendasProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EncomendasProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ClientModel> Clients { get; set; }
        public DbSet<EncomendaModel> Encomendas { get; set; }
        public DbSet<WorkerModel> Workers { get; set; }
        public DbSet<ProductsModel> Products { get; set; }
        public DbSet<EncomendaProduct> EncomendaProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração do relacionamento entre EncomendaProduct e ProductsModel
            modelBuilder.Entity<EncomendaProduct>()
                .HasOne(ep => ep.Product)
                .WithMany()
                .HasForeignKey(ep => ep.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EncomendaProduct>()
                .HasOne(ep => ep.Encomenda)
                .WithMany(e => e.EncomendaProducts)
                .HasForeignKey(ep => ep.EncomendaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EncomendaProduct>()
            .HasKey(ep => new { ep.EncomendaId, ep.ProductId });

            modelBuilder.Entity<EncomendaProduct>()
                .HasOne(ep => ep.Encomenda)
                .WithMany(e => e.EncomendaProducts)
                .HasForeignKey(ep => ep.EncomendaId);

            modelBuilder.Entity<EncomendaProduct>()
                .HasOne(ep => ep.Product)
                .WithMany()
                .HasForeignKey(ep => ep.ProductId);
        }
    }

}
