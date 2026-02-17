using Banking.Application.Interfaces;
using Banking.Domain.Identity;
using Banking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Repositories;

public class UserRepository(BankingDbContext context) : IUserRepository
{
    public async Task AddAsync(User user)
    {
        await context.Users.AddAsync(user);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await context.Users
            .Include(u => u.Emails)
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await context.Users
            .Include(u => u.Emails)
            .Include(u => u.Addresses)
            .ToListAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string emailAddress)
    {
        return await context.Users
            .AnyAsync(u => u.Emails.Any(e => e.Email.Address == emailAddress));
    }

    public Task DeleteAsync(User user)
    {
        context.Users.Remove(user);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}