namespace webApiAghos.Security;

public sealed class ApiKeyOptions
{
    public const string SectionName = "ApiKey";

    public List<string> Keys { get; init; } = [];
}
