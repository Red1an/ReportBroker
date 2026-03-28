using ReportBroker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportBroker.Domain.Interfaces
{
    public interface IReportRequestRepository
    {
        Task<ReportRequest?> AddAsync(ReportRequest request, CancellationToken ct = default);

        Task<ReportRequest?> GetByIdAsync(Guid id, CancellationToken ct = default);
    }
}
