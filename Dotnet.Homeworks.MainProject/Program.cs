using Dotnet.Homeworks.Data.DatabaseContext;
using Dotnet.Homeworks.MainProject.Configuration;
using Dotnet.Homeworks.MainProject.Services;
using Dotnet.Homeworks.MainProject.ServicesExtensions.DataAccess;
using Dotnet.Homeworks.MainProject.ServicesExtensions.Masstransit;
using Dotnet.Homeworks.MainProject.ServicesExtensions.MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddSingleton<IRegistrationService, RegistrationService>();
builder.Services.AddSingleton<ICommunicationService, CommunicationService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMasstransitRabbitMq(builder.Configuration
    .GetSection(nameof(RabbitMqConfig))
    .Get<RabbitMqConfig>()!);

builder.Services.AddMediatR();
builder.Services.AddDataAccess(builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var migrations = await context.Database.GetPendingMigrationsAsync();
    if (migrations.Any())
    {
        await context.Database.MigrateAsync();
    }
}

app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.Run();