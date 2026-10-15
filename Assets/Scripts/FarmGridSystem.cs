using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Story 2: Lưới ô đất của nông trại (sinh ô cỏ + đổi toạ độ world ↔ cell).
/// Story 3: Theo dõi TRẠNG THÁI từng ô + hành động CUỐC ĐẤT (cỏ -> đất nâu).
///
/// Quy ước: mỗi ô rộng 1 đơn vị world; TÂM ô nằm ở toạ độ nguyên (x, y).
/// </summary>
public class FarmGridSystem : MonoBehaviour
{
    [SerializeField] private int width = 18;
    [SerializeField] private int height = 10;

    private int minX, maxX, minY, maxY;

    private Sprite grassSprite;
    private Sprite tilledSprite;

    // Lưu từng ô theo toạ độ cell để còn đổi hình / đổi trạng thái về sau.
    private readonly Dictionary<Vector2Int, SpriteRenderer> tileRenderers = new();
    private readonly Dictionary<Vector2Int, TileState> tileStates = new();

    private void Awake()
    {
        // Lưới cân giữa quanh gốc toạ độ (0,0).
        minX = -width / 2;
        maxX = minX + width - 1;
        minY = -height / 2;
        maxY = minY + height - 1;

        // Ưu tiên art thật (Sprout Lands); thiếu thì dùng ô vuông placeholder.
        grassSprite = SproutLandsArt.Grass()
            ?? PlaceholderSprites.CreateBorderedSquare(
                fill: new Color(0.42f, 0.62f, 0.33f),    // xanh cỏ
                border: new Color(0.32f, 0.48f, 0.25f));
        tilledSprite = SproutLandsArt.Tilled()
            ?? PlaceholderSprites.CreateBorderedSquare(
                fill: new Color(0.45f, 0.30f, 0.18f),    // đất nâu
                border: new Color(0.33f, 0.21f, 0.12f));

        BuildTiles();
    }

    private void BuildTiles()
    {
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                var cell = new Vector2Int(x, y);
                var tile = new GameObject($"Tile_{x}_{y}");
                tile.transform.SetParent(transform);
                tile.transform.position = new Vector3(x, y, 0f);

                var sr = tile.AddComponent<SpriteRenderer>();
                sr.sprite = grassSprite;
                sr.sortingOrder = 0;

                tileRenderers[cell] = sr;
                tileStates[cell] = TileState.Grass;
            }
        }
    }

    /// <summary>Toạ độ thế giới -> ô chứa nó.</summary>
    public Vector2Int WorldToCell(Vector3 world)
        => new Vector2Int(Mathf.RoundToInt(world.x), Mathf.RoundToInt(world.y));

    /// <summary>Toạ độ ô -> tâm ô trong thế giới.</summary>
    public Vector3 CellToWorldCenter(Vector2Int cell)
        => new Vector3(cell.x, cell.y, 0f);

    /// <summary>Ô có nằm trong phạm vi lưới không?</summary>
    public bool IsInside(Vector2Int cell)
        => cell.x >= minX && cell.x <= maxX && cell.y >= minY && cell.y <= maxY;

    /// <summary>Một ô ngẫu nhiên trong lưới (dùng để rải trang trí).</summary>
    public Vector2Int RandomCell(System.Random rng)
        => new Vector2Int(rng.Next(minX, maxX + 1), rng.Next(minY, maxY + 1));

    /// <summary>Một ô ngẫu nhiên ở RÌA lưới (dùng để sinh quái từ mép bản đồ).</summary>
    public Vector2Int RandomEdgeCell(System.Random rng)
    {
        switch (rng.Next(4))
        {
            case 0: return new Vector2Int(rng.Next(minX, maxX + 1), maxY); // cạnh trên
            case 1: return new Vector2Int(rng.Next(minX, maxX + 1), minY); // cạnh dưới
            case 2: return new Vector2Int(minX, rng.Next(minY, maxY + 1)); // cạnh trái
            default: return new Vector2Int(maxX, rng.Next(minY, maxY + 1)); // cạnh phải
        }
    }

    /// <summary>Lấy trạng thái của một ô.</summary>
    public TileState GetState(Vector2Int cell)
        => tileStates.TryGetValue(cell, out var state) ? state : TileState.Grass;

    /// <summary>Liệt kê mọi ô ĐÃ CUỐC (để SaveSystem lưu lại).</summary>
    public IEnumerable<Vector2Int> TilledCells()
    {
        foreach (var kv in tileStates)
            if (kv.Value == TileState.Tilled)
                yield return kv.Key;
    }

    /// <summary>
    /// Cuốc đất: ô cỏ -> ô đất nâu. Trả về true nếu cuốc thành công
    /// (ô nằm trong lưới và đang là cỏ).
    /// </summary>
    public bool TillTile(Vector2Int cell)
    {
        if (!IsInside(cell)) return false;
        if (GetState(cell) != TileState.Grass) return false;

        tileStates[cell] = TileState.Tilled;
        tileRenderers[cell].sprite = tilledSprite;
        return true;
    }
}
