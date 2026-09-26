namespace GeoaiApi.Models.Domain;

/// <summary>ジョブ状態（spec.md 7.2）。</summary>
public enum JobStatus
{
    Queued,
    Fetching,
    Preprocessing,
    Training,
    Predicting,
    Publishing,
    Completed,
    Failed,
}

public static class JobStatusExtensions
{
    private static readonly IReadOnlyDictionary<JobStatus, string> Values = new Dictionary<JobStatus, string>
    {
        [JobStatus.Queued] = "queued",
        [JobStatus.Fetching] = "fetching",
        [JobStatus.Preprocessing] = "preprocessing",
        [JobStatus.Training] = "training",
        [JobStatus.Predicting] = "predicting",
        [JobStatus.Publishing] = "publishing",
        [JobStatus.Completed] = "completed",
        [JobStatus.Failed] = "failed",
    };

    /// <summary>
    /// 次に進める状態。contracts/domain.json と一致することをテストで確認する。
    /// queued → preprocessing と preprocessing → completed は import_labels 専用（spec.md 7.2 の注）。
    /// </summary>
    private static readonly IReadOnlyDictionary<JobStatus, JobStatus[]> NextStatuses = new Dictionary<JobStatus, JobStatus[]>
    {
        [JobStatus.Queued] = [JobStatus.Fetching, JobStatus.Preprocessing, JobStatus.Failed],
        [JobStatus.Fetching] = [JobStatus.Preprocessing, JobStatus.Failed],
        [JobStatus.Preprocessing] = [JobStatus.Training, JobStatus.Predicting, JobStatus.Completed, JobStatus.Failed],
        [JobStatus.Training] = [JobStatus.Predicting, JobStatus.Failed],
        [JobStatus.Predicting] = [JobStatus.Publishing, JobStatus.Failed],
        [JobStatus.Publishing] = [JobStatus.Completed, JobStatus.Failed],
        [JobStatus.Completed] = [],
        [JobStatus.Failed] = [],
    };

    /// <summary>DB・API で使う文字列（例：queued）。</summary>
    public static string ToValue(this JobStatus status) => Values[status];

    public static JobStatus ParseJobStatus(string value) =>
        Values.Single(kv => kv.Value == value).Key;

    public static IReadOnlyList<JobStatus> Next(this JobStatus status) => NextStatuses[status];

    public static bool CanTransitionTo(this JobStatus from, JobStatus to) => NextStatuses[from].Contains(to);
}
