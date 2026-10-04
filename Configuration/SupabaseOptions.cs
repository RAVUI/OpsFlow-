namespace OpsFlow.Configuration;

public class SupabaseOptions
{
    public string Url { get; set; } = string.Empty;

    public string PublishableKey { get; set; } = string.Empty;

    public string Issuer =>
        $"{Url.TrimEnd('/')}/auth/v1";

    public string JwksUrl =>
        $"{Issuer}/.well-known/jwks.json";
}