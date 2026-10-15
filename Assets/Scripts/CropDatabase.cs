using UnityEngine;

/// <summary>
/// Story 13: Danh sách CÁC LOẠI cây trồng trong game. Dựng 1 lần rồi dùng chung
/// ở mọi nơi (gieo hạt, bán hàng, HUD, lưu game).
///
/// Muốn thêm loại cây mới: thêm 1 dòng Make(...) trong Build(), trỏ tới 1 hàng khác
/// của plants.png (bộ art có 2 hàng cây). Thiếu art thì tự dùng ô vuông placeholder.
/// </summary>
public static class CropDatabase
{
    private static CropType[] all;

    public static CropType[] All => all ??= Build();
    public static int Count => All.Length;

    /// <summary>Lấy loại cây theo id (tự kẹp trong khoảng hợp lệ cho an toàn).</summary>
    public static CropType Get(int id) => All[Mathf.Clamp(id, 0, All.Length - 1)];

    private static CropType[] Build()
    {
        return new[]
        {
            Make(0, "Củ vàng",    row: 0, sell: 10, seedBuy: 5, cropColor: new Color(0.95f, 0.78f, 0.20f)),
            Make(1, "Củ cải tím", row: 1, sell: 18, seedBuy: 8, cropColor: new Color(0.72f, 0.35f, 0.75f)),
        };
    }

    private static CropType Make(int id, string name, int row, int sell, int seedBuy, Color cropColor)
    {
        return new CropType
        {
            Id = id,
            Name = name,
            Row = row,
            SellPrice = sell,
            SeedBuyPrice = seedBuy,
            StageSprites = SproutLandsArt.CropStages(row) ?? FallbackStages(cropColor),
            SeedIcon = SproutLandsArt.SeedIconTex(row),
            ProduceIcon = SproutLandsArt.ProduceIconTex(row),
        };
    }

    /// <summary>Thiếu art thật: ô vuông to dần, hai giai đoạn cuối mang màu đặc trưng của cây.</summary>
    private static Sprite[] FallbackStages(Color cropColor)
    {
        var dark = new Color(cropColor.r * 0.8f, cropColor.g * 0.8f, cropColor.b * 0.8f);
        return new[]
        {
            PlaceholderSprites.CreateCenteredSquare(new Color(0.55f, 0.85f, 0.35f), 4),
            PlaceholderSprites.CreateCenteredSquare(new Color(0.45f, 0.78f, 0.28f), 8),
            PlaceholderSprites.CreateCenteredSquare(dark, 11),
            PlaceholderSprites.CreateCenteredSquare(cropColor, 12),
        };
    }
}
