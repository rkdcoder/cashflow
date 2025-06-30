namespace CashFlow.Infrastructure.Shared.Cors
{
    public class CorsSettings
    {
        public string[]? AllowedOrigins { get; set; }
        public bool AllowAnyHeader { get; set; }
        public bool AllowAnyMethod { get; set; }
    }
}