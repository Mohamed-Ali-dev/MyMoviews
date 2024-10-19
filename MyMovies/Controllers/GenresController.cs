using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyMovies.Models;
using MyMovies.Repository.IRepository;

namespace MyMovies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public GenresController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public IActionResult GetGenres()
        {
            var genres = _unitOfWork.Genre.GetAll().OrderBy(g =>g.Name);
            return Ok(genres);
        }
        [HttpPost]
        public IActionResult CreateGenre(GenreDto genreDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var genre = new Genre
            {
                Name = genreDto.Name,
            };
            _unitOfWork.Genre.Add(genre);
            _unitOfWork.Save();
            return Ok(genre);

        }
    }
}
