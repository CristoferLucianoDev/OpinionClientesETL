using Microsoft.EntityFrameworkCore;
using OpinionClienteDwh.Api.Persistence.Entities;

namespace OpinionClienteDwh.Api.Persistence;

public sealed class OpinionesOltpContext : DbContext
{
    public OpinionesOltpContext(DbContextOptions<OpinionesOltpContext> options) : base(options)
    {
    }

    public DbSet<SocialComment> SocialComments => Set<SocialComment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SocialComment>(entity =>
        {
            entity.ToTable("SocialComments", "dbo");
            entity.HasKey(e => e.IdComment);
        });
    }
}