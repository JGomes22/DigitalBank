using DigitalBank.Domain.Enums;

namespace DigitalBank.Domain.DTO.Request
{
    public class LancamentoRequest : RequestBase
    {
        public decimal Valor { get; set; }
        public string IdContaOrigem { get; set; }
        public string IdContaDestino { get; set; }
        public EOperacao Operacao { get; set; }
    }
}
