using System.Text.Json;
using GeoaiApi.Models.Domain;

namespace GeoaiApi.Tests.Domain;

/// <summary>
/// コード上のドメイン定義が contracts/domain.json（契約）と一致することを確認する（spec.md 4.6、T-02）。
/// </summary>
public class ContractTests
{
    private static readonly JsonElement Domain = LoadDomain();

    private static JsonElement LoadDomain()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "contracts", "domain.json");
        using var doc = JsonDocument.Parse(File.ReadAllText(path));
        return doc.RootElement.Clone();
    }

    [Fact]
    public void Classes_MatchContract()
    {
        var expected = Domain.GetProperty("classes").EnumerateArray()
            .Select(c => new LandCoverClass(
                c.GetProperty("code").GetInt32(),
                c.GetProperty("name").GetString()!,
                c.GetProperty("labelJa").GetString()!,
                c.GetProperty("color").GetString()!,
                c.GetProperty("systemOnly").GetBoolean()))
            .ToList();

        Assert.Equal(expected, LandCoverClasses.All);
    }

    [Fact]
    public void JobStatuses_MatchContract()
    {
        var expected = Domain.GetProperty("jobStatuses").EnumerateArray()
            .Select(s => s.GetProperty("value").GetString()!)
            .ToList();

        Assert.Equal(expected, Enum.GetValues<JobStatus>().Select(s => s.ToValue()).ToList());
    }

    [Fact]
    public void JobStatusTransitions_MatchContract()
    {
        foreach (var s in Domain.GetProperty("jobStatuses").EnumerateArray())
        {
            var status = JobStatusExtensions.ParseJobStatus(s.GetProperty("value").GetString()!);
            var expectedNext = s.GetProperty("next").EnumerateArray().Select(n => n.GetString()!).ToList();

            Assert.Equal(expectedNext, status.Next().Select(n => n.ToValue()).ToList());
        }
    }

    [Fact]
    public void JobTypes_MatchContract()
    {
        var expected = Domain.GetProperty("jobTypes").EnumerateArray()
            .Select(t => t.GetProperty("value").GetString()!)
            .ToList();

        Assert.Equal(expected, Enum.GetValues<JobType>().Select(t => t.ToValue()).ToList());
    }
}
