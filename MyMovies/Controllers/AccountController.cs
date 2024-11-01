using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMovies.Services;
using MyMovies.Utility;

namespace MyMovies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(JwtOptions jwtOptions, IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterDto userDto)
        {
         if(!ModelState.IsValid) 
                return BadRequest(ModelState);
         var result = await authService.RegisterAsync(userDto);
            if(!result.IsAuthenticated)
                return BadRequest(result.Message);

            SetRefreshTokenIntoCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> LogIn(LogInDto logInDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.GetTokenAsync(logInDto);

            if(!result.IsAuthenticated)
                return BadRequest(result.Message);

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenIntoCookie(result.RefreshToken, result.RefreshTokenExpiration); 
            return Ok(result);

        }

        [HttpPost("addRole")]
        [Authorize(Roles = SD.Role_Admin)]

        public async Task<IActionResult> AddRoleAsync(AddRoleDto dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.AddRoleAsync(dto);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);
            return Ok(dto);
        }
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var result = await authService. 
        }
        private void SetRefreshTokenIntoCookie(string refreshToken, DateTime expiresOn)
        {
            var cookieOption = new CookieOptions
            {
                HttpOnly = true,
                Expires = expiresOn.ToLocalTime(),
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOption);
        }
    }
}
