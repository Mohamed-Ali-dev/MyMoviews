using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration.UserSecrets;
using MyMovies.Data;
using MyMovies.Models;
using System.Security.Claims;

namespace MyMovies.Authorization
{
    public class PermissionBasedAuthorizationFilter(ApplicationDbContext dbcontext) : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var attripute = (CheckPermissionAttribute)context.ActionDescriptor.EndpointMetadata
                .FirstOrDefault(x => x is CheckPermissionAttribute);
            if (attripute != null)
            {
                var claimIdentity = context.HttpContext.User.Identity as ClaimsIdentity;
                if (claimIdentity == null || !claimIdentity.IsAuthenticated)
                {
                    context.Result = new ForbidResult();
                }
                else
                {
                    var userId = int.Parse(claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
                    
                    var hasPermissions = dbcontext.Set<UserPermission>().Any(x => x.UserId == userId &&
                    x.PermissionId == attripute.Permission);
                    if ((!hasPermissions))
                    {
                        context.Result = new ForbidResult();
                    }
                    {
                        
                    }
                }
             
            }
        }
    }
}
