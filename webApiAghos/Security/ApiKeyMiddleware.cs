using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace webApiAghos.Security;

public sealed class ApiKeyMiddleware(RequestDelegate next, IOptions<ApiKeyOptions> options)
{
    public const string HeaderName = "X-Api-Key";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await next(context);
            return;
        }

        var suppliedKey = context.Request.Headers[HeaderName].ToString();
        var isAuthorized = !string.IsNullOrWhiteSpace(suppliedKey) &&
            options.Value.Keys.Any(configuredKey => IsMatch(configuredKey, suppliedKey));

        if (!isAuthorized)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await next(context);
    }

    private static bool IsMatch(string configuredKey, string suppliedKey)
    {
        var expectedBytes = Encoding.UTF8.GetBytes(configuredKey);
        var suppliedBytes = Encoding.UTF8.GetBytes(suppliedKey);

        return expectedBytes.Length == suppliedBytes.Length &&
               CryptographicOperations.FixedTimeEquals(expectedBytes, suppliedBytes);
    }
}
