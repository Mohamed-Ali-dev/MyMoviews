using MyMovies.Data;
using MyMovies.Repository.IRepository;
using System.Diagnostics.Metrics;

namespace MyMovies.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public IGenreRepository Genre { get; private set; }
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Genre = new GenreRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
