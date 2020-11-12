using AutoMapper;
using System.Linq;
using Bm = Business.Models.Flight;
using Dto = Flights.Contract.Dto.FlightDto;

namespace Flights.Mapping.Profiles
{
    internal sealed class FlightDtoProfile : Profile
    {
        public FlightDtoProfile()
        {
            CreateMap<Bm, Dto>()
                .ForMember(x => x.Id, o => o.MapFrom(m => m.Id))
                .ForMember(x => x.Name, o => o.MapFrom(m => m.Name))
                .ForMember(x => x.From, o => o.MapFrom(m => m.From))
                .ForMember(x => x.To, o => o.MapFrom(m => m.To))
                .ForMember(x => x.NearestDepartureDate, o => o.MapFrom(m => m.NearestDepartureDate))
                .ForMember(x => x.NearestArrivalDate, o => o.MapFrom(m => m.NearestArrivalDate))
                .ForMember(x => x.AirlineId, o => o.MapFrom(m => m.AirlineId))
                .ForMember(x => x.FlightTypeId, o => o.MapFrom(m => m.FlightTypeId))
                .ForMember(x => x.TouristInfoEditLock, o => o.MapFrom(m => m.TouristInfoEditLock))
                .ForMember(x => x.ReturnFlightSameAirlineRequired, o => o.MapFrom(m => m.ReturnFlightSameAirlineRequired))
                .ForMember(x => x.AlternativeSearch, o => o.MapFrom(m => m.AlternativeSearch))
                .ForMember(x => x.FlightDuration, o => o.MapFrom(m => m.FlightDuration))
                .ForMember(x => x.AirplaneModelId, o => o.MapFrom(m => m.AirplaneModelId))
                .ForMember(x => x.StatusId, o => o.MapFrom(m => m.StatusId))
                .ForMember(x => x.ReturnFlightIds, o => o.MapFrom(m => m.ReturnFlightIds.Count>0 && m.ReturnFlightIds.FirstOrDefault()!=0 ? m.ReturnFlightIds : null))//m.ReturnFlightIds.Count != 0 ? m.ReturnFlightIds : null))
                .ForMember(x => x.FlightToId, o => o.MapFrom(m => m.FlightToId))
                .ForMember(x => x.Priority, o => o.MapFrom(m => m.Priority))
                .ForMember(x => x.IsAvailable, o => o.MapFrom(m => m.IsAvailable))
                .ForMember(x => x.IsNativeReturnFlight, o => o.MapFrom(m => m.IsNativeReturnFlight));

            CreateMap<Dto, Bm>()
                .ForMember(x => x.Id, o => o.MapFrom(m => m.Id))
                .ForMember(x => x.Name, o => o.MapFrom(m => m.Name))
                .ForMember(x => x.From, o => o.MapFrom(m => m.From))
                .ForMember(x => x.To, o => o.MapFrom(m => m.To))
                .ForMember(x => x.NearestDepartureDate, o => o.MapFrom(m => m.NearestDepartureDate))
                .ForMember(x => x.NearestArrivalDate, o => o.MapFrom(m => m.NearestArrivalDate))
                .ForMember(x => x.AirlineId, o => o.MapFrom(m => m.AirlineId))
                .ForMember(x => x.FlightTypeId, o => o.MapFrom(m => m.FlightTypeId))
                .ForMember(x => x.TouristInfoEditLock, o => o.MapFrom(m => m.TouristInfoEditLock))
                .ForMember(x => x.ReturnFlightSameAirlineRequired, o => o.MapFrom(m => m.ReturnFlightSameAirlineRequired))
                .ForMember(x => x.AlternativeSearch, o => o.MapFrom(m => m.AlternativeSearch))
                .ForMember(x => x.FlightDuration, o => o.MapFrom(m => m.FlightDuration))
                .ForMember(x => x.AirplaneModelId, o => o.MapFrom(m => m.AirplaneModelId))
                .ForMember(x => x.StatusId, o => o.MapFrom(m => m.StatusId))
                .ForMember(x => x.ReturnFlightIds, o => o.MapFrom(m => m.ReturnFlightIds))
                .ForMember(x => x.FlightToId, o => o.MapFrom(m => m.FlightToId))
                .ForMember(x => x.Priority, o => o.MapFrom(m => m.Priority))
                .ForMember(x => x.IsAvailable, o => o.MapFrom(m => m.IsAvailable))
                .ForMember(x => x.IsNativeReturnFlight, o => o.MapFrom(m => m.IsNativeReturnFlight));
        }
    }
}
