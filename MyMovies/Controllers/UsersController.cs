using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyMovies.Data;
using MyMovies.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyMovies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(JwtOptions jwtOptions, ApplicationDbContext dbContext) : ControllerBase
    {
        [HttpPost]
        [Route("auth")]
        public IActionResult AuthenticateUser(AuthenticationRequestDto request)
        {
         
            return Ok();
        }
    }
}
