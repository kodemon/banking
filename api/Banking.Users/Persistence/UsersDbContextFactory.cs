using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Banking.Users.Persistence;

/*
 |--------------------------------------------------------------------------------
 | Users DbContext Factory
 |--------------------------------------------------------------------------------
 |
 | Used exclusively by EF Core tooling. Allows migrations to be managed
 | directly from the Banking.Users project without a running host:
 |
 |   dotnet ef migrations add InitialUsers --project Banking.Users --output-dir Persistence/Migrations
 |   dotnet ef database update --project Banking.Users
 |
 | The connection string here is for tooling only. At runtime the connection
 | string comes from the host via AddUsersModule().
 |
 */

internal class UsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseSqlServer("Server=localhost,1433;Database=Banking;User Id=sa;Password=!Password1;TrustServerCertificate=True;")
            .Options;

        return new UsersDbContext(options);
    }
}