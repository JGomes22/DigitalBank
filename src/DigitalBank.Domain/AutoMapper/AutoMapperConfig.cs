using AutoMapper;

namespace DigitalBank.Domain.AutoMapper
{
    public class AutoMapperConfig
    {
        public static MapperConfiguration RegisterMappings()
        {
            return new MapperConfiguration(x =>
            {
                x.AddProfile<EntityToResponseMapping>();
                x.AddProfile<RequestToEntityMap>();
            });
        }
    }
}
