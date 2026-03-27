using ReportBroker.Domain.Entities;

namespace ReportBroker.Domain.Interfaces
{
    public interface IReportRepository
    {
        Task<Report?> GetByIdAsync(Guid id, CancellationToken ct = default);
    }   
}
