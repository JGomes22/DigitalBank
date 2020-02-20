namespace DigitalBank.Infra.CrossCutting.Extensions
{
    public static class DecimalCurrencyExtension
    {
        public static string CurrencyFormat(this decimal value)
        {
            return $"R$ {value.ToString("n2")}";
        }
    }
}
