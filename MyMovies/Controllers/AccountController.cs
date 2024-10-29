using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyMovies.Data;
using MyMovies.Models;
using MyMovies.Services;
using MyMovies.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyMovies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(JwtOptions jwtOptions, IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> AuthenticateUser(RegisterDto userDto)
        {
         if(!ModelState.IsValid) 
                return BadRequest(ModelState);
         var result = await authService.RegisterAsync(userDto);
            if(!result.IsAuthenticated)
                return BadRequest(result.Message);

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

            return Ok(result);

        }

        [HttpPost("addRole")]
      //  [Authorize(Roles = SD.Role_Admin)]

        public async Task<IActionResult> AddRoleAsync(AddRoleDto dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.AddRoleAsync(dto);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);
            return Ok(dto);
        }
    }
}
