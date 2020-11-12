using AutoMapper;
using Bm = Business.Models.Airline;
using Dto = Flights.Contract.Dto.AirlineDto;

namespace Flights.Mapping.Profiles
{
    internal sealed class AirlineDtoProfile : Profile
    {
        public AirlineDtoProfile()
        {
            CreateMap<Bm, Dto>()
                .ForMember(x => x.Id, o => o.MapFrom(m => m.Id))
                .ForMember(x => x.Name, o => o.MapFrom(m => m.Name))
                .ForMember(x => x.Iata, o => o.MapFrom(m => m.Iata))
                .ReverseMap();
        }
    }
}
