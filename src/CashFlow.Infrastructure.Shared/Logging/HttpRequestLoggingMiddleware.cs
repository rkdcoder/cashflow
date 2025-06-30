using CashFlow.Shared.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

namespace CashFlow.Infrastructure.Shared.Logging
{
    public class HttpRequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HttpRequestLoggingMiddleware> _logger;

        public HttpRequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<HttpRequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLower();
            if (path.StartsWith("/swagger") || path.Contains(".js") || path.Contains(".css"))
            {
                await _next(context);
                return;
            }

            var requestStart = DateTime.UtcNow;
            var sw = Stopwatch.StartNew();
            string requestBody = string.Empty;
            string responseBody = string.Empty;
            string responseContentType = string.Empty;
            int statusCode = 0;
            bool isSuccess = false;
            string? exceptionString = null;
            var originalBodyStream = context.Response.Body;

            try
            {
                requestBody = await GetRequestBody(context.Request); // before all the others!
                using var memoryStream = new MemoryStream();
                context.Response.Body = memoryStream;
                await _next(context);

                responseBody = await GetResponseBodyAsync(memoryStream);
                await memoryStream.CopyToAsync(originalBodyStream);

                statusCode = context.Response.StatusCode;
                responseContentType = context.Response.ContentType ?? string.Empty;
                isSuccess = statusCode < 400;                
            }
            catch (Exception ex)
            {
                statusCode = 500;
                isSuccess = false;
                exceptionString = ex.ToString();
                responseBody = "[unavailable due to exception]";
                responseContentType = string.Empty;
                _logger.LogError("[HttpRequestLoggingMiddleware] Error: {ErrorMessage} | StackTrace: {StackTrace} | LogType={LogType}", ex.Message, ex.StackTrace, "application");

                throw;
            }
            finally
            {
                sw.Stop();
                context.Response.Body = originalBodyStream;

                if (IsSensitiveEndpoint(path))
                {
                    requestBody = "[REDACTED]";
                    responseBody = "[REDACTED]";
                }

                var authorization = context.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authorization))
                {
                    var spaceIdx = authorization.IndexOf(' ');
                    authorization = spaceIdx > 0 ? authorization.Substring(0, spaceIdx) : "[REDACTED]";
                }

                var logEntry = new HttpLogEntryDto
                {
                    LogRequestId = context.TraceIdentifier,
                    LogCorrelationId = context.Request.Headers.TryGetValue("X-Correlation-ID", out var corr) ? corr.FirstOrDefault() : null,
                    LogRequestStartUtc = requestStart,
                    LogRequestEndUtc = DateTime.UtcNow,
                    LogElapsedSeconds = sw.Elapsed.TotalSeconds,
                    LogIsSuccess = isSuccess,
                    LogException = exceptionString,
                    LogStatusCode = statusCode,
                    LogUserId = TryGetUserId(context.User),
                    LogUserName = context.User.Identity?.IsAuthenticated == true ? context.User.Identity.Name : null,
                    LogClientApp = context.Request.Headers.TryGetValue("X-Client-App", out var app) ? app.FirstOrDefault() : null,
                    LogLocale = context.Request.Headers["Accept-Language"].FirstOrDefault(),
                    LogMethod = context.Request.Method,
                    LogIsHttps = context.Request.IsHttps,
                    LogProtocol = context.Request.Protocol,
                    LogScheme = context.Request.Scheme,
                    LogPath = context.Request.Path,
                    LogPathTemplate = context.GetEndpoint()?.Metadata?.GetMetadata<Microsoft.AspNetCore.Routing.RouteNameMetadata>()?.RouteName,
                    LogQueryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null,
                    LogRouteValues = context.Request.RouteValues.Count > 0 ? string.Join(", ", context.Request.RouteValues.Select(kv => $"{kv.Key}={kv.Value}")) : null,
                    LogHeaders = SerializeHeaders(context.Request.Headers),
                    LogCookies = SerializeCookies(context.Request.Cookies),
                    LogReferer = context.Request.Headers["Referer"].FirstOrDefault(),
                    LogAppName = context.Request.PathBase.HasValue ? context.Request.PathBase.Value.Split('/')[1] : null,
                    LogHost = context.Request.Host.Value,
                    LogUserAgent = context.Request.Headers["User-Agent"].FirstOrDefault(),
                    LogAuthorization = authorization,
                    LogSecChUa = context.Request.Headers["sec-ch-ua"].FirstOrDefault(),
                    LogSecChUaMobile = context.Request.Headers["sec-ch-ua-mobile"].FirstOrDefault(),
                    LogSecChUaPlatform = context.Request.Headers["sec-ch-ua-platform"].FirstOrDefault(),
                    LogContentLength = context.Request.ContentLength,
                    LogRequestBody = requestBody,
                    LogResponseBody = responseBody,
                    LogResponseContentType = responseContentType,
                    LogLocalIp = context.Connection.LocalIpAddress?.ToString(),
                    LogLocalPort = context.Connection.LocalPort,
                    LogRemoteIp = context.Connection.RemoteIpAddress?.ToString(),
                    LogRemotePort = context.Connection.RemotePort,
                    LogConnectionId = context.Connection.Id,
                    LogAdditionalData = null
                };

                _logger.LogHttpRequest(logEntry);
            }
        }

        private async Task<string> GetResponseBodyAsync(Stream body)
        {
            body.Seek(0, SeekOrigin.Begin);
            var bodyString = await new StreamReader(body).ReadToEndAsync();
            body.Seek(0, SeekOrigin.Begin);
            return bodyString;
        }

        private async Task<string> GetRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);
            return body;
        }

        private long? TryGetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim != null && long.TryParse(userIdClaim, out var id))
                return id;
            return null;
        }

        private static string SerializeHeaders(IHeaderDictionary headers)
            => string.Join("\n", headers.Select(h => $"{h.Key}: {h.Value}"));

        private static string SerializeCookies(IRequestCookieCollection cookies)
            => string.Join("; ", cookies.Select(c => $"{c.Key}={c.Value}"));

        private bool IsSensitiveEndpoint(string path)
        {
            return path.Contains("/login") || path.Contains("/password");
        }
    }
}