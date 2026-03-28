using System;
using System.Collections.Generic;
using System.Text;

namespace ReportBroker.Application.DTOs
{   
    public class CreateReportRequestDto
    {
        public string UserId { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public Guid DesignId { get; set; }
        public DateOnly PeriodStart { get; set; }
        public DateOnly PeriodEnd { get; set; }
    }
}
