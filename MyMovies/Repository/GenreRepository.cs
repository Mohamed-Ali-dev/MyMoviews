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
    }
}
