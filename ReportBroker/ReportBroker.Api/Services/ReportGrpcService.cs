using Grpc.Core;
using ReportBroker.Application.Services;
using ReportBroker.Domain.Exceptions;

namespace ReportBroker.Api.Services
{
    public class ReportGrpcService : ReportService.ReportServiceBase
    {
        private readonly GetReportStatusService _getStatusService;

        public ReportGrpcService(GetReportStatusService getStatusService) 
        {
            _getStatusService = getStatusService;
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
                throw new RpcException(new Status(
                    StatusCode.NotFound,
                    ex.Message));
            }
            catch (Exception ex)
            {               
                throw new RpcException(new Status(
                    StatusCode.Internal,
                    "Внутренняя ошибка сервера"));
            }
        }
    }
}
