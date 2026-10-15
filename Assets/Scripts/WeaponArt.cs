using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Nạp art VŨ KHÍ lúc chạy từ Assets/StreamingAssets/Weapons/ (bộ "16x16 Weapons RPG Icons").
/// Hiện dùng cho cây KIẾM người chơi vung khi chém. Thiếu file -> trả null, nơi gọi tự fallback
/// về vệt chém vẽ bằng code.
///
/// ⚠ LICENSE: bộ icon vũ khí này KHÔNG kèm file bản quyền. Dùng tạm cho game HỌC TẬP / CÁ NHÂN.
/// Nếu sau này phát hành thì phải xác minh bản quyền hoặc thay bằng art khác.
/// </summary>
public static class WeaponArt
{
    private const float PixelsPerUnit = 16f;

    private static readonly Dictionary<string, Texture2D> textureCache = new();
    private static Sprite swordSprite;
    private static bool swordTried;

    private static string Folder => Path.Combine(Application.streamingAssetsPath, "Weapons");

    /// <summary>
    /// Cây kiếm sắt. Tâm xoay (pivot) đặt ở CHUÔI (góc dưới-trái) để vung quanh tay;
    /// lưỡi kiếm gốc chỉ hướng Đông-Bắc (~45°).
    /// </summary>
    public static Sprite Sword()
    {
        if (swordTried) return swordSprite;
        swordTried = true;
        swordSprite = LoadSprite("sword_iron.png", new Vector2(0.18f, 0.18f));
        return swordSprite;
    }

    /// <summary>Texture kiếm để vẽ icon trên HUD.</summary>
    public static Texture2D SwordIconTex() => LoadTexture("sword_iron.png");

    private static Sprite LoadSprite(string file, Vector2 pivot)
    {
        var tex = LoadTexture(file);
        if (tex == null) return null;
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivot, PixelsPerUnit);
    }

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
}
