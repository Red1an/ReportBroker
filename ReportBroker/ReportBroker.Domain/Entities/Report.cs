using ReportBroker.Domain.Enums;
using ReportBroker.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportBroker.Domain.Entities
{
    public class Report
    {
        public Guid Id { get; set; }
        public ReportParameters Parameters { get; set; } = null!;
        public ReportStatus Status { get; set; }
        public int? ViewCount { get; set; }
        public int? PaymentCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public Report() { }

        public static Report Create(ReportParameters parameters)
        {
            parameters.Validate();
            return new Report
            {
                Id = Guid.NewGuid(),
                Parameters = parameters,
                Status = ReportStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }
       
        public void Processing()
        {
            if (Status != ReportStatus.Pending)
            {
                throw new InvalidOperationException($"Cant start processing with status {Status}");
            }
            Status = ReportStatus.Processing;
        }

        public void Complete()
        {
            if (Status != ReportStatus.Processing)
            {
                throw new InvalidOperationException($"Cant stop processing with status {Status}");
            }
            Status = ReportStatus.Complited;
        }

        public void Fail()
        {
            Status = ReportStatus.Failed;
        }
    }

}
