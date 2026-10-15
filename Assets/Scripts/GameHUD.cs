using UnityEngine;

/// <summary>
/// HUD kiểu game nông trại (OnGUI — chưa cần Canvas):
///   - Trên-TRÁI: nông sản theo loại + hạt giống theo loại (loại ĐANG CHỌN có dấu ►).
///   - Trên-PHẢI: icon đồng tiền + số.
///   - Khi gần tiệm: gợi ý phím bán (E) / mua hạt loại đang chọn (R).
///   - Góc dưới-phải: credit "Cup Nooble" (license yêu cầu).
///
/// Thiếu icon art thì chỉ bớt hình, chữ (tên + số) vẫn hiện bình thường.
/// </summary>
public class GameHUD : MonoBehaviour
{
    private Inventory inventory;
    private Shop shop;
    private SeedSelection selection;
    private Health playerHealth;
    private DayNightCycle dayNight;

    private GUIStyle headerStyle;
    private GUIStyle numberStyle;
    private GUIStyle numberSelStyle;
    private GUIStyle markStyle;
    private GUIStyle coinStyle;
    private GUIStyle hintStyle;
    private GUIStyle phaseStyle;
    private GUIStyle creditStyle;

    private Texture2D coinIcon;
    private Texture2D heartFull;
    private Texture2D heartEmpty;
    private bool iconsLoaded;

    public void Configure(Inventory inventory, Shop shop, SeedSelection selection,
        Health playerHealth, DayNightCycle dayNight)
    {
        this.inventory = inventory;
        this.shop = shop;
        this.selection = selection;
        this.playerHealth = playerHealth;
        this.dayNight = dayNight;
    }

    private void OnGUI()
    {
        if (inventory == null) return;
        EnsureStyles();
        EnsureIcons();

        const float icon = 30f;
        const float pad = 14f;
        const float rowH = icon + 4f;

        int selected = selection != null ? selection.SelectedType : 0;
        float y = 10f;

        // ---- Nông sản theo loại ----
        GUI.Label(new Rect(pad, y, 320f, 22f), "Nông sản", headerStyle);
        y += 24f;
        for (int t = 0; t < CropDatabase.Count; t++)
        {
            var type = CropDatabase.Get(t);
            DrawRow(pad, y, icon, type.ProduceIcon, type.Name, inventory.Crops(t), false);
            y += rowH;
        }

        // ---- Hạt giống theo loại (đánh dấu loại đang chọn) ----
        y += 8f;
        GUI.Label(new Rect(pad, y, 320f, 22f), "Hạt giống  (phím 1 / 2 / Q để đổi)", headerStyle);
        y += 24f;
        for (int t = 0; t < CropDatabase.Count; t++)
        {
            var type = CropDatabase.Get(t);
            DrawRow(pad, y, icon, type.SeedIcon, type.Name, inventory.Seeds(t), t == selected);
            y += rowH;
        }

        // ---- Trên-phải: tiền (icon + số, canh phải) ----
        GUILayout.BeginArea(new Rect(Screen.width - 210f, 10f, 196f, 34f));
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (coinIcon != null) GUILayout.Label(coinIcon, GUILayout.Width(30f), GUILayout.Height(30f));
        GUILayout.Label($"{inventory.Coins}", coinStyle);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        // ---- Trên-giữa: tim (máu) + nhãn Ngày/Đêm ----
        if (playerHealth != null)
        {
            const float hs = 26f;
            float totalW = playerHealth.Max * (hs + 2f);
            float startX = (Screen.width - totalW) / 2f;
            for (int i = 0; i < playerHealth.Max; i++)
            {
                var tex = i < playerHealth.Current ? heartFull : heartEmpty;
                if (tex != null) GUI.DrawTexture(new Rect(startX + i * (hs + 2f), 8f, hs, hs), tex, ScaleMode.ScaleToFit);
            }
        }

        if (dayNight != null)
        {
            string label = $"Ngày {dayNight.DayNumber} — {dayNight.PhaseLabel}";
            phaseStyle.normal.textColor = dayNight.IsNight
                ? new Color(0.6f, 0.7f, 1f) : new Color(1f, 0.95f, 0.5f);
            GUI.Label(new Rect(Screen.width / 2f - 150f, 38f, 300f, 24f), label, phaseStyle);

            // Ban đêm: nhắc phím chém.
            if (dayNight.IsNight)
            {
                GUI.Label(new Rect(Screen.width / 2f - 260f, Screen.height - 74f, 520f, 26f),
                    "Ban đêm! Quái xuất hiện — Chuột trái: vung kiếm chém", hintStyle);
            }
        }

        // ---- Gần tiệm: gợi ý mua bán ----
        if (shop != null && shop.PlayerInRange)
        {
            var sel = CropDatabase.Get(selected);
            string hint = $"[{shop.SellKey}] Bán tất cả nông sản       " +
                          $"[{shop.BuyKey}] Mua hạt {sel.Name} (-{sel.SeedBuyPrice})";
            float hw = 820f;
            GUI.Label(new Rect((Screen.width - hw) / 2f, Screen.height - 46f, hw, 30f), hint, hintStyle);
        }

        // ---- Credit bắt buộc theo license bộ art ----
        float cw = 300f;
        GUI.Label(new Rect(Screen.width - cw - 8f, Screen.height - 24f, cw, 20f),
            "Art: Sprout Lands — Cup Nooble", creditStyle);
    }

