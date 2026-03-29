using MassTransit;
using ReportBroker.Application.DTOs;
using ReportBroker.Application.Services;

namespace ReportBroker.Infrastructure.Messaging
{
    public class ReportRequestConsumer : IConsumer<ReportRequestMessage>
    {
        private readonly CreateReportRequestService _createReqService;
        private readonly ProcessReportService _processRepService;

        public ReportRequestConsumer(CreateReportRequestService createReqService,  ProcessReportService processRepService)
        {
            _createReqService = createReqService;
            _processRepService = processRepService;
        }

        public async Task Consume(ConsumeContext<ReportRequestMessage> context)
        {
            var message = context.Message;

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
        }
    }
}
