using Microsoft.EntityFrameworkCore;

namespace SocialMedia.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
}
