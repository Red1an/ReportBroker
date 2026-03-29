using Microsoft.Extensions.Logging;
using ReportBroker.Application.DTOs;
using ReportBroker.Application.Interfaces;
using ReportBroker.Domain.Entities;
using ReportBroker.Domain.Exceptions;
using ReportBroker.Domain.Interfaces;

namespace ReportBroker.Application.Services
{
    public class GetReportStatusService
    {
        private readonly IReportRequestRepository _requestRepository;
        private readonly ICacheService _cache;
        private readonly ILogger<GetReportStatusService> _logger;
        private static string CacheKey(Guid requestId) =>
        $"report-status:{requestId}";

        public GetReportStatusService(IReportRequestRepository requestRepository, 
            ICacheService cache, ILogger<GetReportStatusService> logger)
        {
            _requestRepository = requestRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ReportStatusDto> ExecuteAsync(Guid requestId, CancellationToken ct = default)
        {
            var cacheKey = CacheKey(requestId);
            var cached = await _cache.GetAsync<ReportStatusDto>(cacheKey, ct);

            if (cached != null)
            {
                _logger.LogInformation(
                "Статус запроса {RequestId} получен из кэша", requestId);
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
