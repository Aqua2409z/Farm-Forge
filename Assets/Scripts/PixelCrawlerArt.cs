using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Nạp art bộ "Pixel Crawler" (của Anokolisa) LÚC CHẠY, từ
/// Assets/StreamingAssets/PixelCrawler/ — giống cách làm với Sprout Lands.
/// Dùng cho QUÁI (orc, skeleton). Thiếu file thì trả null, nơi gọi tự dùng placeholder.
///
/// License (Terms.txt): dùng cho mọi dự án kể cả thương mại, được chỉnh sửa;
/// KHÔNG bán lại bộ art. Credit không bắt buộc nhưng được khuyến khích.
/// </summary>
public static class PixelCrawlerArt
{
    private const float PixelsPerUnit = 16f;

    private static readonly Dictionary<string, Texture2D> textureCache = new();
    private static readonly Dictionary<string, Sprite> spriteCache = new();

    private static string Folder => Path.Combine(Application.streamingAssetsPath, "PixelCrawler");

    public static bool Available => File.Exists(Path.Combine(Folder, "orc_idle.png"));

    private static Texture2D LoadTexture(string file)
    {
        if (textureCache.TryGetValue(file, out var cached)) return cached;

        string path = Path.Combine(Folder, file);
        Texture2D tex = null;
        if (File.Exists(path))
        {
            var bytes = File.ReadAllBytes(path);
            tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (tex.LoadImage(bytes))
            {
                tex.filterMode = FilterMode.Point;
                tex.wrapMode = TextureWrapMode.Clamp;
            }
            else { Object.Destroy(tex); tex = null; }
        }

        textureCache[file] = tex;
        return tex;
    }

    /// <summary>Cắt 1 khung (frameW x frameH) ở cột/hàng tính từ góc TRÊN-TRÁI.</summary>
    public static Sprite Slice(string file, int col, int row, int frameW, int frameH)
    {
        string key = $"{file}:{col}:{row}:{frameW}x{frameH}";
        if (spriteCache.TryGetValue(key, out var cachedSprite)) return cachedSprite;

        var tex = LoadTexture(file);
        Sprite sprite = null;
        if (tex != null)
        {
            int x = col * frameW;
            int y = tex.height - (row + 1) * frameH; // PNG hàng 0 ở trên; texture y=0 ở dưới
            if (x >= 0 && y >= 0 && x + frameW <= tex.width && y + frameH <= tex.height)
            {
                sprite = Sprite.Create(tex, new Rect(x, y, frameW, frameH),
                    new Vector2(0.5f, 0.5f), PixelsPerUnit);
            }
        }

        spriteCache[key] = sprite;
        return sprite;
    }

    /// <summary>Lấy <paramref name="count"/> khung liên tiếp ở hàng 0 (khung vuông).</summary>
    private static Sprite[] RowFrames(string file, int count, int frame)
    {
        var arr = new Sprite[count];
        for (int i = 0; i < count; i++)
        {
            arr[i] = Slice(file, i, 0, frame, frame);
            if (arr[i] == null) return null; // thiếu khung -> coi như không có art
        }
        return arr;
    }

    /// <summary>4 khung đứng (idle) của Orc (32x32).</summary>
    public static Sprite[] OrcFrames() => RowFrames("orc_idle.png", 4, 32);

    /// <summary>4 khung đứng (idle) của Skeleton (32x32).</summary>
    public static Sprite[] SkeletonFrames() => RowFrames("skeleton_idle.png", 4, 32);
}
