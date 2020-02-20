namespace DigitalBank.Infra.CrossCutting.Ioc
{
    public static class Bootstrapper
    {
        public static readonly string _mongoConnection = "mongodb://host.docker.internal:27017/?readPreference=primary";
        public static readonly string _secretKey = "fedaf7d8863b48e197b9287d492b708e";
    }
}
