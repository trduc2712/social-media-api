using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Primitives;
using SocialMedia.Domain.Users;

namespace SocialMedia.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes().Select(type => type.ClrType).ToList())
        {
            if (typeof(IHasDomainEvents).IsAssignableFrom(entityType))
            {
                modelBuilder.Entity(entityType).Ignore(nameof(IHasDomainEvents.DomainEvents));
            }

            if (typeof(ISoftDeletable).IsAssignableFrom(entityType))
            {
                var entity = Expression.Parameter(entityType, "entity");
                var isNotDeleted = Expression.Not(Expression.Property(entity, nameof(ISoftDeletable.IsDeleted)));

                modelBuilder.Entity(entityType).HasQueryFilter(Expression.Lambda(isNotDeleted, entity));
            }
        }
    }
}
