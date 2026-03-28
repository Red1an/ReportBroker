using ReportBroker.Domain.Enums;
using ReportBroker.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportBroker.Domain.Entities
{
    public class Report
    {
        public Guid Id { get; private set; }
        public ReportParameters Parameters { get; private set; } = null!;
        public ReportStatus Status { get; private set; }
        public int? ViewCount { get; private set; }
        public double? ConversionRatio {  get; private set; }
        public int? PaymentCount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime CompletedAt { get; private set; }

        private readonly List<ReportRequest> _requests = new();
        public IReadOnlyCollection<ReportRequest> Requests => _requests.AsReadOnly();

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

        public void Complete(int viewCount, int paymentCount)
        {
            if (Status != ReportStatus.Processing)
            {
                throw new InvalidOperationException($"Cant finish processing with status {Status}");
            }

            ViewCount = viewCount;
            PaymentCount = paymentCount;
            ConversionRatio = viewCount > 0
            ? Math.Round((double)paymentCount / viewCount * 100, 2)
            : 0;

            Status = ReportStatus.Complited;
            CompletedAt = DateTime.UtcNow;
        }

        public void Fail()
        {
            CompletedAt = DateTime.UtcNow;
            Status = ReportStatus.Failed;
        }
    }

}
