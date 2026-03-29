using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using ReportBroker.Application.DTOs;
using ReportBroker.Application.Services;
using ReportBroker.Infrastructure.Messaging;

namespace ReportBroker.Infrastructure.Messaging;

public class ReportRequestConsumer : IConsumer<ReportRequestMessage>
{
    private readonly CreateReportRequestService _createService;
    private readonly ProcessReportService _processService;
    private readonly ILogger<ReportRequestConsumer> _logger;

    public ReportRequestConsumer(
        CreateReportRequestService createService,
        ProcessReportService processService,
        ILogger<ReportRequestConsumer> logger)
    {
        _createService = createService;
        _processService = processService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReportRequestMessage> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Получен запрос отчёта от пользователя {UserId}",
            message.UserId);

        var dto = new CreateReportRequestDto
        {
            UserId = message.UserId,
            ProductId = message.ProductId,
            DesignId = message.DesignId,
            PeriodStart = message.PeriodStart,
            PeriodEnd = message.PeriodEnd
        };

        try
        {
            var request = await _createService.ExecuteAsync(
                dto, context.CancellationToken);

            _logger.LogInformation(
                "Id запроса для товара {ProductId}: {RequestId}",
                message.ProductId, request.Id);

            await _processService.ExecuteAsync(
                request.ReportId, context.CancellationToken);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException pg
                  && pg.SqlState == "23505")
        {
            // Race condition — два запроса пришли одновременно
            // Один создал отчёт, второй получил ошибку уникальности
            // Просто повторяем — теперь отчёт точно найдётся
            _logger.LogWarning(
                "Race condition для продукта {ProductId}. " +
                "Повторяем с существующим отчётом",
                message.ProductId);

            var request = await _createService.ExecuteAsync(
                dto, context.CancellationToken);

            _logger.LogInformation(
                "Id запроса для товара {ProductId}: {RequestId}",
                message.ProductId, request.Id);

            await _processService.ExecuteAsync(
                request.ReportId, context.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка при обработке сообщения от {UserId}",
                message.UserId);
            throw;
        }
    }
}