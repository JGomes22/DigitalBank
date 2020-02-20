namespace DigitalBank.Domain.DTO.Response
{
    public class ContaResponse : ResponseBase
    {
        public string Numero { get; set; }
        public decimal Saldo { get; set; }
        public string SaldoFormatado { get; set; }
    }
}
