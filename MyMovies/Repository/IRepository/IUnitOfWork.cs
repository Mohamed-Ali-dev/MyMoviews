namespace MyMovies.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IGenreRepository Genre { get; }
        IMovieRepository Movie { get; }
        void Save();
    }
}
