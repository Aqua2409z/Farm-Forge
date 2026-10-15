using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Story 4: Quản lý cây trồng trên các ô (gieo hạt, lưu cây theo toạ độ ô).
/// Story 5: Mỗi cây tự lớn theo thời gian (tốc độ do secondsPerStage quy định).
/// Story 6: Thu hoạch cây chín (xoá cây khỏi ô).
/// Story 13: Gieo theo LOẠI cây (sprite & giá khác nhau — xem CropDatabase).
/// Story 14: Cho phép nạp lại cây đã lưu & liệt kê cây để lưu game.
/// </summary>
public class CropSystem : MonoBehaviour
{
    [Tooltip("Số giây để cây lớn thêm 1 giai đoạn.")]
    [SerializeField] private float secondsPerStage = 3f;

    private FarmGridSystem grid;
    private readonly Dictionary<Vector2Int, Crop> crops = new();

    /// <summary>Toàn bộ cây đang có (để SaveSystem duyệt khi lưu game).</summary>
    public IReadOnlyDictionary<Vector2Int, Crop> ActiveCrops => crops;

    public void Configure(FarmGridSystem grid)
    {
        this.grid = grid;
    }

    public bool HasCrop(Vector2Int cell) => crops.ContainsKey(cell);

    /// <summary>Lấy cây ở ô (null nếu không có).</summary>
    public Crop GetCrop(Vector2Int cell)
        => crops.TryGetValue(cell, out var crop) ? crop : null;

    /// <summary>Gieo hạt loại <paramref name="typeId"/>: chỉ trên ô đã cuốc và chưa có cây.</summary>
    public bool PlantSeed(Vector2Int cell, int typeId)
    {
        if (grid.GetState(cell) != TileState.Tilled) return false;
        if (HasCrop(cell)) return false;

        CreateCrop(cell, typeId, 0, 0f);
        return true;
    }

    /// <summary>Đặt lại 1 cây khi NẠP game đã lưu (biết sẵn loại, giai đoạn, đồng hồ dở).</summary>
    public void PlantLoaded(Vector2Int cell, int typeId, int stage, float timer)
    {
        if (HasCrop(cell)) return;
        CreateCrop(cell, typeId, stage, timer);
    }

    private void CreateCrop(Vector2Int cell, int typeId, int stage, float timer)
    {
        var type = CropDatabase.Get(typeId);

        var cropGO = new GameObject($"Crop_{cell.x}_{cell.y}");
        cropGO.transform.position = grid.CellToWorldCenter(cell);

        var crop = cropGO.AddComponent<Crop>();
        crop.Init(type.StageSprites, secondsPerStage, typeId);
        if (stage > 0 || timer > 0f) crop.LoadState(stage, timer);

        crops[cell] = crop;
    }

    /// <summary>
    /// Thu hoạch: chỉ khi ô có cây ĐÃ CHÍN. Xoá cây, giữ nguyên ô đất (trồng lại được).
    /// Trả về true nếu thu hoạch thành công.
    /// </summary>
    public bool Harvest(Vector2Int cell)
    {
        var crop = GetCrop(cell);
        if (crop == null || !crop.IsMature) return false;

        crops.Remove(cell);
        Destroy(crop.gameObject);
        return true;
    }
}
