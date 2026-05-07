using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Aodp.Api.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class SecurityHeadersExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                var headers = context.Response.Headers;

                headers.StrictTransportSecurity = "max-age=31536000";
                headers.XFrameOptions = "SAMEORIGIN";
                headers.XContentTypeOptions = "nosniff";
                headers.ContentSecurityPolicy =
                    "default-src 'self'; " +
                    "script-src 'self' 'unsafe-inline'; " +
                    "style-src 'self' 'unsafe-inline'; " +
                    "img-src 'self' data:; " +
                    "font-src 'self'; " +
                    "connect-src 'self'; " +
                    "object-src 'none'; " +
                    "base-uri 'self'; " +
                    "frame-ancestors 'none';";

                headers["X-Permitted-Cross-Domain-Policies"] = "none";

                await next();
            });
        }
    }
}
