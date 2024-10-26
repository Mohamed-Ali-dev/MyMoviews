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
            var user = dbContext.Set<User>().FirstOrDefault(x => x.Name == request.Username &&
            x.Password == request.Password);
            if (user == null) 
                return Unauthorized();
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDiscriptor = new SecurityTokenDescriptor
            {
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new (ClaimTypes.Name, user.Name),
                    new(ClaimTypes.Email, "1@b.com")
                })
            };
            var securiryToken = tokenHandler.CreateToken(tokenDiscriptor);
           var accessToken =  tokenHandler.WriteToken(securiryToken);
            return Ok(accessToken);
        }
    }
}
