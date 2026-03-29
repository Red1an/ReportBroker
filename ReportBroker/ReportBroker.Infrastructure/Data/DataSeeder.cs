using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReportBroker.Infrastructure.Data;
using ReportBroker.Infrastructure.Messaging;

namespace ReportBroker.Api.Data;

public class DataSeeder
{
    private readonly IBus _bus;
    private readonly AppDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(
        IBus bus,
        AppDbContext context,
        ILogger<DataSeeder> logger)
    {
        _bus = bus;
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.Report.AnyAsync())
        {
            _logger.LogInformation(
                "База уже содержит данные, пропускаем заполнение");
            return;
        }

        _logger.LogInformation(
            "База пустая, заполняем тестовыми данными...");

        var messages = new List<ReportRequestMessage>
        {
            // Отчёт 1
            new ReportRequestMessage
            {
                UserId = "user-1",
                ProductId = Guid.Parse(
                    "550e8400-e29b-41d4-a716-446655440001"),
                DesignId = Guid.Parse(
                    "6ba7b810-9dad-11d1-80b4-00c04fd430c1"),
                PeriodStart = new DateOnly(2024, 1, 1),
                PeriodEnd = new DateOnly(2024, 1, 31)
            },
            // Отчёт 2 — тот же что 1, другой пользователь
            new ReportRequestMessage
            {
                UserId = "user-2",
                ProductId = Guid.Parse(
                    "550e8400-e29b-41d4-a716-446655440001"),
                DesignId = Guid.Parse(
                    "6ba7b810-9dad-11d1-80b4-00c04fd430c1"),
                PeriodStart = new DateOnly(2024, 1, 1),
                PeriodEnd = new DateOnly(2024, 1, 31)
            },
            // Отчёт 3 — другой товар
            new ReportRequestMessage
            {
                UserId = "user-1",
                ProductId = Guid.Parse(
                    "550e8400-e29b-41d4-a716-446655440002"),
                DesignId = Guid.Parse(
                    "6ba7b810-9dad-11d1-80b4-00c04fd430c2"),
                PeriodStart = new DateOnly(2024, 2, 1),
                PeriodEnd = new DateOnly(2024, 2, 29)
            },
            // Отчёт 4 — третий товар
            new ReportRequestMessage
            {
                UserId = "user-3",
                ProductId = Guid.Parse(
                    "550e8400-e29b-41d4-a716-446655440003"),
                DesignId = Guid.Parse(
                    "6ba7b810-9dad-11d1-80b4-00c04fd430c3"),
                PeriodStart = new DateOnly(2024, 3, 1),
                PeriodEnd = new DateOnly(2024, 3, 31)
            }
        };

        foreach (var message in messages)
        {
            await _bus.Publish(message);

            _logger.LogInformation(
                "Отправлен запрос для продукта {ProductId} " +
                "от пользователя {UserId}",
                message.ProductId, message.UserId);

            await Task.Delay(300);
        }

        _logger.LogInformation(
            "Тестовые данные успешно отправлены!");
    }
}