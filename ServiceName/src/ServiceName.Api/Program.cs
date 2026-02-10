using Microsoft.EntityFrameworkCore;
using ServiceName.Extensions;
using ServiceName.Infrastructure.Persistence;
using ServiceName.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var db = services.GetRequiredService<AppDbContext>();

    const int maxRetries = 5;

    for (int i = 1; i <= maxRetries; i++)
    {
        try
        {
            db.Database.Migrate();
            logger.LogInformation("Database migration successful.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Database not ready. Retry {Attempt}/{Max}",
                i,
                maxRetries);

            if (i == maxRetries)
                throw;

            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();
