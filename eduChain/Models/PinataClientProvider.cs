using Pinata.Client;

namespace eduChain.Models{
    public static class PinataClientProvider
    {
        private static readonly Lazy<PinataClient> client = new Lazy<PinataClient>(() =>
        {
            var config = new Config
            {
                ApiKey = "809a9fb6a4270d8db032", // Replace with your actual API key
            };
            return new PinataClient(config);
        });

        public static PinataClient Instance => client.Value;
    }
}