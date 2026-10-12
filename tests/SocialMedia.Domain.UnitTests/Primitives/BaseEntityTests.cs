using SocialMedia.Domain.Primitives;

namespace SocialMedia.Domain.UnitTests.Primitives;

public class BaseEntityTests
{
    [Fact]
    public void Equals_ShouldBeTrue_WhenTypeAndIdMatch()
    {
        var id = Guid.NewGuid();
        var left = new FirstEntity(id);
        var right = new FirstEntity(id);

        Assert.True(left.Equals(right));
        Assert.True(left == right);
        Assert.False(left != right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldBeFalse_WhenIdsDiffer()
    {
        var left = new FirstEntity(Guid.NewGuid());
        var right = new FirstEntity(Guid.NewGuid());

        Assert.False(left.Equals(right));
        Assert.True(left != right);
    }

    [Fact]
    public void Equals_ShouldBeFalse_WhenTypesDiffer()
    {
        var id = Guid.NewGuid();
        BaseEntity left = new FirstEntity(id);
        BaseEntity right = new SecondEntity(id);

        Assert.False(left.Equals(right));
        Assert.True(left != right);
    }

    [Fact]
    public void Equals_ShouldBeFalse_WhenOtherIsNull()
    {
        var entity = new FirstEntity(Guid.NewGuid());

        Assert.False(entity.Equals(null));
        Assert.False(entity == null);
    }

    private sealed class FirstEntity(Guid id) : BaseEntity(id);

    private sealed class SecondEntity(Guid id) : BaseEntity(id);
}
