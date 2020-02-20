using AutoMapper;
using DigitalBank.Domain.DTO.Request;
using DigitalBank.Domain.DTO.Response;
using DigitalBank.Domain.Entities;

namespace DigitalBank.Domain.AutoMapper
{
    public class RequestToEntityMap : Profile
    {
        public RequestToEntityMap()
        {
            CreateMap<ContaRequest, Conta>();
            CreateMap<ContaResponse, Conta>();
            CreateMap<LancamentoRequest, Lancamento>();
            CreateMap<LancamentoResponse, Lancamento>();
        }
    }
}
