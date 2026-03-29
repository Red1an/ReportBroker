using Microsoft.Extensions.Logging;
using ReportBroker.Application.Interfaces;
using ReportBroker.Domain.Exceptions;
using ReportBroker.Domain.Interfaces;

namespace ReportBroker.Application.Services
{
    public class ProcessReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly ICacheService _cache;
        private readonly ILogger<ProcessReportService> _logger;

        public ProcessReportService(IReportRepository reportRepository,
            ICacheService cache, ILogger<ProcessReportService> logger)
        {
            _reportRepository = reportRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task ExecuteAsync(Guid reportId, CancellationToken ct = default)
        {
            var report = await _reportRepository.GetByIdAsync(reportId)
                    ?? throw new ReportNotFoundException(reportId);
            try
            {
                report.Processing();
                await _reportRepository.UpdateAsync(report, ct);

                _logger.LogInformation(
                "Начата обработка отчёта {ReportId}", reportId);

                await Task.Delay(TimeSpan.FromSeconds(5), ct);

                var random = new Random();
                var viewCount = random.Next(100, 1000);
                var paymentCount = random.Next(0, viewCount / 2);

                report.Complete(viewCount, paymentCount);
                await _reportRepository.UpdateAsync(report, ct);

                foreach(var request in report.Requests)
                {
                    await _cache.RemoveAsync($"report-status:{request.Id}", ct);
                }

                _logger.LogInformation(
               "Отчёт {ReportId} успешно обработан. ", reportId);

            }
            catch (Exception ex) 
            {
                _logger.LogError("Ошибка при обработке отчёта {ReportId}", reportId);

                report.Fail();
                await _reportRepository.UpdateAsync(report, ct);
            }
        }
    }
}
