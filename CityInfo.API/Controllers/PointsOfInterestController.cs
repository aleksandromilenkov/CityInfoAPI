using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Interface;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers {
    [Route("api/v{version:apiVersion}/cities/{cityId}/[controller]")]
    //[Authorize(Policy = "MustBeFromAntwerp")]
    [ApiController]
    [ApiVersion("2.0")]
    public class PointsOfInterestController : ControllerBase {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IPointOfInterestRepository _pointOfInterestRepository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, IMapper mapper, ICityInfoRepository cityInfoRepository, IPointOfInterestRepository pointOfInterestRepository) {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _mapper = mapper;
            _cityInfoRepository = cityInfoRepository;
            _pointOfInterestRepository = pointOfInterestRepository;
        }


        [HttpGet]
        public async Task<IActionResult> GetPointsOfInterest([FromRoute] int cityId) {
            try {
                //var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;
                //if (!await _pointOfInterestRepository.CityNameMatchesCityId(cityName, cityId)) {
                //    return Forbid();
                //}
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
        public async Task<IActionResult> CreatePointsOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }
            var city = await _cityInfoRepository.CityExistsAsync(cityId);
            if (!city) {
                return NotFound();
            }
            var pointOfInterestToBeCreated = _mapper.Map<PointOfInterest>(pointOfInterest);
            var isCreated = await _pointOfInterestRepository.CreatePointOfInterest(cityId, pointOfInterestToBeCreated);
            var createdPointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(pointOfInterestToBeCreated);
            if (isCreated) {
                return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, pointOfInterestId = createdPointOfInterestToReturn.Id }, createdPointOfInterestToReturn);
            }
            else {
                return BadRequest(ModelState);
            }
        }


        [HttpPut("{pointOfInterestId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdatingDto pointOfInterest) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }
            var city = await _cityInfoRepository.CityExistsAsync(cityId);
            if (!city) {
                return NotFound();
            }

            if (!await _pointOfInterestRepository.PointOfInterestExists(pointOfInterestId)) {
                return NotFound();
            }
            // find point of interest:
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsNoTracking(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null) {
                return NotFound();
            }

            var pointOfInterestToBeUpdated = _mapper.Map<PointOfInterest>(pointOfInterest);
            pointOfInterestToBeUpdated.CityId = cityId;
            pointOfInterestToBeUpdated.Id = pointOfInterestId;
            var isUpdated = await _pointOfInterestRepository.UpdatePointOfInterest(pointOfInterestToBeUpdated);
            if (isUpdated) {

                return NoContent();
            }
            else {
                return BadRequest();
            }
        }

        [HttpPatch("{pointOfInterestId}")]
        public async Task<IActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdatingPartialyDto> patchDocument) {
            var city = await _cityInfoRepository.CityExistsAsync(cityId);
            if (!city) {
                return NotFound();
            }
            // find point of interest:
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null) {
                return NotFound();
            }

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdatingPartialyDto>(pointOfInterestEntity);

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid) {
                return BadRequest();
            }

            if (!TryValidateModel(pointOfInterestToPatch)) {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
            await _pointOfInterestRepository.Save();
            return NoContent();
        }

        [HttpDelete("{pointOfInterestId}")]
        public async Task<IActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId) {
            var city = await _cityInfoRepository.CityExistsAsync(cityId);
            if (!city) { return NotFound(); }
            var pointOfInterestToDelete = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestToDelete == null) {
                return NotFound();
            }
            await _pointOfInterestRepository.DeletePointOfInterest(pointOfInterestToDelete);
            _mailService.Send(
               "Point of interest deleted.",
               $"Point of interest {pointOfInterestToDelete.Name} with id {pointOfInterestToDelete.Id} was deleted.");
            return NoContent();
        }



    }
}
