using UnityEngine;

/// <summary>
/// Story 7: Cái tiệm trong nông trại.
/// Khi người chơi đứng GẦN tiệm:
///   - Phím E: bán TẤT CẢ nông sản (mỗi loại tính theo giá riêng).
///   - Phím R: mua 1 hạt giống của LOẠI ĐANG CHỌN (-giá riêng của loại đó).
/// Story 13: giá bán/mua lấy theo từng loại cây (xem CropDatabase).
/// </summary>
public class Shop : MonoBehaviour
{
    [SerializeField] private float interactRange = 1.8f;
    [SerializeField] private KeyCode sellKey = KeyCode.E;
    [SerializeField] private KeyCode buyKey = KeyCode.R;

    private Transform player;
    private Inventory inventory;
    private SeedSelection selection;

    /// <summary>Người chơi có đang đứng trong tầm tương tác không (HUD dùng để hiện gợi ý).</summary>
    public bool PlayerInRange { get; private set; }

    public KeyCode SellKey => sellKey;
    public KeyCode BuyKey => buyKey;

    public void Configure(Transform player, Inventory inventory, SeedSelection selection)
    {
        this.player = player;
        this.inventory = inventory;
        this.selection = selection;
    }

    private void Update()
    {
        if (player == null || inventory == null) return;

        PlayerInRange = Vector2.Distance(player.position, transform.position) <= interactRange;
        if (!PlayerInRange) return;

        if (Input.GetKeyDown(sellKey)) SellAllCrops();
        if (Input.GetKeyDown(buyKey)) BuySeed();
    }

    private void SellAllCrops()
    {
        int earned = 0;
        for (int t = 0; t < inventory.TypeCount; t++)
        {
            int n = inventory.Crops(t);
            if (n <= 0) continue;
            earned += n * CropDatabase.Get(t).SellPrice;
            inventory.RemoveAllCrops(t);
        }

        if (earned > 0)
        {
            inventory.AddCoins(earned);
            GameAudio.Sell();
        }
    }

    private void BuySeed()
    {
        int type = selection != null ? selection.SelectedType : 0;
        int price = CropDatabase.Get(type).SeedBuyPrice;

        if (inventory.SpendCoins(price))
        {
            inventory.AddSeeds(type, 1);
            GameAudio.Buy();
        }
    }
}
