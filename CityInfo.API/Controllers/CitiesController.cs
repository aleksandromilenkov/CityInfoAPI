using CityInfo.API.Interface;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase {
        private readonly ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository) {
            _cityInfoRepository = cityInfoRepository;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CityWithoutPointOfInterestDto>))]
        public async Task<IActionResult> GetCities() {
            var cities = await _cityInfoRepository.GetCitiesAsync();
            var results = new List<CityWithoutPointOfInterestDto>();
            foreach (var city in cities) {
                results.Add(new CityWithoutPointOfInterestDto {
                    Id = city.Id,
                    Name = city.Name,
                    Description = city.Description
                });
            }
            return Ok(results);
        }
        [HttpGet("{cityId:int}")]
        public async Task<IActionResult> GetCity([FromRoute] int cityId) {
            if (cityId <= 0) {
                return BadRequest(ModelState);
            }
            var city = await _cityInfoRepository.GetCityAsync(cityId, true);
            if (city == null) {
                return NotFound();
            }
            return Ok(city);
        }
    }
}
