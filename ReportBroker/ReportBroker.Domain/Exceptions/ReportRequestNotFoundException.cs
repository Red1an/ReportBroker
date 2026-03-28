using System;
using System.Collections.Generic;
using System.Text;

namespace ReportBroker.Domain.Exceptions
{
    public class ReportRequestNotFoundException : Exception
    {
        public ReportRequestNotFoundException(Guid requestId)
            : base($"Request with id {requestId} is not found") { }
    }
}
