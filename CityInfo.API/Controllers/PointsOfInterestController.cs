using AutoMapper;
using CityInfo.API.Interface;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers {
    [Route("api/cities/{cityId}/[controller]")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase {
        private readonly ICitiesDataBase _citiesDataBase;
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;
        private readonly ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ICitiesDataBase citiesDataBase, ILogger<PointsOfInterestController> logger, IMailService mailService, IMapper mapper, ICityInfoRepository cityInfoRepository) {
            _citiesDataBase = citiesDataBase;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _mapper = mapper;
            _cityInfoRepository = cityInfoRepository;
        }


        [HttpGet]
        public async Task<IActionResult> GetPointsOfInterest([FromRoute] int cityId) {
            try {
                var cityExists = await _cityInfoRepository.CityExistsAsync(cityId);
                if (!cityExists) {
                    _logger.LogInformation($"City with id {cityId} cannot be found.");
                    return NotFound();
                }
                var pointOfInterests = _mapper.Map<IEnumerable<PointOfInterestDto>>(await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId));
                return Ok(pointOfInterests);
            }
            catch (Exception e) {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.");
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public async Task<IActionResult> GetPointOfInterest([FromRoute] int pointOfInterestId, int cityId) {
            var city = await _cityInfoRepository.CityExistsAsync(cityId);
            if (!city) {
                return NotFound();
            }
            var point = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (point == null) {
                return NotFound();
            }
            return Ok(_mapper.Map<PointOfInterestDto>(point));
        }

        [HttpPost]
        public IActionResult CreatePointsOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }
            var city = _citiesDataBase.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) {
                return NotFound();
            }
            // for now demo calculating the id:
            var maxPointsOfInterestId = _citiesDataBase.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);
            var newPointOfInterest = new PointOfInterestDto() {
                Id = ++maxPointsOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };
            city.PointsOfInterest.Add(newPointOfInterest);
            return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, pointOfInterestId = newPointOfInterest.Id }, newPointOfInterest);
        }

        [HttpPut("{pointOfInterestId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdatingDto pointOfInterest) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }
            var city = _citiesDataBase.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) {
                return NotFound();
            }
            // find point of interest:
            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (pointOfInterestFromStore == null) {
                return NotFound();
            }

            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;
            return NoContent();
        }

        [HttpPatch("{pointOfInterestId}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdatingDto> patchDocument) {
            var city = _citiesDataBase.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) {
                return NotFound();
            }
            // find point of interest:
            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (pointOfInterestFromStore == null) {
                return NotFound();
            }

            var pointOfInterestToPatch = new PointOfInterestForUpdatingDto() {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description
            };

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid) {
                return BadRequest();
            }

            if (!TryValidateModel(pointOfInterestToPatch)) {
                return BadRequest(ModelState);
            }

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;
            return NoContent();
        }

        [HttpDelete("{pointOfInterestId}")]
        public IActionResult DeletePointOfInterest(int cityId, int pointOfInterestId) {
            var city = _citiesDataBase.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) { return NotFound(); }
            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (pointOfInterestFromStore == null) {
                return NotFound();
            }
            city.PointsOfInterest.Remove(pointOfInterestFromStore);
            _mailService.Send(
               "Point of interest deleted.",
               $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted.");
            return NoContent();
        }

    }
}
