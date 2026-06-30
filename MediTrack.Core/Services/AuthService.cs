using MediTrack.Core.Data;
using MediTrack.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace MediTrack.Core.Services;

public class AuthService : IAuthService
{
    private readonly MediTrackDbContext _context;
    public User? CurrentUser { get; private set; }
    public event EventHandler? AuthStateChanged;

    public AuthService(MediTrackDbContext context)
    {
        _context = context;
    }

    public async Task<User?> RegisterAsync(string username, string email, string password, string fullName)
    {
        if (await _context.Users.AnyAsync(u => u.Username == username || u.Email == email))
            return null;

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            FullName = fullName
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        CurrentUser = user;
        AuthStateChanged?.Invoke(this, EventArgs.Empty);
        return user;
    }

    public async Task<User?> LoginAsync(string usernameOrEmail, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        CurrentUser = user;
        AuthStateChanged?.Invoke(this, EventArgs.Empty);
        return user;
    }

    public Task LogoutAsync()
    {
        CurrentUser = null;
        AuthStateChanged?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }

    public async Task<bool> UpdateProfileAsync(int userId, string fullName, string email)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        if (await _context.Users.AnyAsync(u => u.Email == email && u.Id != userId))
            return false;

        user.FullName = fullName;
        user.Email = email;
        await _context.SaveChangesAsync();

        if (CurrentUser?.Id == userId)
        {
            CurrentUser.FullName = fullName;
            CurrentUser.Email = email;
        }
        return true;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _context.SaveChangesAsync();
        return true;
    }
}
