
using HeaterSim.Models;
using Version2.Models;
using Version2.Models.Version2.Models;
using Version2.Seeding; // Added namespace for seeding

namespace Version2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Use the seeder class to initialise EnvironmentState
            builder.Services.AddSingleton<EnvironmentState>(provider =>
            {
                return EnvironmentStateSeeder.SeedEnvironmentState(); // Modified to call seeder
            });

            builder.Services.AddSingleton<EnvironmentState>();
            builder.Services.AddSingleton<ApiKeyManager>();
            builder.Services.AddSingleton<ClientStateManager>();
            builder.Services.AddSingleton<TemperatureCalculator>();
            builder.Services.AddHostedService<EnvironmentUpdater>();
            builder.Services.AddHostedService<StateCleanupService>();
            builder.Services.AddHostedService<SimulationMonitorService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            // }

            app.UseHttpsRedirection();

            app.UseMiddleware<ApiKeyMiddleware>();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
