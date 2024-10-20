using MyMovies.Data;
using MyMovies.Models;
using MyMovies.Repository.IRepository;

namespace MyMovies.Repository
{
    public class GenreRepository : Repository<Genre>, IGenreRepository
    {
        private readonly ApplicationDbContext _db;
        public GenreRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(int id ,GenreDto genreDto)
        {
            var genreFromDb = _db.Genres.Where(g => g.Id == id)
                .FirstOrDefault(); 
            genreFromDb.Name = genreDto.Name;
        }
    }
}
