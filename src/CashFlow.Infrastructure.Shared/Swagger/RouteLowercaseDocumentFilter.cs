using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CashFlow.Infrastructure.Shared.Swagger
{
    /// <summary>
    /// Swagger document filter that converts all route paths to lowercase,
    /// except for path parameters (enclosed in curly braces).
    /// </summary>
    public class RouteLowercaseDocumentFilter : IDocumentFilter
    {
        /// <summary>
        /// Applies the lowercase transformation to all paths in the Swagger document.
        /// </summary>
        /// <param name="swaggerDoc">The OpenAPI/Swagger document being processed.</param>
        /// <param name="context">The document filter context.</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var newPaths = new OpenApiPaths();

            // Process each path in the document
            foreach (var (key, value) in swaggerDoc.Paths)
            {
                var lowerPath = ToLowercasePath(key);
                newPaths.Add(lowerPath, value);
            }

            swaggerDoc.Paths = newPaths;
        }

        /// <summary>
        /// Converts a path to lowercase while preserving path parameter casing.
        /// </summary>
        /// <param name="path">The original path string.</param>
        /// <returns>The path with segments converted to lowercase, except parameters.</returns>
        private static string ToLowercasePath(string path)
        {
            var parts = path.Split('/')
                            .Select(part => part.StartsWith("{") && part.EndsWith("}")
                                ? part  // Preserve parameter casing
                                : part.ToLowerInvariant());  // Lowercase static segments
            return string.Join('/', parts);
        }
    }
}