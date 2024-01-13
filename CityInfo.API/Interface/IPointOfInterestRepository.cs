using CityInfo.API.Entities;

namespace CityInfo.API.Interface {
    public interface IPointOfInterestRepository {
        Task<bool> CityNameMatchesCityId(string? cityName, int cityId);
        Task<bool> PointOfInterestExists(int id);
        Task<bool> CreatePointOfInterest(int cityId, PointOfInterest pointOfInterest);
        Task<bool> UpdatePointOfInterest(PointOfInterest pointOfInterest);
        Task<bool> DeletePointOfInterest(PointOfInterest pointOfInterest);
        Task<bool> Save();
    }
}
