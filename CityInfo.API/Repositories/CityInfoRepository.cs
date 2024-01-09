using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using CityInfo.API.Interface;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Repositories {
    public class CityInfoRepository : ICityInfoRepository {
        private readonly ApplicationDbContext _context;

        public CityInfoRepository(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<bool> CityExistsAsync(int cityId) {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task<IEnumerable<City>> GetCitiesAsync() {
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointOfInterest) {
            if (includePointOfInterest) {
                return await _context.Cities.Include(c => c.PointsOfInterest).Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }
            return await _context.Cities.FirstOrDefaultAsync(c => c.Id == cityId);
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId) {
            return await _context.PointOfInterests.FirstOrDefaultAsync(p => p.Id == pointOfInterestId && p.CityId == cityId);
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId) {
            return await _context.PointOfInterests.Where(p => p.CityId == cityId).ToListAsync();
        }
    }
}
