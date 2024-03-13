using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace BE_W07Pizza.Models
{
    public partial class ModelDBContext : DbContext
    {
        public ModelDBContext()
            : base("name=ModelDBContext")
        {
        }

        public virtual DbSet<Articoli> Articoli { get; set; }
        public virtual DbSet<DettagliOrdine> DettagliOrdine { get; set; }
        public virtual DbSet<Ordini> Ordini { get; set; }
        public virtual DbSet<Utenti> Utenti { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Articoli>()
                .Property(e => e.Prezzo)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Articoli>()
                .HasMany(e => e.DettagliOrdine)
                .WithRequired(e => e.Articoli)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Ordini>()
                .HasMany(e => e.DettagliOrdine)
                .WithRequired(e => e.Ordini)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.Ordini)
                .WithRequired(e => e.Utenti)
                .WillCascadeOnDelete(false);
        }
    }
}
