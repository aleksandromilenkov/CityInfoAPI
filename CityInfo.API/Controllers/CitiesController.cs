using AutoMapper;
using CityInfo.API.Interface;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers {
    [ApiController]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class CitiesController : ControllerBase {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxCitiesPageSize = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper) {
            _cityInfoRepository = cityInfoRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CityWithoutPointOfInterestDto>))]
        public async Task<IActionResult> GetCities([FromQuery] string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10) {
            if (pageSize > maxCitiesPageSize) {
                pageSize = maxCitiesPageSize;
            }
            var (cities, paginationMetadata) = await _cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cities));
        }
        /// <summary>
        /// Get a city by id
        /// </summary>
        /// <param name="id">The id of the city to get</param>
        /// <param name="includePointsOfInterest">Whether or not to include the points of interest</param>
        /// <returns>An IActionResult</returns>
        /// <response code="200">Returns the requested city</response>
        [HttpGet("{cityId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetCity([FromRoute] int cityId, [FromQuery] bool includePointsOfInterest = false) {
            if (cityId <= 0) {
                return BadRequest(ModelState);
            }
            var city = await _cityInfoRepository.GetCityAsync(cityId, includePointsOfInterest);
            if (city == null) {
                return NotFound();
            }
            if (includePointsOfInterest) {
                return Ok(_mapper.Map<CityDto>(city));
            }
            else {
                return Ok(_mapper.Map<CityWithoutPointOfInterestDto>(city));
            }
        }
    }
}
