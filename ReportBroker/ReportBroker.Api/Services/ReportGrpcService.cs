using Grpc.Core;
using ReportBroker.Application.Services;
using ReportBroker.Domain.Exceptions;

namespace ReportBroker.Api.Services
{
    public class ReportGrpcService : ReportService.ReportServiceBase
    {
        private readonly GetReportStatusService _getStatusService;
        private readonly ILogger<ReportGrpcService> _logger;

        public ReportGrpcService(GetReportStatusService getStatusService,
            ILogger<ReportGrpcService> logger) 
        {
            _getStatusService = getStatusService;
            _logger = logger;
        }

        public override async Task<GetReportStatusResponse> GetReportStatus(
            GetReportStatusRequest request,
            ServerCallContext context)
        {       
            if (!Guid.TryParse(request.RequestId, out var requestId))
            {
                throw new RpcException(new Status(
                    StatusCode.InvalidArgument, $"Incorrect format RequestId: {request.RequestId}"));
            }

            try
            {
                var result = await _getStatusService.ExecuteAsync(
                    requestId,
                    context.CancellationToken);

                return new GetReportStatusResponse
                {
                    RequestId = result.RequestId.ToString(),
                    ReportId = result.ReportId.ToString(),
                    Status = result.Status,
                    ConversionRatio = result.ConversionRatio ?? 0,
                    PaymentCount = result.PaymentCount ?? 0,
                    ViewCount = result.ViewCount ?? 0,
                    CreatedAt = result.CreatedAt.ToString("O"),
                    CompletedAt = result.CompletedAt?.ToString("O")
                        ?? string.Empty
                };
            }
            catch (ReportRequestNotFoundException ex)
            {
                _logger.LogWarning(
                "Запрос не найден: {Message}", ex.Message);

                throw new RpcException(new Status(
                    StatusCode.NotFound,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                "Ошибка при получении статуса отчёта");

                throw new RpcException(new Status(
                    StatusCode.Internal,
                    "Внутренняя ошибка сервера"));
            }
        }
    }
}
