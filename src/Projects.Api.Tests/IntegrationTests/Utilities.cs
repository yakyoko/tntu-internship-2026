using Projects.Api.Infrastructure;
using Projects.Api.Models;

namespace Projects.Api.Tests.IntegrationTests;

public static class Utilities
{
    public static void InitializeDbForTests(ProjectsDbContext db)
    {
        if (db.Projects.Any())
        {
            db.Projects.RemoveRange(db.Projects);
            db.SaveChanges();
        }

        var now = DateTime.UtcNow;

        db.Projects.AddRange(
            new Project
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Seed Project A",
                Description = "Seeded project A",
                IsArchived = false,
                CreatedAt = now.AddMinutes(-30),
            },
            new Project
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Seed Project B (archived)",
                Description = "Seeded project B",
                IsArchived = true,
                CreatedAt = now.AddMinutes(-20),
            },
            new Project
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Seed Project C",
                Description = "Seeded project C",
                IsArchived = false,
                CreatedAt = now.AddMinutes(-10),
            }
        );

        db.SaveChanges();
    }
}
