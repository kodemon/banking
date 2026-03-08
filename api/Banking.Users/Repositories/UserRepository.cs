using Banking.Users.Database;
using Banking.Users.Interfaces;
using Banking.Users.Repositories.Resources;
using Microsoft.EntityFrameworkCore;

namespace Banking.Users.Repositories;

internal class UserRepository(UsersDbContext context) : IUserRepository
{
    public DbSet<User> users = context.Users;

    public async Task AddAsync(User user)
    {
        await users.AddAsync(user);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await users
            .Include(u => u.Emails)
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByOwnerIdAsync(Guid ownerId)
    {
        return await users
            .Include(u => u.Emails)
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.OwnerId == ownerId);
    }

    public async Task<User?> GetByEmailAsync(string emailAddress)
    {
        return await users
            .Include(u => u.Emails)
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Emails.Any((email) => email.Email.Address == emailAddress));
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await users.Include(u => u.Emails).Include(u => u.Addresses).ToListAsync();
    }

    public Task DeleteAsync(User user)
    {
        users.Remove(user);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
