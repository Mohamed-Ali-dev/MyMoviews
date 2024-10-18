
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMovies.Models
{
    public class Genre
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }


    }
}
