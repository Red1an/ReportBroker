using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ReportBroker.Application.DTOs;
using ReportBroker.Application.Services;
using ReportBroker.Domain.Entities;
using ReportBroker.Domain.Interfaces;
using ReportBroker.Domain.ValueObjects;

namespace ReportBroker.Tests.UseCases;

public class CreateReportRequestServiceTests
{
    private readonly Mock<IReportRepository> _reportRepoMock;
    private readonly Mock<IReportRequestRepository> _requestRepoMock;
    private readonly CreateReportRequestService _service;

    public CreateReportRequestServiceTests()
    {
        _reportRepoMock = new Mock<IReportRepository>();
        _requestRepoMock = new Mock<IReportRequestRepository>();

        _service = new CreateReportRequestService(
            _reportRepoMock.Object,
            _requestRepoMock.Object,
            NullLogger<CreateReportRequestService>.Instance);
    }

    [Fact]
    public async Task Execute_WhenReportNotExists_CreatesNewReport()
    {
        var parameters = ReportParametersHelper.CreateDefault();
        var dto = CreateDto(parameters);

        _reportRepoMock
            .Setup(r => r.GetByParametersAsync(
                It.IsAny<ReportParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Report?)null);

        _reportRepoMock
            .Setup(r => r.AddAsync(
                It.IsAny<Report>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Report r, CancellationToken _) => r);

        _requestRepoMock
            .Setup(r => r.AddAsync(
                It.IsAny<ReportRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReportRequest r, CancellationToken _) => r);

        var result = await _service.ExecuteAsync(dto);

        _reportRepoMock.Verify(r =>
            r.AddAsync(It.IsAny<Report>(),
            It.IsAny<CancellationToken>()),
            Times.Once);

        _requestRepoMock.Verify(r =>
            r.AddAsync(It.IsAny<ReportRequest>(),
            It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task Execute_WhenReportAlreadyExists_DoesNotCreateNewReport()
    {
        var parameters = ReportParametersHelper.CreateDefault();
        var dto = CreateDto(parameters);

        var existingReport = Report.Create(parameters);

        _reportRepoMock
            .Setup(r => r.GetByParametersAsync(
                It.IsAny<ReportParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReport);

        _requestRepoMock
            .Setup(r => r.AddAsync(
                It.IsAny<ReportRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReportRequest r, CancellationToken _) => r);

        var result = await _service.ExecuteAsync(dto);

        _reportRepoMock.Verify(r =>
            r.AddAsync(
                It.IsAny<Report>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _requestRepoMock.Verify(r =>
            r.AddAsync(
                It.IsAny<ReportRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Execute_WhenTwoUsersRequestSameReport_BothGetSameReportId()
    {
        var parameters = ReportParametersHelper.CreateDefault();
        var dto1 = CreateDto(parameters, userId: "user-1");
        var dto2 = CreateDto(parameters, userId: "user-2");

        var existingReport = Report.Create(parameters);

        _reportRepoMock
            .SetupSequence(r => r.GetByParametersAsync(
                It.IsAny<ReportParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Report?)null)
            .ReturnsAsync(existingReport);

        _reportRepoMock
            .Setup(r => r.AddAsync(
                It.IsAny<Report>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReport);

        _requestRepoMock
            .Setup(r => r.AddAsync(
                It.IsAny<ReportRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReportRequest r, CancellationToken _) => r);

        var result1 = await _service.ExecuteAsync(dto1);
        var result2 = await _service.ExecuteAsync(dto2);

        Assert.Equal(result1.ReportId, result2.ReportId);

        _reportRepoMock.Verify(r =>
            r.AddAsync(
                It.IsAny<Report>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _requestRepoMock.Verify(r =>
            r.AddAsync(
                It.IsAny<ReportRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task Execute_WhenInvalidParameters_ThrowsArgumentException()
    {
        var dto = new CreateReportRequestDto
        {
            UserId = "user-1",
            ProductId = Guid.NewGuid(),
            DesignId = Guid.NewGuid(),
            PeriodStart = new DateOnly(2024, 2, 1),
            PeriodEnd = new DateOnly(2024, 1, 1) 
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.ExecuteAsync(dto));
    }

    [Fact]
    public async Task Execute_WhenEmptyUserId_ThrowsArgumentException()
    {
        var parameters = ReportParametersHelper.CreateDefault();
        var dto = CreateDto(parameters, userId: "");

        _reportRepoMock
            .Setup(r => r.GetByParametersAsync(
                It.IsAny<ReportParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Report?)null);

        _reportRepoMock
            .Setup(r => r.AddAsync(
                It.IsAny<Report>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Report r, CancellationToken _) => r);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.ExecuteAsync(dto));
    }

    private static CreateReportRequestDto CreateDto(
        ReportParameters parameters,
        string userId = "test-user")
    {
        return new CreateReportRequestDto
        {
            UserId = userId,
            ProductId = parameters.ProductId,
            DesignId = parameters.DesignId,
            PeriodStart = parameters.PeriodStart,
            PeriodEnd = parameters.PeriodEnd
        };
    }
}