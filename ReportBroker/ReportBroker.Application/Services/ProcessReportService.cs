using ReportBroker.Domain.Interfaces;

namespace ReportBroker.Application.Services
{
    public class ProcessReportService
    {
        private readonly IReportRepository _reportRepository;

        public ProcessReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task ExecuteAsync(Guid ReportId, CancellationToken ct = default)
        {
            var report = await _reportRepository.GetByIdAsync(ReportId);

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

            }
            catch (Exception ex) 
            {
                report.Fail();
                await _reportRepository.UpdateAsync(report, ct);
            }
        }
    }
}
