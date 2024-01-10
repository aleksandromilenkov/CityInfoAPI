using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using CityInfo.API.Interface;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Repositories {
    public class PointOfInterestRepository : IPointOfInterestRepository {
        private readonly ApplicationDbContext _context;

        public PointOfInterestRepository(ApplicationDbContext context) {
            _context = context;
        }
        public async Task<bool> CreatePointOfInterest(int cityId, PointOfInterest pointOfInterest) {
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == cityId);
            if (city != null) {
                city.PointsOfInterest.Add(pointOfInterest);
            }
            return await Save();
        }

        public async Task<bool> DeletePointOfInterest(PointOfInterest pointOfInterest) {
            _context.PointOfInterests.Remove(pointOfInterest);
            return await Save();
        }

        public async Task<bool> PointOfInterestExists(int id) {
            return await _context.PointOfInterests.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> Save() {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdatePointOfInterest(PointOfInterest pointOfInterest) {
            _context.PointOfInterests.Update(pointOfInterest);
            return await Save();
        }
    }
}
