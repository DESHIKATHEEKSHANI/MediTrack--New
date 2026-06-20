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
}