    /// <summary>Vẽ 1 dòng: [►] [icon] "xN  Tên". selected = tô vàng + dấu ►.</summary>
    private void DrawRow(float x, float y, float icon, Texture2D tex, string name, int count, bool selected)
    {
        if (selected)
            GUI.Label(new Rect(x - 2f, y, 16f, icon), "►", markStyle);

        float ix = x + 14f;
        if (tex != null)
            GUI.DrawTexture(new Rect(ix, y, icon, icon), tex, ScaleMode.ScaleToFit);

        GUI.Label(new Rect(ix + icon + 6f, y, 300f, icon),
            $"x {count}   {name}", selected ? numberSelStyle : numberStyle);
    }

    private void EnsureIcons()
    {
        if (iconsLoaded) return;
        iconsLoaded = true;
        coinIcon = BuildCoinTexture();
        heartFull = BuildHeart(true);
        heartEmpty = BuildHeart(false);
    }

    private void EnsureStyles()
    {
        if (numberStyle != null) return;

        numberStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
        };
        numberStyle.normal.textColor = Color.white;

        numberSelStyle = new GUIStyle(numberStyle);
        numberSelStyle.normal.textColor = new Color(1f, 0.92f, 0.30f); // vàng: loại đang chọn

        markStyle = new GUIStyle(numberStyle);
        markStyle.normal.textColor = new Color(1f, 0.92f, 0.30f);

        headerStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 15,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
        };
        headerStyle.normal.textColor = new Color(1f, 1f, 1f, 0.75f);

        coinStyle = new GUIStyle(numberStyle) { fontSize = 22 };
        coinStyle.normal.textColor = new Color(1f, 0.92f, 0.30f);

        hintStyle = new GUIStyle(numberStyle) { alignment = TextAnchor.UpperCenter };
        hintStyle.normal.textColor = Color.white;

        phaseStyle = new GUIStyle(numberStyle)
        {
            alignment = TextAnchor.UpperCenter,
            fontSize = 16,
        };
        phaseStyle.normal.textColor = Color.white;

        creditStyle = new GUIStyle(numberStyle)
        {
            alignment = TextAnchor.LowerRight,
            fontSize = 12,
            fontStyle = FontStyle.Normal,
        };
        creditStyle.normal.textColor = new Color(1f, 1f, 1f, 0.6f);
    }

    /// <summary>Vẽ 1 đồng tiền vàng tròn nhỏ bằng code (bộ art không có sẵn icon tiền).</summary>
    private Texture2D BuildCoinTexture()
    {
        const int size = 16;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };

        var gold = new Color(1f, 0.82f, 0.20f);
        var goldDark = new Color(0.78f, 0.56f, 0.12f);
        var shine = new Color(1f, 0.96f, 0.72f);

        float c = (size - 1) / 2f;
        const float rOuter = 7.3f, rInner = 6.0f;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float d = Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c));
                Color col;
                if (d > rOuter) col = Color.clear;
                else if (d > rInner) col = goldDark; // viền
                else col = gold;                     // thân
                tex.SetPixel(x, y, col);
            }
        }

        tex.SetPixel(5, 10, shine);
        tex.SetPixel(6, 10, shine);
        tex.SetPixel(5, 9, shine);

        tex.Apply();
        return tex;
    }

    /// <summary>Vẽ 1 trái tim pixel. filled = đỏ (còn máu), ngược lại = xám tối (đã mất).</summary>
    private Texture2D BuildHeart(bool filled)
    {
        const int size = 16;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };

        var body = filled ? new Color(0.90f, 0.18f, 0.24f) : new Color(0.22f, 0.20f, 0.22f);
        var edge = filled ? new Color(0.62f, 0.10f, 0.16f) : new Color(0.14f, 0.13f, 0.14f);
        var shine = new Color(1f, 0.75f, 0.78f);

        // Hàm toán học hình trái tim (toạ độ chuẩn hoá quanh tâm).
        for (int px = 0; px < size; px++)
        {
            for (int py = 0; py < size; py++)
            {
                tex.SetPixel(px, py, Color.clear);

                float x = (px - (size - 1) / 2f) / 6.2f;
                // Lật y: hàng trên của ảnh là đỉnh khe tim.
                float yy = ((size - 1) - py);
                float y = (yy - 4.0f) / 6.2f;

                float v = x * x + y * y - 1f;
                bool inside = v * v * v - x * x * y * y * y < 0f;
                if (inside) tex.SetPixel(px, py, body);
            }
        }

        // Viền tối: pixel thuộc thân nhưng cạnh nó có pixel trống.
        for (int px = 0; px < size; px++)
        {
            for (int py = 0; py < size; py++)
            {
                if (tex.GetPixel(px, py) != body) continue;
                bool border =
                    IsClear(tex, px + 1, py, size) || IsClear(tex, px - 1, py, size) ||
                    IsClear(tex, px, py + 1, size) || IsClear(tex, px, py - 1, size);
                if (border) tex.SetPixel(px, py, edge);
            }
        }

        if (filled)
        {
            tex.SetPixel(5, 11, shine);
            tex.SetPixel(5, 10, shine);
            tex.SetPixel(4, 10, shine);
        }

        tex.Apply();
        return tex;
    }

    private static bool IsClear(Texture2D tex, int x, int y, int size)
        => x < 0 || y < 0 || x >= size || y >= size || tex.GetPixel(x, y).a < 0.01f;
}
