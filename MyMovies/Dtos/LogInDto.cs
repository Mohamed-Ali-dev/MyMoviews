using System.ComponentModel.DataAnnotations;

namespace MyMovies.Dtos
{
    public class LogInDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
