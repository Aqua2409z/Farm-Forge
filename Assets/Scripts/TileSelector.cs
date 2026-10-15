using UnityEngine;

/// <summary>
/// Story 2: Ô đang được nhắm — ô ngay PHÍA TRƯỚC hướng nhìn của nhân vật.
/// GameObject này mang luôn khung viền sáng (highlight) và tự di chuyển tới ô mục tiêu.
///
/// Story 3+ sẽ dùng <see cref="TargetCell"/> để biết cuốc/trồng vào ô nào.
/// </summary>
public class TileSelector : MonoBehaviour
{
    private PlayerMovement player;
    private FarmGridSystem grid;
    private SpriteRenderer highlight;

    /// <summary>Ô đang nhắm (toạ độ cell).</summary>
    public Vector2Int TargetCell { get; private set; }

    /// <summary>Ô đang nhắm có nằm trong lưới không.</summary>
    public bool HasValidTarget { get; private set; }

    /// <summary>Gắn nguồn dữ liệu (được gọi bởi FarmWorldBootstrap lúc khởi tạo).</summary>
    public void Configure(PlayerMovement player, FarmGridSystem grid)
    {
        this.player = player;
        this.grid = grid;
    }

    private void Awake()
    {
        highlight = gameObject.AddComponent<SpriteRenderer>();
        highlight.sprite = PlaceholderSprites.CreateHollowSquare(
            new Color(1f, 1f, 0.3f, 0.9f), thickness: 2); // viền vàng
        highlight.sortingOrder = 5; // trên cỏ, dưới nhân vật
    }

    private void LateUpdate()
    {
        if (player == null || grid == null) return;

        // Ô mục tiêu = ô nhân vật đang đứng + 1 bước theo hướng nhìn.
        Vector2Int playerCell = grid.WorldToCell(player.transform.position);
        TargetCell = playerCell + player.FacingDirection;
        HasValidTarget = grid.IsInside(TargetCell);

        highlight.enabled = HasValidTarget;
        if (HasValidTarget)
        {
            transform.position = grid.CellToWorldCenter(TargetCell);
        }
    }
}
