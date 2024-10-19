using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            var genres = _unitOfWork.Genre.GetAll();
            return Ok(genres);
        }
        [HttpPost]
        public IActionResult CreateGenre()
        {

        }
    }
}
