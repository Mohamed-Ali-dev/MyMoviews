using MyMovies.Data;
using MyMovies.Models;
using MyMovies.Repository.IRepository;

namespace MyMovies.Repository
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly ApplicationDbContext _db;
        public MovieRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Movie movie)
        {
            _db.Update(movie);
        }
    }
}
