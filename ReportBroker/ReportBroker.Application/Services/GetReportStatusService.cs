using ReportBroker.Domain.Interfaces;
using ReportBroker.Domain.Entities;
using ReportBroker.Domain.Exceptions;
using ReportBroker.Application.DTOs;
using ReportBroker.Application.Interfaces;

namespace ReportBroker.Application.Services
{
    public class GetReportStatusService
    {
        private readonly IReportRequestRepository _requestRepository;
        private readonly ICacheService _cache;
        private static string CacheKey(Guid requestId) =>
        $"report-status:{requestId}";

        public GetReportStatusService(IReportRequestRepository requestRepository, ICacheService cache)
        {
            _requestRepository = requestRepository;
            _cache = cache;
        }

        public async Task<ReportStatusDto> ExecuteAsync(Guid requestId, CancellationToken ct = default)
        {
            var cacheKey = CacheKey(requestId);
            var cached = await _cache.GetAsync<ReportStatusDto>(cacheKey, ct);

            if (cached != null)
            {
                return cached;
            }

            var request = await _requestRepository.GetByIdAsync(requestId, ct)
                    ?? throw new ReportRequestNotFoundException(requestId);

            var dto = new ReportStatusDto
            {
                RequestId = request.Id,
                ReportId = request.ReportId,
                Status = request.Report.Status.ToString(),
                ConversionRatio = request.Report.ConversionRatio,
                ViewCount = request.Report.ViewCount,
                PaymentCount = request.Report.PaymentCount,
                CreatedAt = request.Report.CreatedAt,
                CompletedAt = request.Report.CompletedAt
            };

            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromSeconds(30), ct);

            return dto;
        }
           
    }
}
