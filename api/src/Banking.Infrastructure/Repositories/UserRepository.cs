using Banking.Application.Interfaces;
using Banking.Domain.Identity;
using Banking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BankingDbContext _context;

    public UserRepository(BankingDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        return user;
    }

    public Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }

    // Email operations
    public async Task<UserEmail> AddEmailAsync(UserEmail email)
    {
        _context.UserEmails.Add(email);
        return email;
    }

    public async Task<bool> EmailExistsAsync(string emailAddress)
    {
        return await _context.UserEmails.AnyAsync(e => e.Address == emailAddress);
    }

    public async Task<UserEmail?> GetEmailAsync(Guid emailId, Guid userId)
    {
        return await _context.UserEmails
            .FirstOrDefaultAsync(e => e.Id == emailId && e.UserId == userId);
    }

    public Task DeleteEmailAsync(UserEmail email)
    {
        _context.UserEmails.Remove(email);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<UserEmail>> GetUserEmailsAsync(Guid userId)
    {
        return await _context.UserEmails
            .Where(e => e.UserId == userId)
            .ToListAsync();
    }

    // Address operations
    public async Task<UserAddress> AddAddressAsync(UserAddress address)
    {
        _context.UserAddresses.Add(address);
        return address;
    }

    public async Task<UserAddress?> GetAddressAsync(Guid addressId, Guid userId)
    {
        return await _context.UserAddresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
    }

    public Task DeleteAddressAsync(UserAddress address)
    {
        _context.UserAddresses.Remove(address);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<UserAddress>> GetUserAddressesAsync(Guid userId)
    {
        return await _context.UserAddresses
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}