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
                headers.ContentSecurityPolicy = "default-src 'none';";

                headers["X-Permitted-Cross-Domain-Policies"] = "none";

                await next();
            });
        }
    }
}
