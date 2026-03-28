using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Text;

namespace ReportBroker.Domain.Entities
{
    public class ReportRequest
    {
        public Guid Id { get; set; }
        public Guid ReportId { get; set; }
        public Report Report { get; set; } = null!;
        public DateTime RequestAt { get; set; }
        public string UserId { get; set; } = string.Empty;

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
