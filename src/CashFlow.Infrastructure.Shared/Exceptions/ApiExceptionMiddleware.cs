using CashFlow.Shared.Dtos;
using CashFlow.Shared.Exceptions.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CashFlow.Infrastructure.Shared.Exceptions
{
    /// <summary>
    /// Middleware to handle and standardize all unhandled exceptions for API responses.
    /// </summary>
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionMiddleware> _logger;

        public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex) // Custom exceptions first
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex.StatusCode;

                var apiError = new ApiErrorResponseDto
                {
                    Message = ex.Message,
                    Path = context.Request.Path,
                    TraceId = context.TraceIdentifier,
#if DEBUG
                    Details = ex.ToString()
#endif
                };

                _logger.LogError(ex, "[{StatusCode}] {Message} | Path: {Path} | TraceId: {TraceId}  | LogType={LogType}",
                    ex.StatusCode, ex.Message, context.Request.Path, context.TraceIdentifier, "application");

                if (!context.Response.HasStarted)
                {
                    var result = JsonSerializer.Serialize(apiError, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    await context.Response.WriteAsync(result);
                }
            }
            catch (Exception ex) // General exceptions
            {
                context.Response.ContentType = "application/json";
                int statusCode = StatusCodes.Status500InternalServerError;
                string message = "An unexpected error has occurred.";

                switch (ex)
                {
                    case UnauthorizedAccessException:
                    case System.Security.SecurityException:
                        statusCode = StatusCodes.Status401Unauthorized;
                        message = $"Unauthorized access. {ex.Message}";
                        break;
                    case KeyNotFoundException:
                        statusCode = StatusCodes.Status404NotFound;
                        message = $"Resource not found. {ex.Message}";
                        break;
                    case ArgumentNullException:
                    case ArgumentOutOfRangeException:
                    case FormatException:
                    case ArgumentException:
                        statusCode = StatusCodes.Status400BadRequest;
                        message = $"Invalid request. {ex.Message}";
                        break;
                    case InvalidOperationException:
                        statusCode = StatusCodes.Status409Conflict;
                        message = $"Invalid operation. {ex.Message}";
                        break;
                    case TimeoutException:
                        statusCode = StatusCodes.Status504GatewayTimeout;
                        message = $"Request timeout. {ex.Message}";
                        break;
                    case NotSupportedException:
                        statusCode = StatusCodes.Status501NotImplemented;
                        message = $"Feature not supported. {ex.Message}";
                        break;
                    default:
                        statusCode = StatusCodes.Status400BadRequest;
                        message = $"An error occurred. {ex.Message}";
                        break;
                }

                context.Response.StatusCode = statusCode;

                var apiError = new ApiErrorResponseDto
                {
                    Message = message,
                    Path = context.Request.Path,
                    TraceId = context.TraceIdentifier,
#if DEBUG
                    Details = ex.ToString()
#endif
                };

                _logger.LogError(ex, "[{StatusCode}] {Message} | Path: {Path} | TraceId: {TraceId}  | LogType={LogType}",
                    statusCode, message, context.Request.Path, context.TraceIdentifier, "application");

                if (!context.Response.HasStarted)
                {
                    var result = JsonSerializer.Serialize(apiError, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    await context.Response.WriteAsync(result);
                }
            }
        }
    }
}
