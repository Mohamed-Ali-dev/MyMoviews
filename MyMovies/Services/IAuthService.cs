using MyMovies.Models;

namespace MyMovies.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterDto dto);
        Task<AuthModel> GetTokenAsync(LogInDto dto);
        Task<string> AddRoleAsync(AddRoleDto dto);
    }
}
