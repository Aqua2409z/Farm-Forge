using UnityEngine;

/// <summary>
/// Sinh sprite ô vuông đơn giản bằng code, để game chạy được khi CHƯA có art thật.
/// Sau này khi thay sprite ansimuz vào thì có thể bỏ dần các hàm này.
/// </summary>
public static class PlaceholderSprites
{
    private const int Size = 16; // 16px = 1 đơn vị world (khớp pixels-per-unit)

    /// <summary>Ô vuông đặc có viền sẫm hơn — để nhìn thấy đường lưới.</summary>
    public static Sprite CreateBorderedSquare(Color fill, Color border)
    {
        var tex = NewTexture();
        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                bool edge = x == 0 || y == 0 || x == Size - 1 || y == Size - 1;
                tex.SetPixel(x, y, edge ? border : fill);
            }
        }
        return Finish(tex);
    }

    /// <summary>Ô vuông rỗng ruột (chỉ có viền) — dùng làm khung highlight ô đang nhắm.</summary>
    public static Sprite CreateHollowSquare(Color color, int thickness)
    {
        var tex = NewTexture();
        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                bool edge = x < thickness || y < thickness
                          || x >= Size - thickness || y >= Size - thickness;
                tex.SetPixel(x, y, edge ? color : Color.clear);
            }
        }
        return Finish(tex);
    }

    /// <summary>Ô vuông đặc một màu.</summary>
    public static Sprite CreateSolidSquare(Color color)
    {
        var tex = NewTexture();
        var pixels = new Color[Size * Size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        return Finish(tex);
    }

    /// <summary>
    /// Nền trong suốt với một khối vuông nhỏ ở GIỮA.
    /// Dùng vẽ cây theo giai đoạn: innerSize càng lớn = cây càng lớn.
    /// </summary>
    public static Sprite CreateCenteredSquare(Color color, int innerSize)
    {
        var tex = NewTexture();
        var clear = new Color(0f, 0f, 0f, 0f);
        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                tex.SetPixel(x, y, clear);

        int start = (Size - innerSize) / 2;
        for (int x = start; x < start + innerSize; x++)
            for (int y = start; y < start + innerSize; y++)
                tex.SetPixel(x, y, color);

        return Finish(tex);
    }

    private static Texture2D NewTexture()
    {
        var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point; // nét, không mờ (pixel-art)
        return tex;
    }

    private static Sprite Finish(Texture2D tex)
    {
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f), Size);
    }
}
