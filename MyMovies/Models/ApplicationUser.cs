using Microsoft.AspNetCore.Identity;

namespace MyMovies.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(50)]
        public string  FirstName { get; set; }
        [Required, MaxLength(50)]
        public string lastName { get; set; }
    }
}
