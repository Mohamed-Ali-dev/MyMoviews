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
        [HttpPut("{id}")]
        public IActionResult UpdateGenre(int id, [FromBody] GenreDto dto )
        {
            if (!_unitOfWork.Genre.ObjectExist(u => u.Id == id))
            {
                return NotFound($"No Genre with found with Id : {id}");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _unitOfWork.Genre.Update(id, dto);
            _unitOfWork.Save();
            return Ok(dto);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteGenre(int id)
        {
            if(id == 0)
            {
                return BadRequest("Id cannot be zero");
            }
            if (!_unitOfWork.Genre.ObjectExist(u => u.Id == id))
            {
                return NotFound("Genre Not Found");
            }
            var genreToDelete = _unitOfWork.Genre.Get(u => u.Id == id);
           _unitOfWork.Genre.Remove(genreToDelete);
            return Ok(genreToDelete);
        }
    }
}
