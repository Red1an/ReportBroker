using ReportBroker.Domain.Interfaces;
using ReportBroker.Domain.Entities;
using ReportBroker.Application.DTOs;

namespace ReportBroker.Application.Services
{
    public class GetReportStatusService
    {
        private readonly IReportRequestRepository _requestRepository;

        public GetReportStatusService(IReportRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<ReportStatusDto> ExecuteAsync(Guid requestId, CancellationToken ct = default)
        {
            var request = await _requestRepository.GetByIdAsync(requestId, ct);

            var dto = new ReportStatusDto
            {
                RequestId = request!.Id,
                ReportId = request.ReportId,
                Status = request.Report.Status.ToString(),
                ConversionRatio = request.Report.ConversionRatio,
                ViewCount = request.Report.ViewCount,
                PaymentCount = request.Report.PaymentCount,
                CreatedAt = request.Report.CreatedAt,
                CompletedAt = request.Report.ComplitedAt
            };
            return dto;
        }
           
    }
}
