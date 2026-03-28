using ReportBroker.Domain.Interfaces;
using ReportBroker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ReportBroker.Infrastructure.Data.Repositories
{
    public class ReportRequestRepository : IReportRequestRepository
    {
        private readonly AppDbContext _context;

        public ReportRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ReportRequest?> AddAsync(ReportRequest request, CancellationToken ct = default)
        {
            await _context.ReportRequest.AddAsync(request, ct);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<ReportRequest?> GetByIdAsync(Guid id, CancellationToken ct = default) 
        {
            return await _context.ReportRequest
                .Include(rr => rr.Report)
                .FirstOrDefaultAsync(rr => rr.Id == id);
        }
    }
}
