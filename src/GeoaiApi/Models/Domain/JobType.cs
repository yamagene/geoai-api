namespace GeoaiApi.Models.Domain;

/// <summary>ジョブ種別（spec.md 7.3）。</summary>
public enum JobType
{
    TrainAndPredict,
    PredictOnly,
    ImportLabels,
}

public static class JobTypeExtensions
{
    private static readonly IReadOnlyDictionary<JobType, string> Values = new Dictionary<JobType, string>
    {
        [JobType.TrainAndPredict] = "train_and_predict",
        [JobType.PredictOnly] = "predict_only",
        [JobType.ImportLabels] = "import_labels",
    };

    /// <summary>DB・API で使う文字列（例：train_and_predict）。</summary>
    public static string ToValue(this JobType type) => Values[type];

    public static JobType ParseJobType(string value) =>
        Values.Single(kv => kv.Value == value).Key;
}
