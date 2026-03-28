using ReportBroker.Domain.Entities;
using ReportBroker.Domain.ValueObjects;

namespace ReportBroker.Domain.Interfaces
{
    public interface IReportRepository
    {
        Task<Report?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Report?> GetByParametersAsync(ReportParameters parameters, CancellationToken ct = default);

        Task<Report?> AddAsync(Report report, CancellationToken ct = default);

        Task<Report?> UpdateAsync(Report report, CancellationToken ct = default);
    }   
}
