using CityInfo.API.Interface;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers {
    [Route("api/cities/{cityId}/[controller]")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase {
        private readonly ICitiesDataBase _citiesDataBase;

        public PointsOfInterestController(ICitiesDataBase citiesDataBase) {
            _citiesDataBase = citiesDataBase;
        }
        [HttpGet]
        public IActionResult GetPointsOfInterest([FromRoute] int cityId) {
            var city = _citiesDataBase.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) {
                return NotFound();
            }
            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public IActionResult GetPointsOfInterest([FromRoute] int pointOfInterestId, int cityId) {
            var city = _citiesDataBase.Cities.FirstOrDefault(c => c.Id == cityId);
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

    }
}
