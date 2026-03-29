using MassTransit;
using Microsoft.Extensions.Logging;
using ReportBroker.Application.DTOs;
using ReportBroker.Application.Services;

namespace ReportBroker.Infrastructure.Messaging
{
    public class ReportRequestConsumer : IConsumer<ReportRequestMessage>
    {
        private readonly CreateReportRequestService _createReqService;
        private readonly ProcessReportService _processRepService;
        private readonly ILogger<ReportRequestConsumer> _logger;

        public ReportRequestConsumer(CreateReportRequestService createReqService,  
            ProcessReportService processRepService, ILogger<ReportRequestConsumer> logger)
        {
            _createReqService = createReqService;
            _processRepService = processRepService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReportRequestMessage> context)
        {
            var message = context.Message;

            _logger.LogInformation(
           "Получен запрос отчёта от пользователя {UserId}", message.UserId);
            try
            {
                var dto = new CreateReportRequestDto
                {
                    UserId = message.UserId,
                    ProductId = message.ProductId,
                    DesignId = message.DesignId,
                    PeriodStart = message.PeriodStart,
                    PeriodEnd = message.PeriodEnd
                };

                var request = await _createReqService.ExecuteAsync(dto, context.CancellationToken);

                await _processRepService.ExecuteAsync(request.ReportId, context.CancellationToken);
                _logger.LogInformation(
           "Id запроса для товара {ProductId}: {Request}", message.ProductId, request.Id);
            }
            catch (Exception) 
            {
                _logger.LogError(
                "Ошибка при обработке сообщения от {UserId}", message.UserId);
                throw;
            }
            
        }
    }
}
