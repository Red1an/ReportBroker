using ReportBroker.Domain.Interfaces;
using ReportBroker.Domain.Entities;
using ReportBroker.Application.DTOs;

using ReportBroker.Domain.ValueObjects;

namespace ReportBroker.Application.Services
{
    public class CreateReportRequestService
    {
        private readonly IReportRequestRepository _requestRepository;
        private readonly IReportRepository _reportRepository;

        public CreateReportRequestService(IReportRepository reportRepository, 
            IReportRequestRepository requestRepository)
        {
            _reportRepository = reportRepository;
            _requestRepository = requestRepository;
        }

        public async Task<ReportRequest> ExecuteAsync(
            CreateReportRequestDto dto,
            CancellationToken ct = default)
        {
            var parameters = new ReportParameters(
                dto.ProductId,
                dto.DesignId,
                dto.PeriodStart,
                dto.PeriodEnd);

            parameters.Validate();

            var existingReport = await _reportRepository.GetByIdAsync(parameters.ProductId, ct);

            Report? report;

            if (existingReport != null)
            {
                report = existingReport;
            }
            else
            {
                var newReport = Report.Create(parameters);
                report = await _reportRepository.AddAsync(newReport, ct);
            }
            var request = ReportRequest.Create(report!.Id, dto.UserId);

            return request;
        }
    }
}
