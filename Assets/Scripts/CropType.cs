using UnityEngine;

/// <summary>
/// Story 13: Mô tả MỘT loại cây trồng (vd: củ vàng, củ cải tím).
/// Chỉ là dữ liệu thuần (không phải MonoBehaviour) — do CropDatabase tạo sẵn.
/// </summary>
public class CropType
{
    /// <summary>Số thứ tự loại cây (0, 1, ...). Dùng để lưu game & đếm trong túi đồ.</summary>
    public int Id;

    /// <summary>Tên hiển thị (tiếng Việt).</summary>
    public string Name;

    /// <summary>Hàng trong plants.png (mỗi loại cây 1 hàng).</summary>
    public int Row;

    /// <summary>Giá bán 1 nông sản của loại này.</summary>
    public int SellPrice;

    /// <summary>Giá mua 1 hạt giống của loại này.</summary>
    public int SeedBuyPrice;

    /// <summary>4 sprite cho 4 giai đoạn lớn.</summary>
    public Sprite[] StageSprites;

    /// <summary>Icon túi hạt (HUD) — có thể null nếu thiếu art.</summary>
    public Texture2D SeedIcon;

    /// <summary>Icon nông sản (HUD) — có thể null nếu thiếu art.</summary>
    public Texture2D ProduceIcon;
}
