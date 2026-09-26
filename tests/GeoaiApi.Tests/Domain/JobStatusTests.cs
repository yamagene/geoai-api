using GeoaiApi.Models.Domain;

namespace GeoaiApi.Tests.Domain;

public class JobStatusTests
{
    [Theory]
    [InlineData(JobStatus.Queued, JobStatus.Fetching, true)]
    [InlineData(JobStatus.Queued, JobStatus.Preprocessing, true)]
    [InlineData(JobStatus.Preprocessing, JobStatus.Completed, true)]
    [InlineData(JobStatus.Publishing, JobStatus.Completed, true)]
    [InlineData(JobStatus.Training, JobStatus.Failed, true)]
    [InlineData(JobStatus.Queued, JobStatus.Completed, false)]
    [InlineData(JobStatus.Fetching, JobStatus.Queued, false)]
    [InlineData(JobStatus.Completed, JobStatus.Failed, false)]
    [InlineData(JobStatus.Failed, JobStatus.Queued, false)]
    public void CanTransitionTo(JobStatus from, JobStatus to, bool expected)
    {
        Assert.Equal(expected, from.CanTransitionTo(to));
    }

    [Theory]
    [InlineData("queued", JobStatus.Queued)]
    [InlineData("preprocessing", JobStatus.Preprocessing)]
    public void ParseJobStatus_RoundTrips(string value, JobStatus status)
    {
        Assert.Equal(status, JobStatusExtensions.ParseJobStatus(value));
        Assert.Equal(value, status.ToValue());
    }

    [Fact]
    public void ParseJobStatus_RejectsUnknownValue()
    {
        Assert.ThrowsAny<InvalidOperationException>(() => JobStatusExtensions.ParseJobStatus("running"));
    }
}
