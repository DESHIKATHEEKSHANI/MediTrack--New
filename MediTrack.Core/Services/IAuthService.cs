using MediTrack.Core.Models;

namespace MediTrack.Core.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(string username, string email, string password, string fullName);
    Task<User?> LoginAsync(string usernameOrEmail, string password);
    Task LogoutAsync();
    Task<bool> UpdateProfileAsync(int userId, string fullName, string email);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    User? CurrentUser { get; }
    event EventHandler? AuthStateChanged;
}
