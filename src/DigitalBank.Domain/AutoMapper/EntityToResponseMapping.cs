using AutoMapper;
using DigitalBank.Domain.DTO.Request;
using DigitalBank.Domain.DTO.Response;
using DigitalBank.Domain.Entities;
using DigitalBank.Infra.CrossCutting.Extensions;

namespace DigitalBank.Domain.AutoMapper
{
    public class EntityToResponseMapping : Profile
    {
        public EntityToResponseMapping()
        {
            CreateMap<Conta, ContaRequest>();
            CreateMap<Conta, ContaResponse>().ForMember(x => x.SaldoFormatado, y => y.MapFrom(c => c.Saldo.CurrencyFormat()));

            CreateMap<Lancamento, LancamentoRequest>();
            CreateMap<Lancamento, LancamentoResponse>().ForMember(x => x.Operacao, y => y.MapFrom(c => c.Operacao.Description()))
                                                       .ForMember(x => x.Valor, y => y.MapFrom(c => c.Valor.CurrencyFormat()));
        }
    }
}
