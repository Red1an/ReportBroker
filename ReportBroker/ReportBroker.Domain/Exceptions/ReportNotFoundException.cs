using System;
using System.Collections.Generic;
using System.Text;

namespace ReportBroker.Domain.Exceptions
{
    public class ReportNotFoundException : Exception
    {
        public ReportNotFoundException(Guid reportId) 
            : base($"Report with id {reportId} is not found"){ }
    }
}
