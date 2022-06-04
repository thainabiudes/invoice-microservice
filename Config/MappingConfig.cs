using AutoMapper;
using Invoice.API.Data.ValueObjects;
using Invoice.API.Model;

namespace Invoice.API.Config
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<InvoiceVO, InvoiceEntity>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
