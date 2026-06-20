using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Persistence;

public class UrlShortenerDbContext : DbContext
{
    public UrlShortenerDbContext(
        DbContextOptions<UrlShortenerDbContext> options)
        : base(options)
    {
    }

    public DbSet<ShortenedUrl> ShortenedUrls => Set<ShortenedUrl>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortenedUrl>(entity =>
        {
            entity.ToTable("shortened_urls");

            entity.HasKey(x => x.Code);

            entity.Property(x => x.Code)
                .HasColumnName("code")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.OriginalUrl)
                .HasColumnName("original_url")
                .HasMaxLength(2048)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();
        });
    }
}