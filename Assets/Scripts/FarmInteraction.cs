using UnityEngine;

/// <summary>
/// Xử lý hành động của người chơi lên ô đang nhắm (ô trước mặt), bằng MỘT phím Space
/// "thông minh" — tự làm đúng việc theo trạng thái ô:
///   - Cây đã chín   -> thu hoạch (+1 nông sản)      (Story 6)
///   - Ô cỏ          -> cuốc đất                     (Story 3)
///   - Ô đất trống   -> gieo hạt (tốn 1 hạt giống)   (Story 4 + 7)
/// </summary>
public class FarmInteraction : MonoBehaviour
{
    [SerializeField] private KeyCode actionKey = KeyCode.Space;

    private TileSelector selector;
    private FarmGridSystem grid;
    private CropSystem crops;
    private Inventory inventory;
    private MapDecorations decorations;
    private SeedSelection selection;

    public void Configure(TileSelector selector, FarmGridSystem grid, CropSystem crops,
        Inventory inventory, MapDecorations decorations, SeedSelection selection)
    {
        this.selector = selector;
        this.grid = grid;
        this.crops = crops;
        this.inventory = inventory;
        this.decorations = decorations;
        this.selection = selection;
    }

    private void Update()
    {
        if (selector == null || grid == null || crops == null || inventory == null) return;
        if (!Input.GetKeyDown(actionKey) || !selector.HasValidTarget) return;

        Vector2Int cell = selector.TargetCell;
        Crop crop = crops.GetCrop(cell);

        // Ưu tiên: có cây chín thì thu hoạch trước.
        if (crop != null && crop.IsMature)
        {
            int cropType = crop.TypeId; // phải đọc TRƯỚC khi Harvest xoá cây
            if (crops.Harvest(cell))
            {
                inventory.AddCrops(cropType, 1);
                GameAudio.Harvest();
            }
            return;
        }

        TileState state = grid.GetState(cell);
        if (state == TileState.Grass)
        {
            if (grid.TillTile(cell))             // cuốc đất
            {
                decorations?.ClearAt(cell);      // xoá hoa/cỏ trang trí ở ô vừa cuốc
                GameAudio.Till();
            }
        }
        else if (state == TileState.Tilled && crop == null)
        {
            // Gieo hạt của LOẠI đang chọn: cần còn hạt giống loại đó.
            int type = selection != null ? selection.SelectedType : 0;
            if (inventory.UseSeed(type))
            {
                if (crops.PlantSeed(cell, type)) GameAudio.Plant();
            }
        }
    }
}
