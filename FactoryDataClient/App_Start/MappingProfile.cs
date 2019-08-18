using AutoMapper;
using FactoryDataClient.Dtos;
using FactoryDataClient.Models;
using System.Collections.Generic;


namespace FactoryDataClient.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain to dto
            Mapper.CreateMap<Plc, PlcDto>();

            Mapper.CreateMap<Tag, TagDto>();

            Mapper.CreateMap<TagType, TagTypeDto>();

            Mapper.CreateMap<SubscriptionTagRecord, SubscriptionTagRecordDto>();

            Mapper.CreateMap<EventTagRecord, EventTagRecordDto>();

            // Dto to domain
            Mapper.CreateMap<PlcDto, Plc>()
            .ForMember(p => p.Id, opt => opt.Ignore());

            Mapper.CreateMap<TagDto, Tag>()
            .ForMember(t => t.Id, opt => opt.Ignore());

            Mapper.CreateMap<TagTypeDto, TagType>()
            .ForMember(t => t.Id, opt => opt.Ignore());

            Mapper.CreateMap<SubscriptionTagRecordDto, SubscriptionTagRecord>()
            .ForMember(r => r.Id, opt => opt.Ignore());

            Mapper.CreateMap<EventTagRecordDto, EventTagRecord>()
           .ForMember(r => r.Id, opt => opt.Ignore());

            // Dto to domain
            Mapper.CreateMap<IEnumerable<PlcDto>, IEnumerable<Plc>>();

            Mapper.CreateMap<IEnumerable<SubscriptionTagRecordDto>, IEnumerable<SubscriptionTagRecord>>();

            Mapper.CreateMap<IEnumerable<EventTagRecordDto>, IEnumerable<EventTagRecord>>();

        }
    }
}