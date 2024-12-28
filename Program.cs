
using Microsoft.Extensions.Caching.Memory;

namespace MinimalApiMemoryCache
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddMemoryCache(); // MemoryCache service
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            var random = new Random();

            // Endpoint with no memory cache
            app.MapGet("/no-cache", () =>
            {
                var forecast = GenerateRandomWeather(random);
                return Results.Ok(new { Source = "No Cache", Data = forecast });
            });

            // Endpoint with 5 seconds memory cache
            app.MapGet("/cache-5-seconds", (IMemoryCache memoryCache) =>
            {
                const string cacheKey = "Weather_5_Seconds";

                if (!memoryCache.TryGetValue(cacheKey, out WeatherForecast? forecast))
                {
                    forecast = GenerateRandomWeather(random);
                    memoryCache.Set(cacheKey, forecast, TimeSpan.FromSeconds(5));
                }

                return Results.Ok(new { Source = "5 Seconds Cache", Data = forecast });
            });

            // Endpoint with 30 seconds memory cache
            app.MapGet("/cache-30-seconds", (IMemoryCache memoryCache) =>
            {
                const string cacheKey = "Weather_30_Seconds";

                if (!memoryCache.TryGetValue(cacheKey, out WeatherForecast? forecast))
                {
                    forecast = GenerateRandomWeather(random);
                    memoryCache.Set(cacheKey, forecast, TimeSpan.FromSeconds(30));
                }

                return Results.Ok(new { Source = "30 Seconds Cache", Data = forecast });
            });

            app.Run();
        }

        private static WeatherForecast GenerateRandomWeather(Random random)
        {
            return new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = random.Next(-20, 55),
                Summary = GetRandomSummary(random)
            };
        }

        private static string GetRandomSummary(Random random)
        {
            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };
            return summaries[random.Next(summaries.Length)];
        }

        public class WeatherForecast
        {
            public DateTime Date { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; } = string.Empty;
        }
    }
}
