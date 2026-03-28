using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Text;

namespace ReportBroker.Domain.Entities
{
    public class ReportRequest
    {
        public Guid Id { get; private set; }
        public Guid ReportId { get; private set; }
        public Report Report { get; private set; } = null!;
        public DateTime RequestAt { get; private set; }
        public string UserId { get; private set; } = string.Empty;

        public ReportRequest() { }

        public static ReportRequest Create(Guid reportId, string userId) 
        {
            if (userId == string.Empty)
                throw new ArgumentException("userId cantt be null");

            return new ReportRequest
            {
                Id = Guid.NewGuid(),
                ReportId = reportId,
                UserId = userId,
                RequestAt = DateTime.UtcNow,
            };
        }

    }
}
