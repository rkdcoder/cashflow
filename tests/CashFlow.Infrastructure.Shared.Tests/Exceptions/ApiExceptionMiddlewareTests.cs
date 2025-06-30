using CashFlow.Infrastructure.Shared.Exceptions;
using CashFlow.Shared.Dtos;
using CashFlow.Shared.Exceptions.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CashFlow.Infrastructure.Shared.Tests.Exceptions
{
    public class ApiExceptionMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_Should_Handle_AppException_And_Return_Expected_Response()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ApiExceptionMiddleware>>();
            var exception = new DummyAppException("Test message", 422);
            var context = new DefaultHttpContext();
            var responseStream = new MemoryStream();
            context.Response.Body = responseStream;
            context.Request.Path = "/api/test";
            context.TraceIdentifier = "trace-123";

            RequestDelegate next = ctx => throw exception;

            var middleware = new ApiExceptionMiddleware(next, mockLogger.Object);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(422, context.Response.StatusCode);
            responseStream.Seek(0, SeekOrigin.Begin);
            var json = new StreamReader(responseStream).ReadToEnd();
            var apiError = JsonSerializer.Deserialize<ApiErrorResponseDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(apiError);
            Assert.Contains("Test message", apiError!.Message);
            Assert.Equal("/api/test", apiError.Path);
            Assert.Equal("trace-123", apiError.TraceId);
            mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[422] Test message")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_Should_Handle_UnauthorizedAccessException_And_Return_401()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ApiExceptionMiddleware>>();
            var context = new DefaultHttpContext();
            var responseStream = new MemoryStream();
            context.Response.Body = responseStream;
            context.Request.Path = "/api/auth";
            context.TraceIdentifier = "trace-401";

            RequestDelegate next = ctx => throw new UnauthorizedAccessException("Unauthorized!!");
            var middleware = new ApiExceptionMiddleware(next, mockLogger.Object);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
            responseStream.Seek(0, SeekOrigin.Begin);
            var json = new StreamReader(responseStream).ReadToEnd();
            var apiError = JsonSerializer.Deserialize<ApiErrorResponseDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(apiError);
            Assert.Contains("Unauthorized access", apiError!.Message);
            Assert.Equal("/api/auth", apiError.Path);
            Assert.Equal("trace-401", apiError.TraceId);
            mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[401] Unauthorized access")),
                    It.IsAny<UnauthorizedAccessException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_Should_Handle_KeyNotFoundException_And_Return_404()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ApiExceptionMiddleware>>();
            var context = new DefaultHttpContext();
            var responseStream = new MemoryStream();
            context.Response.Body = responseStream;
            context.Request.Path = "/api/missing";
            context.TraceIdentifier = "trace-404";

            RequestDelegate next = ctx => throw new KeyNotFoundException("Key not found!!");
            var middleware = new ApiExceptionMiddleware(next, mockLogger.Object);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
            responseStream.Seek(0, SeekOrigin.Begin);
            var json = new StreamReader(responseStream).ReadToEnd();
            var apiError = JsonSerializer.Deserialize<ApiErrorResponseDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(apiError);
            Assert.Contains("Resource not found", apiError!.Message);
            mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[404] Resource not found")),
                    It.IsAny<KeyNotFoundException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_Should_Handle_UnexpectedException_And_Return_400()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ApiExceptionMiddleware>>();
            var context = new DefaultHttpContext();
            var responseStream = new MemoryStream();
            context.Response.Body = responseStream;
            context.Request.Path = "/api/boom";
            context.TraceIdentifier = "trace-400";

            RequestDelegate next = ctx => throw new Exception("Generic error!");
            var middleware = new ApiExceptionMiddleware(next, mockLogger.Object);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
            responseStream.Seek(0, SeekOrigin.Begin);
            var json = new StreamReader(responseStream).ReadToEnd();
            var apiError = JsonSerializer.Deserialize<ApiErrorResponseDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(apiError);
            Assert.Contains("An error occurred", apiError!.Message);
            mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[400] An error occurred")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        public class DummyAppException : AppException
        {
            public DummyAppException(string message, int statusCode) : base(message, statusCode) { }
        }

    }
}