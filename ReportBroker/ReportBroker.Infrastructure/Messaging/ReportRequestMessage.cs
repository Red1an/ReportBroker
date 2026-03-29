
namespace ReportBroker.Infrastructure.Messaging
{
    public record ReportRequestMessage(
        string UserId,
        Guid ProductId,
        Guid DesignId,
        DateOnly PeriodStart,
        DateOnly PeriodEnd); 
}
