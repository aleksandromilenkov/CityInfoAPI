using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;

namespace CityInfo.API.Profiles {
    public class PointOfInterestProfile : Profile {
        public PointOfInterestProfile() {
            CreateMap<PointOfInterest, PointOfInterestDto>().ReverseMap();
            CreateMap<PointOfInterestForCreationDto, PointOfInterest>().ReverseMap();
            CreateMap<PointOfInterestForUpdatingDto, PointOfInterest>().ReverseMap();
            CreateMap<PointOfInterest, PointOfInterestForUpdatingPartialyDto>().ReverseMap();
        }
    }
}
