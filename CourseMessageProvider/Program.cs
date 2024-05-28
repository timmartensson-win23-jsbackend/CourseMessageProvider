using CourseMessageProvider.Data.Contexts;
using CourseMessageProvider.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(Environment.GetEnvironmentVariable("SqlServer"));
        });
        services.AddScoped<CourseMessageService>();
        services.AddScoped<CleanerService>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var migration = context.Database.GetPendingMigrations();
        if (migration != null && migration.Any())
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"ERROR : Program.cs() :: {ex.Message}");
    }

}

host.Run();
