using SocialMedia.Domain.Primitives;

namespace SocialMedia.Domain.UnitTests.Primitives;

public class AuditableTests
{
    public static TheoryData<BaseEntity, bool, bool> Entities => new()
    {
        { new SampleAuditableEntity(), false, false },
        { new SampleSoftDeletableEntity(), true, false },
        { new SampleAuditableEventfulEntity(), false, true },
        { new SampleSoftDeletableEventfulEntity(), true, true },
    };

    [Theory]
    [MemberData(nameof(Entities))]
    public void BaseClass_ShouldImplementItsMarkers(BaseEntity entity, bool softDeletable, bool hasDomainEvents)
    {
        Assert.IsAssignableFrom<IAuditable>(entity);
        Assert.Equal(softDeletable, entity is ISoftDeletable);
        Assert.Equal(hasDomainEvents, entity is IHasDomainEvents);
    }

    [Fact]
    public void Auditable_ShouldStartWithUnsetTimestamps()
    {
        IAuditable auditable = new SampleAuditableEntity();

        Assert.Equal(default, auditable.CreatedAtUtc);
        Assert.Null(auditable.UpdatedAtUtc);
    }

    [Fact]
    public void SoftDeletable_ShouldStartNotDeleted()
    {
        ISoftDeletable softDeletable = new SampleSoftDeletableEventfulEntity();

        Assert.False(softDeletable.IsDeleted);
        Assert.Null(softDeletable.DeletedAtUtc);
    }

    private sealed class SampleAuditableEntity() : AuditableEntity(Guid.NewGuid());

    private sealed class SampleSoftDeletableEntity() : SoftDeletableEntity(Guid.NewGuid());

    private sealed class SampleAuditableEventfulEntity() : AuditableEventfulEntity(Guid.NewGuid());

    private sealed class SampleSoftDeletableEventfulEntity() : SoftDeletableEventfulEntity(Guid.NewGuid());
}
