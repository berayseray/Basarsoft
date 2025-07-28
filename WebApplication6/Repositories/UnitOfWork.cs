using System.Threading.Tasks;
using WebApplication6.Data;
using WebApplication6.Interfaces;

namespace WebApplication6.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private ISpatialFeatureRepository _featureRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // Repository'yi sadece ilk istendiğinde oluştur (lazy initialization).
        public ISpatialFeatureRepository Features => _featureRepository ??= new SpatialFeatureRepository(_context);

        public async Task<int> CompleteAsync()
        {
            // Tüm değişiklikleri tek bir transaction'da veritabanına yansıt.
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}