using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rải hình trang trí nhỏ (hoa, cỏ, nấm...) lên các ô cỏ ngẫu nhiên để map sinh động
/// như mấy game nông trại khác. Khi người chơi cuốc 1 ô thì xoá trang trí ở ô đó đi.
///
/// Trang trí chỉ là hình trang trí (không va chạm) — nhân vật đi xuyên qua bình thường.
/// </summary>
public class MapDecorations : MonoBehaviour
{
    [Tooltip("Số lượng trang trí rải ra.")]
    [SerializeField] private int count = 26;

    [Tooltip("Hạt giống ngẫu nhiên cố định để lần nào chạy cũng rải giống nhau.")]
    [SerializeField] private int randomSeed = 2026;

    private FarmGridSystem grid;
    private Sprite[] decorSprites;
    private readonly Dictionary<Vector2Int, GameObject> decors = new();

    public void Configure(FarmGridSystem grid)
    {
        this.grid = grid;
    }

    private void Start()
    {
        decorSprites = SproutLandsArt.DecorSprites();
        if (decorSprites == null || grid == null) return; // thiếu art -> bỏ qua, map vẫn chạy
        Scatter();
    }

    private void Scatter()
    {
        var rng = new System.Random(randomSeed);
        var shopCell = new Vector2Int(7, 4); // tránh ô đặt tiệm

        int guard = 0;
        while (decors.Count < count && guard < count * 30)
        {
            guard++;
            var cell = grid.RandomCell(rng);
            if (decors.ContainsKey(cell)) continue;
            if (cell == shopCell) continue;
            if (grid.GetState(cell) != TileState.Grass) continue; // chỉ rải trên cỏ

            var sprite = decorSprites[rng.Next(decorSprites.Length)];
            var go = new GameObject($"Decor_{cell.x}_{cell.y}");
            go.transform.SetParent(transform);
            go.transform.position = grid.CellToWorldCenter(cell);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = 1; // trên cỏ (0), dưới cây (2) và nhân vật (10)

            decors[cell] = go;
        }
    }

    /// <summary>Xoá trang trí ở 1 ô (gọi khi ô đó bị cuốc thành đất).</summary>
    public void ClearAt(Vector2Int cell)
    {
        if (decors.TryGetValue(cell, out var go))
        {
            decors.Remove(cell);
            if (go != null) Destroy(go);
        }
    }
}
