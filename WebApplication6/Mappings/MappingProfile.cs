using AutoMapper;
using NetTopologySuite.IO;
using WebApplication6.DTOs;
using WebApplication6.Models;

namespace WebApplication6.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            var wktReader = new WKTReader();
            var wktWriter = new WKTWriter();

            // Kural 1: Veritabanı modelini (SpatialFeature) API DTO'suna (FeatureDto) dönüştürme
            CreateMap<SpatialFeature, FeatureDto>()
                .ForMember(dest => dest.LocationWkt, opt => opt.MapFrom(src => wktWriter.Write(src.Location)))
                // GeometryType alanını, Geometry nesnesinin kendi tip isminden al.
                .ForMember(dest => dest.GeometryType, opt => opt.MapFrom(src => src.Location.GeometryType));

            // Kural 2: Gelen DTO'yu (CreateFeatureDto) veritabanı modeline (SpatialFeature) dönüştürme
            CreateMap<CreateFeatureDto, SpatialFeature>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => wktReader.Read(src.LocationWkt)));
        }
    }
}