using AutoMapper;
using Bm = Business.Models.Flight;
using Entity = Flights.DAL.Entities.Flight;

namespace Flights.DAL.Mapping
{
    internal sealed class FlightProfile : Profile
    {
        public FlightProfile()
        {
            CreateMap<Entity, Bm>()
                .ForMember(x => x.Id, o => o.MapFrom(m => m.Id))
                .ForMember(x => x.Name, o => o.MapFrom(m => m.Name))
                .ForMember(x => x.From, o => o.MapFrom(m => m.From))
                .ForMember(x => x.To, o => o.MapFrom(m => m.To))
                .ForMember(x => x.AirlineId, o => o.MapFrom(m => m.AirlineId))
                .ForMember(x => x.FlightTypeId, o => o.MapFrom(m => m.FlightTypeId))
                .ForMember(x => x.TouristInfoEditLock, o => o.MapFrom(m => m.TouristInfoEditLock))
                .ForMember(x => x.ReturnFlightSameAirlineRequired, o => o.MapFrom(m => m.ReturnFlightSameAirlineRequired))
                .ForMember(x => x.AlternativeSearch, o => o.MapFrom(m => m.AlternativeSearch))
                .ForMember(x => x.FlightDuration, o => o.MapFrom(m => m.FlightDuration))
                .ForMember(x => x.AirplaneModelId, o => o.MapFrom(m => m.AirplaneModelId))
                .ForMember(x => x.StatusId, o => o.MapFrom(m => m.StatusId))
                .ForMember(x => x.NearestDepartureDate, o => o.Ignore())
                .ForMember(x => x.NearestArrivalDate, o => o.Ignore())
                .ForMember(x => x.ReturnFlightIds, o => o.Ignore())
                .ForMember(x => x.IsReturnFlight, o => o.Ignore())
                .ForMember(x => x.FlightToId, o => o.Ignore())
                .ForMember(x => x.Priority, o => o.Ignore())
                .ForMember(x => x.IsAvailable, o => o.Ignore())
                .ForMember(x => x.IsNativeReturnFlight, o => o.Ignore())
                .ReverseMap();

        }
    }
}
