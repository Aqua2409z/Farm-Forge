using System.IO;
using UnityEngine;

/// <summary>
/// Story 14: Lưu & nạp nông trại — thoát ra mở lại vẫn còn nguyên:
///   đất đã cuốc, cây (loại + giai đoạn + đồng hồ lớn dở), tiền, hạt, loại hạt đang chọn.
///
/// Lưu ở file JSON trong Application.persistentDataPath (thư mục dữ liệu riêng của game,
/// KHÔNG đụng vào thư mục dự án). Tự lưu khi: thoát game, mỗi 20 giây, hoặc bấm F5.
/// Tự nạp 1 lần ngay khi mở game (do FarmWorldBootstrap gọi LoadNow).
/// </summary>
public class SaveSystem : MonoBehaviour
{
    private const float AutosaveInterval = 20f;

    private FarmGridSystem grid;
    private CropSystem crops;
    private Inventory inventory;
    private SeedSelection selection;
    private MapDecorations decorations;

    private float autosaveTimer;

    private string SavePath => Path.Combine(Application.persistentDataPath, "farmsave.json");

    public void Configure(FarmGridSystem grid, CropSystem crops, Inventory inventory,
        SeedSelection selection, MapDecorations decorations)
    {
        this.grid = grid;
        this.crops = crops;
        this.inventory = inventory;
        this.selection = selection;
        this.decorations = decorations;
    }

    /// <summary>Nạp game đã lưu (nếu có). Trả về true nếu có file để nạp.</summary>
    public bool LoadNow()
    {
        if (!File.Exists(SavePath)) return false;

        SaveData data;
        try { data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath)); }
        catch { return false; }
        if (data == null) return false;

        inventory.LoadState(data.coins, data.seeds, data.crops);
        selection?.Select(data.selectedType);

        // Cuốc lại các ô đã lưu (và xoá trang trí ở đó cho sạch).
        if (data.tilled != null)
            foreach (var t in data.tilled)
            {
                var cell = new Vector2Int(t.x, t.y);
                grid.TillTile(cell);
                decorations?.ClearAt(cell);
            }

        // Trồng lại cây đúng loại/giai đoạn/đồng hồ.
        if (data.plantedCrops != null)
            foreach (var cs in data.plantedCrops)
                crops.PlantLoaded(new Vector2Int(cs.x, cs.y), cs.typeId, cs.stage, cs.timer);

        return true;
    }

    public void Save()
    {
        if (grid == null || inventory == null || crops == null) return;

        var data = new SaveData
        {
            coins = inventory.Coins,
            seeds = inventory.SeedsArray,
            crops = inventory.CropsArray,
            selectedType = selection != null ? selection.SelectedType : 0,
        };

        foreach (var cell in grid.TilledCells())
            data.tilled.Add(new TileSave { x = cell.x, y = cell.y });

        foreach (var kv in crops.ActiveCrops)
        {
            var cell = kv.Key;
            var crop = kv.Value;
            data.plantedCrops.Add(new CropSave
            {
                x = cell.x,
                y = cell.y,
                typeId = crop.TypeId,
                stage = crop.Stage,
                timer = crop.Timer,
            });
        }

        try { File.WriteAllText(SavePath, JsonUtility.ToJson(data)); }
        catch { /* không lưu được thì bỏ qua, không làm game crash */ }
    }

    private void Update()
    {
        autosaveTimer += Time.deltaTime;
        if (autosaveTimer >= AutosaveInterval)
        {
            autosaveTimer = 0f;
            Save();
        }

        if (Input.GetKeyDown(KeyCode.F5)) Save(); // lưu tay cho chắc
    }

    private void OnApplicationQuit() => Save(); // tắt game / bấm Stop trong Editor cũng lưu
}
