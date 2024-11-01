using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyMovies.Models;
using MyMovies.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MyMovies.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        private readonly JwtOptions _jwtOptions;

        public AuthService(UserManager<ApplicationUser> userManager, JwtOptions jwtOptions, RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            _jwtOptions = jwtOptions;
            _roleManager = roleManager;
        }
        public async Task<AuthModel> RegisterAsync(RegisterDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) is not null || await _userManager.FindByEmailAsync(dto.UserName) is not null)
                return new AuthModel { Message = "Email is already registered!" };

            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FirstName = dto.FirstName,
                lastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if(!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}, ";
                }
                return new AuthModel { Message = errors };
            }
            await _userManager.AddToRoleAsync(user, SD.Role_User);
            var jwtSecurityToken = await CreateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens?.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            return new AuthModel {
            Email = user.Email,
            UserName = user.UserName,
            IsAuthenticated = true,
            Roles = new List<string> { SD.Role_User },
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiration = refreshToken.ExpiresOn
            };
          
        }
        public async Task<AuthModel> GetTokenAsync(LogInDto dto)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if(user == null || !await _userManager.CheckPasswordAsync(user, dto.Password)) {
                authModel.Message = "Email Or Password is incorrect";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.UserName = user.UserName;
            authModel.Roles = rolesList.ToList();

            if(user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authModel.RefreshToken = activeRefreshToken.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }
            return authModel;
        }
        public async Task<string> AddRoleAsync(AddRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if(user is null || ! await _roleManager.RoleExistsAsync(dto.Role))
                return "Invalid user Id Or Role";

            if(await _userManager.IsInRoleAsync(user, dto.Role))
                return "User is already assign to this role";

            var result = await _userManager.AddToRoleAsync(user, dto.Role);
            return result.Succeeded ? String.Empty : "Something went wrong";

        }


        internal async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }
            .Union(roleClaims).Union(userClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                 issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwtOptions.lifetime),
                signingCredentials: signingCredentials);
            return jwt;
        }
        public async Task<AuthModel> RefreshToken(string token)
        {
            var authModel = new AuthModel();

             var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if(user == null)
            {
                authModel.IsAuthenticated= false;
                authModel.Message = "Invalid Token";
                return authModel;
            }
            var refreshToken = user.RefreshTokens.Single(t =>t.Token == token);
            if (!refreshToken.IsActive)
            {
                authModel.IsAuthenticated= false;
                authModel.Message = "Inactive Token";
                return authModel;
            }
            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);
            authModel.IsAuthenticated = true;
            authModel.Email = user.Email;
            authModel.UserName = user.UserName;
            authModel.Roles = roles.ToList();
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return authModel;
        }
        private RefreshToken GenerateRefreshToken()
        {
            var reandomNumber = new byte[32];
            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(reandomNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(reandomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow,
            };
        }
    }
}
