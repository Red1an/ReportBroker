using System;
using System.Collections.Generic;
using System.Text;

namespace ReportBroker.Domain.ValueObjects
{
    public record ReportParameters(
        Guid ProductId,
        Guid DesignId,
        DateOnly PeriodStart,
        DateOnly PeriodEnd)
    {        
        public void Validate()
        {
            if (ProductId == Guid.Empty)
                throw new ArgumentException("ProductId cant be null");

            if (DesignId == Guid.Empty)
                throw new ArgumentException("DesignId cant be null");

            if (PeriodStart >= PeriodEnd)
                throw new ArgumentException("Start date must be earlier than end date");
        }
    }
}
