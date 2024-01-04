using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers {
    [Route("api/cities/{cityId}/[controller]")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase {
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<PointOfInterestDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetPointsOfInterest([FromRoute] int cityId) {
            var cities = new CitiesDataBase();
            var city = cities.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) {
                return NotFound();
            }
            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{pointOfInterestId}")]
        [ProducesResponseType(200, Type = typeof(PointOfInterestDto))]
        [ProducesResponseType(400)]
        public IActionResult GetPointsOfInterest([FromRoute] int pointOfInterestId, int cityId) {
            var cities = new CitiesDataBase();
            var city = cities.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) {
                return NotFound();
            }
            var point = city.PointsOfInterest.Where(p => p.Id == pointOfInterestId).FirstOrDefault();
            if (point == null) {
                return NotFound();
            }
            return Ok(point);
        }
    }
}
