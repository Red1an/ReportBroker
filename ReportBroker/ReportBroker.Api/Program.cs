using MassTransit;
using Microsoft.EntityFrameworkCore;
using ReportBroker.Api.Services;
using ReportBroker.Application.Interfaces;
using ReportBroker.Application.Services;
using ReportBroker.Domain.Interfaces;
using ReportBroker.Infrastructure.Cache;
using ReportBroker.Infrastructure.Data;
using ReportBroker.Infrastructure.Data.Repositories;
using ReportBroker.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration.GetConnectionString("Redis"));

builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportRequestRepository, ReportRequestRepository>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();

builder.Services.AddScoped<CreateReportRequestService>();
builder.Services.AddScoped<GetReportStatusService>();
builder.Services.AddScoped<ProcessReportService>();

builder.Services.AddGrpc();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ReportRequestConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"]!);
            h.Password(builder.Configuration["RabbitMq:Password"]!);
        });

        cfg.ReceiveEndpoint("report-requests", e =>
        {
            e.PrefetchCount = 10;

            e.UseMessageRetry(r =>
                r.Interval(3, TimeSpan.FromSeconds(5)));

            e.ConfigureConsumer<ReportRequestConsumer>(ctx);
        });
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.MapGrpcService<ReportGrpcService>();
app.Run();
