using ReportBroker.Domain.Interfaces;
using ReportBroker.Domain.Exceptions;
using ReportBroker.Application.Interfaces;

namespace ReportBroker.Application.Services
{
    public class ProcessReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly ICacheService _cache;

        public ProcessReportService(IReportRepository reportRepository, ICacheService cache)
        {
            _reportRepository = reportRepository;
            _cache = cache;
        }

        public async Task ExecuteAsync(Guid reportId, CancellationToken ct = default)
        {
            var report = await _reportRepository.GetByIdAsync(reportId)
                    ?? throw new ReportNotFoundException(reportId);
            try
            {
                report.Processing();
                await _reportRepository.UpdateAsync(report, ct);

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

            }
            catch (Exception ex) 
            {
                report.Fail();
                await _reportRepository.UpdateAsync(report, ct);
            }
        }
    }
}
