using MassTransit;
using Microsoft.Extensions.Logging;
using ReportBroker.Infrastructure.Messaging;

namespace ReportBroker.Api.Data;

public class DataSeeder
{
    private readonly IBus _bus;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(IBus bus, ILogger<DataSeeder> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Заполняем базу тестовыми данными...");

        var messages = new List<ReportRequestMessage>
        {
            new ReportRequestMessage
            {
                UserId = "user-1",
                ProductId = Guid.Parse("550e8400-e29b-41d4-a716-446655440001"),
                DesignId = Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c1"),
                PeriodStart = new DateOnly(2024, 1, 1),
                PeriodEnd = new DateOnly(2024, 1, 31)
            },
            //new ReportRequestMessage
            //{
            //    UserId = "user-2",
            //    ProductId = Guid.Parse("550e8400-e29b-41d4-a716-446655440001"),
            //    DesignId = Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c1"),
            //    PeriodStart = new DateOnly(2024, 1, 1),
            //    PeriodEnd = new DateOnly(2024, 1, 31)
            //},
            new ReportRequestMessage
            {
                UserId = "user-1",
                ProductId = Guid.Parse("550e8400-e29b-41d4-a716-446655440002"),
                DesignId = Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c2"),
                PeriodStart = new DateOnly(2024, 2, 1),
                PeriodEnd = new DateOnly(2024, 2, 29)
            },
            new ReportRequestMessage
            {
                UserId = "user-3",
                ProductId = Guid.Parse("550e8400-e29b-41d4-a716-446655440003"),
                DesignId = Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c3"),
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

            await Task.Delay(500);
        }

        _logger.LogInformation("Тестовые данные отправлены");
    }
}