namespace DigitalBank.Domain.DTO.Request
{
    public class ContaRequest : RequestBase
    {
        public string Numero { get; set; }
        public decimal Saldo { get; private set; }
    }
}
