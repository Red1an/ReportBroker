using Microsoft.Extensions.Logging;
using ReportBroker.Application.DTOs;
using ReportBroker.Domain.Entities;
using ReportBroker.Domain.Interfaces;
using ReportBroker.Domain.ValueObjects;

namespace ReportBroker.Application.Services
{
    public class CreateReportRequestService
    {
        private readonly IReportRequestRepository _requestRepository;
        private readonly IReportRepository _reportRepository;
        private readonly ILogger<CreateReportRequestService> _logger;

        public CreateReportRequestService(IReportRepository reportRepository, 
            IReportRequestRepository requestRepository,
            ILogger<CreateReportRequestService> logger)
        {
            _reportRepository = reportRepository;
            _requestRepository = requestRepository;
            _logger = logger;
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
                _logger.LogInformation(
                "Найден существующий отчёт {ReportId} для продукта {ProductId}",
                existingReport.Id, dto.ProductId);
                report = existingReport;
            }
            else
            {
                _logger.LogInformation(
                "Создаём новый отчёт для продукта {ProductId}",
                dto.ProductId);
                var newReport = Report.Create(parameters);
                report = await _reportRepository.AddAsync(newReport, ct);
            }
            var request = ReportRequest.Create(report!.Id, dto.UserId);            
            request = await _requestRepository.AddAsync(request, ct);

            return request;
        }
    }
}
