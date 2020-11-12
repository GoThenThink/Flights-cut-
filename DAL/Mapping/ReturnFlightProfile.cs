using AutoMapper;
using Bm = Business.Models.Flight;
using Entity = Flights.DAL.Entities.ReturnFlight;

namespace Flights.DAL.Mapping
{
    internal sealed class ReturnFlightProfile : Profile
    {
        public ReturnFlightProfile()
        {
            CreateMap<Bm, Entity>()
                .ForMember(x => x.FlightFromId, o => o.MapFrom(m => m.Id))
                .ForMember(x => x.FlightToId, o => o.Ignore())
                .ForMember(x => x.Priority, o => o.MapFrom(m => m.Priority))
                .ForMember(x => x.IsAvailable, o => o.MapFrom(m => m.IsAvailable))
                .ForMember(x => x.IsNativeReturnFlight, o => o.MapFrom(m => m.IsNativeReturnFlight));
        }
    }
}
