namespace GeoaiApi.Models.Domain;

/// <summary>分類クラス（spec.md 7.1）。</summary>
/// <param name="Code">class_code</param>
/// <param name="Name">英語名</param>
/// <param name="LabelJa">画面表示用の日本語名</param>
/// <param name="Color">表示色（#RRGGBB または #RRGGBBAA）</param>
/// <param name="SystemOnly">システム専用（教師データで使えない）</param>
public sealed record LandCoverClass(int Code, string Name, string LabelJa, string Color, bool SystemOnly);

/// <summary>分類クラスの一覧。contracts/domain.json と一致することをテストで確認する。</summary>
public static class LandCoverClasses
{
    public static readonly IReadOnlyList<LandCoverClass> All =
    [
        new(0, "unclassified", "未分類", "#00000000", SystemOnly: true),
        new(1, "paddy", "水田", "#4FC3F7", SystemOnly: false),
        new(2, "cropland", "畑", "#FFD54F", SystemOnly: false),
        new(3, "forest", "森林", "#2E7D32", SystemOnly: false),
        new(4, "urban", "市街地・建物", "#E53935", SystemOnly: false),
        new(5, "water", "水域", "#1565C0", SystemOnly: false),
        new(6, "grassland", "草地・荒地", "#A1887F", SystemOnly: false),
    ];

    /// <summary>教師データで使えるクラス（システム専用を除く）。</summary>
    public static IEnumerable<LandCoverClass> Labelable => All.Where(c => !c.SystemOnly);
}
