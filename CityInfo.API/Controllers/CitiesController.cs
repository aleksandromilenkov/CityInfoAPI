using AutoMapper;
using CityInfo.API.Interface;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper) {
            _cityInfoRepository = cityInfoRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CityWithoutPointOfInterestDto>))]
        public async Task<IActionResult> GetCities() {
            var cities = _mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(await _cityInfoRepository.GetCitiesAsync());
            return Ok(cities);
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
