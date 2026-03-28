using System;
using System.Collections.Generic;
using System.Text;

namespace ReportBroker.Application.DTOs;

public class ReportStatusDto
{
    public Guid RequestId { get; set; }
    public Guid ReportId { get; set; }
    public string Status { get; set; } = string.Empty;
    public double? ConversionRatio { get; set; }
    public int? PaymentCount { get; set; }
    public int? ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
