using Supabase;
using Microsoft.Extensions.Configuration;
using eduChain;
using Microsoft.Extensions.Options;

public interface ISupabaseClientFactory
{
    Supabase.Client CreateClient();
}

public class SupabaseClientFactory : ISupabaseClientFactory
{
    private readonly AppSettings _settings;

    public SupabaseClientFactory(IOptions<AppSettings> options)
    {
        _settings = options.Value;
    }

    public Supabase.Client CreateClient()
    {
        return new Supabase.Client(_settings.SupabaseUrl, _settings.SupabaseApiKey);

    }
}
