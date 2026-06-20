using MediTrack.Core.Models;

namespace MediTrack.Core.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(string username, string email, string password, string fullName);
    Task<User?> LoginAsync(string usernameOrEmail, string password);
    Task LogoutAsync();
    User? CurrentUser { get; }
    event EventHandler? AuthStateChanged;
}
