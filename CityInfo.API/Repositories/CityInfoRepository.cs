using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using CityInfo.API.Interface;
using CityInfo.API.Services;
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

        public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize) {

            // collection to start from:
            var collection = _context.Cities.AsQueryable();
            if (!string.IsNullOrWhiteSpace(name)) {
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery)) {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(c => c.Name.Contains(searchQuery) || (c.Description != null && c.Description.Contains(searchQuery)));
            }


            var skipNumber = (pageNumber - 1) * pageSize;
            var totalItemsCount = await _context.Cities.CountAsync();
            var paginationMetadata = new PaginationMetadata(totalItemsCount, pageSize, pageNumber);
            var collectionToReturn = await collection.OrderBy(c => c.Name).Skip(skipNumber).Take(pageSize).ToListAsync();
            return (collectionToReturn, paginationMetadata);

        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointOfInterest) {
            if (includePointOfInterest) {
                return await _context.Cities.Include(c => c.PointsOfInterest).Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }
            return await _context.Cities.FirstOrDefaultAsync(c => c.Id == cityId);
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsNoTracking(int cityId, int pointOfInterestId) {
            return await _context.PointOfInterests.Where(p => p.Id == pointOfInterestId && p.CityId == cityId).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId) {
            return await _context.PointOfInterests.FirstOrDefaultAsync(p => p.Id == pointOfInterestId && p.CityId == cityId);
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId) {
            return await _context.PointOfInterests.Where(p => p.CityId == cityId).ToListAsync();
        }

        public async Task<bool> Save() {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
