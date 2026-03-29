
namespace ReportBroker.Infrastructure.Messaging
{
    public class ReportRequestMessage
    {
        public string UserId { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public Guid DesignId { get; set; }
        public DateOnly PeriodStart { get; set; }
        public DateOnly PeriodEnd { get; set; }
    }
}
