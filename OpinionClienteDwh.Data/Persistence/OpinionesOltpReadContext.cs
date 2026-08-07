using Microsoft.EntityFrameworkCore;
using OpinionClienteDwh.Data.Persistence.Entities;

namespace OpinionClienteDwh.Data.Persistence;

public sealed class OpinionesOltpReadContext : DbContext
{
    public OpinionesOltpReadContext(DbContextOptions<OpinionesOltpReadContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<ProductoCategoria> ProductoCategorias => Set<ProductoCategoria>();
    public DbSet<Categoria> Categorias => Set<Categoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(e =>
        {
            e.ToTable("Cliente", "dbo");
            e.HasKey(c => c.IdCliente);
        });

        modelBuilder.Entity<Producto>(e =>
        {
            e.ToTable("Producto", "dbo");
            e.HasKey(p => p.IdProducto);
        });

        modelBuilder.Entity<ProductoCategoria>(e =>
        {
            e.ToTable("ProductoCategoria", "dbo");
            e.HasKey(pc => new { pc.IdProducto, pc.IdCategoria });
        });

        modelBuilder.Entity<Categoria>(e =>
        {
            e.ToTable("Categoria", "dbo");
            e.HasKey(c => c.IdCategoria);
        });
    }
}