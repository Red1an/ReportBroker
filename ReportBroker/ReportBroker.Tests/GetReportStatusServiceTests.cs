using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ReportBroker.Application.DTOs;
using ReportBroker.Application.Interfaces;
using ReportBroker.Application.Services;
using ReportBroker.Domain.Entities;
using ReportBroker.Domain.Exceptions;
using ReportBroker.Domain.Interfaces;

namespace ReportBroker.Tests.UseCases;

public class GetReportStatusUseCaseTests
{
    private readonly Mock<IReportRequestRepository> _requestRepoMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly GetReportStatusService _service;

    public GetReportStatusUseCaseTests()
    {
        _requestRepoMock = new Mock<IReportRequestRepository>();
        _cacheMock = new Mock<ICacheService>();

        _service = new GetReportStatusService(
            _requestRepoMock.Object,
            _cacheMock.Object,
            NullLogger<GetReportStatusService>.Instance);
    }

    [Fact]
    public async Task Execute_WhenStatusInCache_ReturnsCachedResult()
    {
        var requestId = Guid.NewGuid();
        var cachedDto = new ReportStatusDto
        {
            RequestId = requestId,
            Status = "Completed"
        };

        _cacheMock
            .Setup(c => c.GetAsync<ReportStatusDto>(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedDto);

        var result = await _service.ExecuteAsync(requestId);

        Assert.Equal("Completed", result.Status);

        _requestRepoMock.Verify(r =>
            r.GetByIdAsync(It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Execute_WhenCacheMiss_GoesToDatabase()
    {
        var parameters = ReportParametersHelper.CreateDefault();
        var report = Report.Create(parameters);

        var request = ReportRequest.Create(report.Id, "user-1");

        typeof(ReportRequest)
            .GetProperty(nameof(ReportRequest.Report))!
            .SetValue(request, report);

        _cacheMock
            .Setup(c => c.GetAsync<ReportStatusDto>(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))!
            .ReturnsAsync((ReportStatusDto?)null);

        _requestRepoMock
            .Setup(r => r.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        var result = await _service.ExecuteAsync(request.Id);

        Assert.Equal(request.Id, result.RequestId);

        _cacheMock.Verify(c =>
            c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<ReportStatusDto>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Execute_WhenRequestNotFound_ThrowsNotFoundException()
    {
        var requestId = Guid.NewGuid();

        _cacheMock
            .Setup(c => c.GetAsync<ReportStatusDto>(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))!
            .ReturnsAsync((ReportStatusDto?)null);

        _requestRepoMock
            .Setup(r => r.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReportRequest?)null);

        await Assert.ThrowsAsync<ReportRequestNotFoundException>(() =>
            _service.ExecuteAsync(requestId));
    }
}