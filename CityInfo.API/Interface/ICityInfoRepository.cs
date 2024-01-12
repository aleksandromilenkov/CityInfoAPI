
using CityInfo.API.Entities;
using CityInfo.API.Services;

namespace CityInfo.API.Interface {
    public interface ICityInfoRepository {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);
        Task<City?> GetCityAsync(int cityId, bool includePointOfInterest);
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);
        Task<PointOfInterest?> GetPointOfInterestForCityAsNoTracking(int cityId, int pointOfInterestId);
        Task<bool> CityExistsAsync(int cityId);
        Task<bool> Save();

    }
}
