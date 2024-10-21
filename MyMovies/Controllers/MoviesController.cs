using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using MyMovies.Models;
using MyMovies.Repository.IRepository;

namespace MyMovies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private new List<string> _allowedExtesions = new List<string>() {".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576 ; 
        public MoviesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var movies = _unitOfWork.Movie.GetAll();
            return Ok(movies);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]MovieDto dto)
        {
            if (!_allowedExtesions.Contains(Path.GetExtension(dto.Poster.FileName.ToLower())))
            {
                return BadRequest("Only .png and .jpg images are allowed!");
            }
            if(dto.Poster.Length > _maxAllowedPosterSize)
            { 
                return BadRequest("Max allowed size for Poster is 1MB!");
            }
            if(!_unitOfWork.Genre.ObjectExist(g => g.Id == dto.GenreId))
            {
                return BadRequest("Invalid Genre Id");

            }
            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);
            var movie = new Movie
            {
                Id = dto.Id,
                GenreId = dto.GenreId,
                Title  = dto.Title,
                Poster = dataStream.ToArray(),
                Rate = dto.Rate,
                Storeline = dto.Storeline,
                Year = dto.Year
            };
            _unitOfWork.Movie.Add(movie);
            _unitOfWork.Save();
            return Ok(movie);
        }
    }
}
