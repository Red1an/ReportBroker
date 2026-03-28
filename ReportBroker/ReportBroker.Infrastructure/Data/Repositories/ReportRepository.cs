using ReportBroker.Domain.Interfaces;
using ReportBroker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ReportBroker.Domain.ValueObjects;

namespace ReportBroker.Infrastructure.Data.Repositories
{
    internal class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context) 
        {
            _context = context;
        }

        public async Task<Report?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Report
                .Include(r => r.Requests)
                .FirstOrDefaultAsync(r => r.Id == id, ct);
        }

        public async Task<Report?> GetByParametersAsync(ReportParameters parameters, CancellationToken ct = default)
        {
            return await _context.Report
                .Include(r => r.Requests)
                .FirstOrDefaultAsync(r =>
                    r.Parameters.ProductId == parameters.ProductId &&
                    r.Parameters.DesignId == parameters.DesignId &&
                    r.Parameters.PeriodStart == parameters.PeriodStart &&
                    r.Parameters.PeriodEnd == parameters.PeriodEnd);            
        }

        public async Task<Report?> AddAsync(Report report, CancellationToken ct = default)
        {
            await _context.Report.AddAsync(report, ct);
            await _context.SaveChangesAsync(ct);
            return report;
        }

        public async Task UpdateAsync(Report report, CancellationToken ct = default)
        {
            _context.Report.Update(report);
            await _context.SaveChangesAsync(ct);
        }
    }
}
