using MyMovies.Models;

namespace MyMovies.Repository.IRepository
{
    public interface IGenreRepository : IRepository<Genre>
    {
        void Update(int id, GenreDto genreDto);
    }
}
