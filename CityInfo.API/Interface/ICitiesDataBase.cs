using CityInfo.API.Models;

namespace CityInfo.API.Interface {
    public interface ICitiesDataBase {
        List<CityDto> Cities { get; set; }
    }
}
