using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers {
    [Route("api/cities/{cityId}/[controller]")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase {
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<PointOfInterestDto>))]
        [ProducesResponseType(404)]
        public IActionResult GetPointsOfInterest([FromRoute] int cityId) {
            var cities = new CitiesDataBase();
            var city = cities.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) {
                return NotFound();
            }
            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        [ProducesResponseType(200, Type = typeof(PointOfInterestDto))]
        [ProducesResponseType(404)]
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

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PointOfInterestDto))]
        [ProducesResponseType(400)]
        public IActionResult CreatePointsOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }
            var cities = new CitiesDataBase();
            var city = cities.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) {
                return NotFound();
            }
            // for now demo calculating the id:
            var maxPointsOfInterestId = cities.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);
            var newPointOfInterest = new PointOfInterestDto() {
                Id = ++maxPointsOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };
            city.PointsOfInterest.Add(newPointOfInterest);
            return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, pointOfInterestId = newPointOfInterest.Id }, newPointOfInterest);
        }
    }
}
