namespace DigitalBank.Domain.DTO.Response
{
    public class LancamentoResponse : ResponseBase
    {
        public string Valor { get; set; }
        public string IdContaOrigem { get; set; }
        public string IdContaDestino { get; set; }
        public string Operacao { get; set; }
    }
}
