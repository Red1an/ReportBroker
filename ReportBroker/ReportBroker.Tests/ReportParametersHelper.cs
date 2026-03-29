using ReportBroker.Domain.ValueObjects;

namespace ReportBroker.Tests;

public static class ReportParametersHelper
{
    public static ReportParameters CreateDefault() =>
        new ReportParameters(
            ProductId: Guid.NewGuid(),
            DesignId: Guid.NewGuid(),
            PeriodStart: new DateOnly(2024, 1, 1),
            PeriodEnd: new DateOnly(2024, 1, 31));
}