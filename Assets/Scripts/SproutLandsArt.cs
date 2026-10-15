using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Nạp art thật (bộ "Sprout Lands" của Cup Nooble) NGAY LÚC CHẠY, đọc trực tiếp các
/// file PNG trong Assets/StreamingAssets/SproutLands/.
///
/// Nhờ đọc lúc chạy nên KHÔNG cần đóng Unity, KHÔNG cần tự cắt sprite trong Sprite Editor,
/// KHÔNG cần chỉnh import settings — chỉ cần Unity biên dịch lại script là chạy được.
///
/// Thiếu file ảnh thì các hàm trả về null, và nơi gọi sẽ tự quay về sprite placeholder cũ.
/// </summary>
public static class SproutLandsArt
{
    // Đồng nhất cả game: 16px = 1 ô (1 đơn vị world). Dùng chung cho mọi sprite để
    // các pixel có cùng kích thước -> tỉ lệ pixel-art không bị lệch.
    private const float PixelsPerUnit = 16f;

    private static readonly Dictionary<string, Texture2D> textureCache = new();
    private static readonly Dictionary<string, Sprite> spriteCache = new();
    private static readonly Dictionary<string, Texture2D> iconCache = new();

    private static string Folder => Path.Combine(Application.streamingAssetsPath, "SproutLands");

    /// <summary>Có art thật để dùng không (file ảnh chính có tồn tại không).</summary>
    public static bool Available => File.Exists(Path.Combine(Folder, "grass.png"));

    private static Texture2D LoadTexture(string file)
    {
        if (textureCache.TryGetValue(file, out var cached)) return cached;

        string path = Path.Combine(Folder, file);
        Texture2D tex = null;
        if (File.Exists(path))
        {
            var bytes = File.ReadAllBytes(path);
            tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (tex.LoadImage(bytes)) // tự đổi kích thước theo ảnh
            {
                tex.filterMode = FilterMode.Point;      // nét, không mờ (pixel-art)
                tex.wrapMode = TextureWrapMode.Clamp;
            }
            else
            {
                Object.Destroy(tex);
                tex = null;
            }
        }

        textureCache[file] = tex;
        return tex;
    }

    /// <summary>
    /// Cắt 1 ô vuông kích thước <paramref name="tile"/> px ở cột/hàng (tính từ góc TRÊN-TRÁI)
    /// ra thành Sprite. Trả về null nếu thiếu file hoặc toạ độ vượt ảnh.
    /// </summary>
    public static Sprite Slice(string file, int col, int row, int tile)
    {
        string key = $"{file}:{col}:{row}:{tile}";
        if (spriteCache.TryGetValue(key, out var cachedSprite)) return cachedSprite;

        var tex = LoadTexture(file);
        Sprite sprite = null;
        if (tex != null)
        {
            // PNG: hàng 0 ở TRÊN cùng; còn texture Unity: y=0 ở DƯỚI
            // -> đổi "hàng tính từ trên" sang "y tính từ dưới".
            int x = col * tile;
            int y = tex.height - (row + 1) * tile;
            if (x >= 0 && y >= 0 && x + tile <= tex.width && y + tile <= tex.height)
            {
                sprite = Sprite.Create(tex, new Rect(x, y, tile, tile),
                    new Vector2(0.5f, 0.5f), PixelsPerUnit);
            }
        }

        spriteCache[key] = sprite;
        return sprite;
    }

    // --- Tiện ích cho từng loại art (toạ độ đã chọn sẵn từ các sprite sheet) ---

    /// <summary>Ô cỏ đầy (xanh trơn).</summary>
    public static Sprite Grass() => Slice("grass.png", 1, 1, 16);

    /// <summary>Ô đất đã cuốc (nâu trơn).</summary>
    public static Sprite Tilled() => Slice("tilled.png", 0, 5, 16);

    /// <summary>Rương vàng (dùng làm cái tiệm). Khung 48x48.</summary>
    public static Sprite Chest() => Slice("chest.png", 0, 0, 48);

    /// <summary>
    /// 4 giai đoạn lớn của cây ở HÀNG <paramref name="row"/> trong plants.png
    /// (mầm -> non -> ra nụ -> chín). Mỗi hàng là 1 loại cây khác nhau.
    /// </summary>
    public static Sprite[] CropStages(int row)
    {
        var stages = new[]
        {
            Slice("plants.png", 1, row, 16),
            Slice("plants.png", 2, row, 16),
            Slice("plants.png", 3, row, 16),
            Slice("plants.png", 4, row, 16),
        };
        foreach (var s in stages)
            if (s == null) return null; // thiếu 1 giai đoạn -> dùng placeholder cho đồng bộ
        return stages;
    }

    /// <summary>
    /// 1 khung nhân vật. Khung 48x48, lưới 4x4:
    ///   hàng (dir): 0 = nhìn xuống, 1 = nhìn lên, 2 = nhìn phải, 3 = nhìn trái
    ///   cột (frame): 0..3 = các khung animation đi bộ
    /// </summary>
    public static Sprite Character(int dir, int frame) => Slice("character.png", frame, dir, 48);

    /// <summary>Các hình trang trí nhỏ rải trên cỏ (cỏ, hoa vàng, hoa hồng, chùm đỏ, nấm).</summary>
    public static Sprite[] DecorSprites()
    {
        var picks = new[] { (5, 1), (6, 2), (6, 3), (0, 2), (3, 3) };
        var list = new List<Sprite>();
        foreach (var p in picks)
        {
            var s = Slice("decor.png", p.Item1, p.Item2, 16);
            if (s != null) list.Add(s);
        }
        return list.Count > 0 ? list.ToArray() : null;
    }

    // --- Icon dạng Texture2D riêng (để HUD vẽ bằng OnGUI / GUI.DrawTexture) ---

    /// <summary>Cắt 1 ô ra thành Texture2D độc lập (ô 16px). null nếu thiếu file/toạ độ sai.</summary>
    public static Texture2D IconTexture(string file, int col, int row, int tile)
    {
        string key = $"{file}:{col}:{row}:{tile}";
        if (iconCache.TryGetValue(key, out var cached)) return cached;

        var tex = LoadTexture(file);
        Texture2D icon = null;
        if (tex != null)
        {
            int x = col * tile;
            int y = tex.height - (row + 1) * tile;
            if (x >= 0 && y >= 0 && x + tile <= tex.width && y + tile <= tex.height)
            {
                var pixels = tex.GetPixels(x, y, tile, tile); // tex do LoadImage tạo -> đọc được
                icon = new Texture2D(tile, tile, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
                icon.SetPixels(pixels);
                icon.Apply();
            }
        }

        iconCache[key] = icon;
        return icon;
    }

    /// <summary>Icon túi hạt giống của loại cây ở HÀNG <paramref name="row"/> (cho HUD).</summary>
    public static Texture2D SeedIconTex(int row) => IconTexture("plants.png", 0, row, 16);

    /// <summary>Icon nông sản thu hoạch của loại cây ở HÀNG <paramref name="row"/> (cho HUD).</summary>
    public static Texture2D ProduceIconTex(int row) => IconTexture("plants.png", 5, row, 16);
}
