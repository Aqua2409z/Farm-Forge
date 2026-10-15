using UnityEngine;

/// <summary>
/// Tự khởi tạo thế giới nông trại NGAY KHI bấm Play — không cần sửa scene thủ công.
/// Nhờ vậy các story sau chỉ cần thêm/sửa script là chạy, KHÔNG phải đóng/mở lại Unity.
///
/// Cơ chế: [RuntimeInitializeOnLoadMethod] được Unity gọi tự động sau khi scene load xong.
/// </summary>
public static class FarmWorldBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        // Tìm nhân vật trong scene. Nếu không có (scene khác) thì bỏ qua.
        var player = Object.FindFirstObjectByType<PlayerMovement>();
        if (player == null) return;

        // Cho nhân vật vẽ ĐÈ lên cỏ.
        var playerRenderer = player.GetComponent<SpriteRenderer>();
        if (playerRenderer != null) playerRenderer.sortingOrder = 10;

        // Art nhân vật thật + animation đi bộ 4 hướng (tự giữ ô vuông nếu thiếu ảnh).
        var visual = player.gameObject.AddComponent<PlayerVisual>();
        visual.Configure(player);

        // Máu + hồi sinh của người chơi (6 tim). Điểm hồi sinh = chỗ đang đứng.
        var status = player.gameObject.AddComponent<PlayerStatus>();
        status.Configure(player.transform.position, 6);

        // Tấn công (phím J — chém quái trước mặt).
        var combat = player.gameObject.AddComponent<PlayerCombat>();
        combat.Configure(player);

        // Camera bám theo nhân vật.
        var cam = Camera.main != null ? Camera.main : Object.FindFirstObjectByType<Camera>();
        if (cam != null)
        {
            var follow = cam.gameObject.AddComponent<CameraFollow>();
            follow.Configure(player.transform);
        }

        // Chu kỳ ngày → đêm (lớp phủ tối bám theo camera). Ban đêm quái xuất hiện.
        DayNightCycle dayNight = null;
        if (cam != null)
        {
            var dayNightGO = new GameObject("DayNightCycle");
            dayNight = dayNightGO.AddComponent<DayNightCycle>();
            dayNight.Configure(cam);
        }

        // Âm thanh (tạo bằng code — cuốc / gieo / thu hoạch / mua / bán).
        var audioGO = new GameObject("GameAudio");
        audioGO.AddComponent<GameAudio>();

        // Lưới ô đất (tự sinh các ô cỏ trong Awake của nó).
        var gridGO = new GameObject("FarmWorld");
        var grid = gridGO.AddComponent<FarmGridSystem>();

        // Trang trí map (hoa, cỏ, nấm rải ngẫu nhiên trên cỏ).
        var decorGO = new GameObject("MapDecorations");
        var decorations = decorGO.AddComponent<MapDecorations>();
        decorations.Configure(grid);

        // Quản lý cây trồng.
        var cropSystemGO = new GameObject("CropSystem");
        var crops = cropSystemGO.AddComponent<CropSystem>();
        crops.Configure(grid);

        // Túi đồ (nông sản, hạt giống, tiền) — đếm theo TỪNG loại cây.
        // Khởi đầu: 5 hạt "củ vàng" (loại 0) + 3 hạt "củ cải tím" (loại 1) để thử ngay.
        var inventoryGO = new GameObject("Inventory");
        var inventory = inventoryGO.AddComponent<Inventory>();
        inventory.Setup(CropDatabase.Count, new[] { 5, 3 });

        // Chọn loại hạt để gieo (đổi bằng phím 1 / 2 / Q).
        var selectionGO = new GameObject("SeedSelection");
        var selection = selectionGO.AddComponent<SeedSelection>();
        selection.Setup(CropDatabase.Count);

        // Tiệm bán hàng — đặt ở góc trên-phải nông trại. Dùng cái rương vàng làm tiệm.
        var shopGO = new GameObject("Shop");
        var shopSr = shopGO.AddComponent<SpriteRenderer>();
        var chestSprite = SproutLandsArt.Chest();
        if (chestSprite != null)
        {
            shopSr.sprite = chestSprite;              // rương vàng (art thật)
            shopGO.transform.localScale = Vector3.one; // khung 48px đã to sẵn
        }
        else
        {
            shopSr.sprite = PlaceholderSprites.CreateBorderedSquare(
                new Color(0.30f, 0.55f, 0.95f), new Color(0.15f, 0.30f, 0.60f)); // xanh dương nổi bật
            shopGO.transform.localScale = Vector3.one * 1.4f; // to hơn cho dễ thấy
        }
        shopSr.sortingOrder = 3;
        shopGO.transform.position = grid.CellToWorldCenter(new Vector2Int(7, 4));
        var shop = shopGO.AddComponent<Shop>();
        shop.Configure(player.transform, inventory, selection);

        // HUD (nông sản + hạt + tiền + gợi ý tiệm + tim máu + nhãn ngày/đêm).
        var hudGO = new GameObject("GameHUD");
        var hud = hudGO.AddComponent<GameHUD>();
        hud.Configure(inventory, shop, selection, status.Health, dayNight);

        // Ô đang nhắm (khung highlight) — bám theo hướng nhìn của nhân vật.
        var selectorGO = new GameObject("TileSelector");
        var selector = selectorGO.AddComponent<TileSelector>();
        selector.Configure(player, grid);

        // Xử lý hành động (thu hoạch / cuốc đất / gieo hạt).
        var interactionGO = new GameObject("FarmInteraction");
        var interaction = interactionGO.AddComponent<FarmInteraction>();
        interaction.Configure(selector, grid, crops, inventory, decorations, selection);

        // Lưu/nạp game. Nạp NGAY bây giờ (trước khi MapDecorations rải trang trí ở
        // frame sau — nhờ vậy ô đã cuốc trong file lưu sẽ không bị mọc hoa/cỏ lên).
        var saveGO = new GameObject("SaveSystem");
        var save = saveGO.AddComponent<SaveSystem>();
        save.Configure(grid, crops, inventory, selection, decorations);
        save.LoadNow();

        // Sinh quái ban đêm (orc / skeleton ở rìa bản đồ). Ban ngày tự dọn sạch.
        var spawnerGO = new GameObject("EnemySpawner");
        var spawner = spawnerGO.AddComponent<EnemySpawner>();
        spawner.Configure(grid, status, inventory, dayNight);
    }
}
