using AutoMapper;
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
        private readonly IMapper _mapper;
        private new List<string> _allowedExtesions = new List<string>() { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;
        public MoviesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var movies = _unitOfWork.Movie.GetAll(includeProperties: "Genre")
               .OrderByDescending(m => m.Rate);
            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);  
            return Ok(data);
        }
        [HttpGet("{id}")]
        public IActionResult GetMovie(int id)
        {
            if (!_unitOfWork.Movie.ObjectExist(u => u.Id == id))
            {
                return NotFound("Movie Not Found");
            }

            var movie = _unitOfWork.Movie.Get(m => m.Id == id, includeProperties: "Genre");
            var data = _mapper.Map<MovieDetailsDto>(movie);

            return Ok(data);
        }
        [HttpGet("/GetByGenreId/{genreId}")]
        public IActionResult GetByGenreId(byte genreId)
        {
            var moviesOfGenre = _unitOfWork.Movie.GetAll(u => u.GenreId == genreId, includeProperties: "Genre")
            .OrderByDescending(m => m.Rate);
            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(moviesOfGenre);

            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] MovieDto dto)
        {
            if(dto.Poster == null)
            {
                return BadRequest("Poster is required");
            }
            if (!_allowedExtesions.Contains(Path.GetExtension(dto.Poster.FileName.ToLower())))
            {
                return BadRequest("Only .png and .jpg images are allowed!");
            }
            if (dto.Poster.Length > _maxAllowedPosterSize)
            {
                return BadRequest("Max allowed size for Poster is 1MB!");
            }
            if (!_unitOfWork.Genre.ObjectExist(g => g.Id == dto.GenreId))
            {
                return BadRequest("Invalid Genre Id");

            }
            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);
            var movie = _mapper.Map<Movie>(dto);
            movie.Poster = dataStream.ToArray();
            _unitOfWork.Movie.Add(movie);
            _unitOfWork.Save();
            return Ok(movie);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromForm] MovieDto movieDto)
        {

            if (!_unitOfWork.Movie.ObjectExist(g => g.Id == id))
            {
                return NotFound("Movie Not Found");
            }
            var movieToUpdate = _unitOfWork.Movie.Get(m => m.Id == id);

            if (!_unitOfWork.Genre.ObjectExist(g => g.Id == movieDto.GenreId))
            {
                return BadRequest("Invalid Genre Id");

            }
            if (movieDto.Poster != null)
            {
                if (!_allowedExtesions.Contains(Path.GetExtension(movieDto.Poster.FileName.ToLower())))
                {
                    return BadRequest("Only .png and .jpg images are allowed!");
                }
                if (movieDto.Poster.Length > _maxAllowedPosterSize)
                {
                    return BadRequest("Max allowed size for Poster is 1MB!");
                }
                using var dataStream = new MemoryStream();
               await movieDto.Poster.CopyToAsync(dataStream);
                movieToUpdate.Poster = dataStream.ToArray(); ;
            }
            movieToUpdate.Title = movieDto.Title;
            movieToUpdate.Storeline = movieDto.Storeline;
            movieToUpdate.Year = movieDto.Year;
            movieToUpdate.Rate = movieDto.Rate;
            movieToUpdate.GenreId = movieDto.GenreId;
            _unitOfWork.Save();
       return Ok(movieToUpdate);

        }
        [HttpDelete(template:"{id}")]
        public IActionResult DeleteMovie(int id)
        {
            if (!_unitOfWork.Movie.ObjectExist(g => g.Id == id))
            {
                return NotFound("Movie Not Found");

            }
            var movieToDelete = _unitOfWork.Movie.Get(m =>m.Id == id);
            _unitOfWork.Movie.Remove(movieToDelete);
            _unitOfWork.Save();
            return Ok(movieToDelete);
        }
    }
}
