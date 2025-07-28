using WebApplication6.Data;
using WebApplication6.Interfaces;
using WebApplication6.Models;

namespace WebApplication6.Repositories
{
    public class SpatialFeatureRepository : GenericRepository<SpatialFeature>, ISpatialFeatureRepository
    {
        public SpatialFeatureRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}