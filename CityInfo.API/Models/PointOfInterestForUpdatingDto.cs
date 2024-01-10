using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models {
    public class PointOfInterestForUpdatingDto {
        public int Id { get; set; }
        [Required(ErrorMessage = "You should provide a name value.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(200)]
        public string? Description { get; set; }
        public int CityId { get; set; }
    }
}
