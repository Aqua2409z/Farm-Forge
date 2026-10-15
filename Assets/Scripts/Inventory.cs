using UnityEngine;

/// <summary>
/// Túi đồ của người chơi: nông sản (để bán), hạt giống (để trồng), tiền.
/// Story 13: hạt & nông sản đếm THEO TỪNG LOẠI cây (mảng theo id loại).
/// Story 14: nạp/lưu được (xem SaveSystem).
/// </summary>
public class Inventory : MonoBehaviour
{
    private int[] seeds; // số hạt giống theo loại
    private int[] crops; // số nông sản chờ bán theo loại

    /// <summary>Số tiền.</summary>
    public int Coins { get; private set; }

    /// <summary>Số loại cây đang quản lý.</summary>
    public int TypeCount => seeds?.Length ?? 0;

    /// <summary>Khởi tạo túi đồ với số loại cây + số hạt khởi đầu cho từng loại.</summary>
    public void Setup(int typeCount, int[] startingSeeds)
    {
        seeds = new int[typeCount];
        crops = new int[typeCount];
        if (startingSeeds != null)
            for (int i = 0; i < typeCount && i < startingSeeds.Length; i++)
                seeds[i] = startingSeeds[i];
    }

    public int Seeds(int type) => Valid(type) ? seeds[type] : 0;
    public int Crops(int type) => Valid(type) ? crops[type] : 0;

    public void AddCrops(int type, int amount) { if (Valid(type)) crops[type] += amount; }
    public void RemoveAllCrops(int type) { if (Valid(type)) crops[type] = 0; }
    public void AddSeeds(int type, int amount) { if (Valid(type)) seeds[type] += amount; }

    /// <summary>Dùng 1 hạt giống loại <paramref name="type"/> nếu còn. Trả về true nếu dùng được.</summary>
    public bool UseSeed(int type)
    {
        if (!Valid(type) || seeds[type] <= 0) return false;
        seeds[type]--;
        return true;
    }

    public void AddCoins(int amount) => Coins += amount;

    /// <summary>Tiêu tiền nếu đủ. Trả về true nếu tiêu thành công.</summary>
    public bool SpendCoins(int amount)
    {
        if (Coins < amount) return false;
        Coins -= amount;
        return true;
    }

    // --- Dùng cho SaveSystem (lưu / nạp) ---
    public int[] SeedsArray => seeds;
    public int[] CropsArray => crops;

    public void LoadState(int coins, int[] seedsData, int[] cropsData)
    {
        Coins = coins;
        if (seedsData != null)
            for (int i = 0; i < seeds.Length && i < seedsData.Length; i++) seeds[i] = seedsData[i];
        if (cropsData != null)
            for (int i = 0; i < crops.Length && i < cropsData.Length; i++) crops[i] = cropsData[i];
    }

    private bool Valid(int type) => seeds != null && type >= 0 && type < seeds.Length;
}
