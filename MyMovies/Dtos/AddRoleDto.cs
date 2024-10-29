using System.ComponentModel.DataAnnotations;

namespace MyMovies.Dtos
{
    public class AddRoleDto
    {
        [Required]
        public string  UserId { get; set; }
        [Required]
        public string  Role { get; set; }
    }
}
