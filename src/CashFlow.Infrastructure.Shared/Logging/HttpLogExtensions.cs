using CashFlow.Shared.Logging;
using Microsoft.Extensions.Logging;

namespace CashFlow.Infrastructure.Shared.Logging
{
    public static class HttpLogExtensions
    {
        public static void LogHttpRequest(this ILogger logger, HttpLogEntryDto logEntry)
        {
            logger.LogInformation(
                "HTTP Request " +
                "LogType={LogType} " +
                "LogRequestId={LogRequestId} " +
                "LogCorrelationId={LogCorrelationId} " +
                "LogRequestStartUtc={LogRequestStartUtc} " +
                "LogRequestEndUtc={LogRequestEndUtc} " +
                "LogElapsedSeconds={LogElapsedSeconds} " +
                "LogIsSuccess={LogIsSuccess} " +
                "LogStatusCode={LogStatusCode} " +
                "LogException={LogException} " +
                "LogUserId={LogUserId} " +
                "LogUserName={LogUserName} " +
                "LogClientApp={LogClientApp} " +
                "LogLocale={LogLocale} " +
                "LogMethod={LogMethod} " +
                "LogIsHttps={LogIsHttps} " +
                "LogProtocol={LogProtocol} " +
                "LogScheme={LogScheme} " +
                "LogPath={LogPath} " +
                "LogPathTemplate={LogPathTemplate} " +
                "LogQueryString={LogQueryString} " +
                "LogRouteValues={LogRouteValues} " +
                "LogAuthorization={LogAuthorization} " +
                "LogHeaders={LogHeaders} " +
                "LogCookies={LogCookies} " +
                "LogReferer={LogReferer} " +
                "LogUserAgent={LogUserAgent} " +
                "LogSecChUa={LogSecChUa} " +
                "LogSecChUaMobile={LogSecChUaMobile} " +
                "LogSecChUaPlatform={LogSecChUaPlatform} " +
                "LogAppName={LogAppName} " +
                "LogHost={LogHost} " +
                "LogContentLength={LogContentLength} " +
                "LogRequestBody={LogRequestBody} " +
                "LogResponseBody={LogResponseBody} " +
                "LogResponseContentType={LogResponseContentType} " +
                "LogLocalIp={LogLocalIp} " +
                "LogLocalPort={LogLocalPort} " +
                "LogRemoteIp={LogRemoteIp} " +
                "LogRemotePort={LogRemotePort} " +
                "LogConnectionId={LogConnectionId} " +
                "LogAdditionalData={LogAdditionalData}",
                "http",
                logEntry.LogRequestId,
                logEntry.LogCorrelationId,
                logEntry.LogRequestStartUtc,
                logEntry.LogRequestEndUtc,
                logEntry.LogElapsedSeconds,
                logEntry.LogIsSuccess,
                logEntry.LogStatusCode,
                logEntry.LogException,
                logEntry.LogUserId,
                logEntry.LogUserName,
                logEntry.LogClientApp,
                logEntry.LogLocale,
                logEntry.LogMethod,
                logEntry.LogIsHttps,
                logEntry.LogProtocol,
                logEntry.LogScheme,
                logEntry.LogPath,
                logEntry.LogPathTemplate,
                logEntry.LogQueryString,
                logEntry.LogRouteValues,
                logEntry.LogAuthorization,
                logEntry.LogHeaders,
                logEntry.LogCookies,
                logEntry.LogReferer,
                logEntry.LogUserAgent,
                logEntry.LogSecChUa,
                logEntry.LogSecChUaMobile,
                logEntry.LogSecChUaPlatform,
                logEntry.LogAppName,
                logEntry.LogHost,
                logEntry.LogContentLength,
                logEntry.LogRequestBody,
                logEntry.LogResponseBody,
                logEntry.LogResponseContentType,
                logEntry.LogLocalIp,
                logEntry.LogLocalPort,
                logEntry.LogRemoteIp,
                logEntry.LogRemotePort,
                logEntry.LogConnectionId,
                logEntry.LogAdditionalData
            );
        }
    }
}
