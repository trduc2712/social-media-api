using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialMedia.Domain.Users;

namespace SocialMedia.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    private const int ClerkUserIdMaxLength = 255;
    private const int UsernameMaxLength = 64;
    private const int NameMaxLength = 100;
    private const int AvatarUrlMaxLength = 2048;

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", table => table.HasComment(
            "People who use the app. Sign-in is handled by Clerk; each row is the local copy of one Clerk user."));

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasComment("Identifier of the user in this system (UUID v4).");

        builder.Property(user => user.ClerkUserId)
            .HasColumnName("clerk_user_id")
            .HasMaxLength(ClerkUserIdMaxLength)
            .HasComment("Identifier of the user in Clerk, taken from the sub claim of the session token.");

        builder.Property(user => user.Username)
            .HasColumnName("username")
            .HasMaxLength(UsernameMaxLength)
            .HasComment("Username copied from Clerk. Null when the user has none.");

        builder.Property(user => user.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(NameMaxLength)
            .HasComment("First name copied from Clerk. Null when the user has none.");

        builder.Property(user => user.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(NameMaxLength)
            .HasComment("Last name copied from Clerk. Null when the user has none.");

        builder.Property(user => user.AvatarUrl)
            .HasColumnName("avatar_url")
            .HasMaxLength(AvatarUrlMaxLength)
            .HasComment("URL of the profile image copied from Clerk. Null when the user has none.");

        builder.Property(user => user.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasComment("When the row was created, in UTC.");

        builder.Property(user => user.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasComment("When the row was last changed, in UTC. Null when it has never been changed.");

        builder.Property(user => user.IsDeleted)
            .HasColumnName("is_deleted")
            .HasComment("True when the user has been soft-deleted. Deleted users are hidden from queries.");

        builder.Property(user => user.DeletedAtUtc)
            .HasColumnName("deleted_at_utc")
            .HasComment("When the user was soft-deleted, in UTC. Null while the user is not deleted.");

        builder.HasIndex(user => user.ClerkUserId).IsUnique();
    }
}
