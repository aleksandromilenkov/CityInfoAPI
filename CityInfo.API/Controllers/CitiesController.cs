using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase {
        [HttpGet]
        public IActionResult GetCities() {
            var cities = new CitiesDataBase();
            return Ok(cities.Cities);
        }
        [HttpGet("{cityId:int}")]
        public IActionResult GetCity([FromRoute] int cityId) {
            if (cityId == null) {
                return BadRequest(ModelState);
            }
            var cities = new CitiesDataBase();
            var city = cities.Cities.Where(c => c.Id == cityId).FirstOrDefault();
            if (city == null) {
                return NotFound();
            }
            return Ok(city);
        }
    }
}
