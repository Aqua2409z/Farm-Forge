using UnityEngine;

/// <summary>
/// Story 1: Di chuyển nhân vật kiểu top-down bằng WASD / phím mũi tên.
/// Story 2: Theo dõi HƯỚNG NHÌN (facing) để biết ô nào đang ở trước mặt.
///
/// Gắn script này vào GameObject "Player" (có SpriteRenderer).
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Tooltip("Tốc độ di chuyển (đơn vị/giây). Chỉnh trong Inspector để cảm nhận.")]
    [SerializeField] private float moveSpeed = 5f;

    /// <summary>Hướng nhân vật đang nhìn (4 hướng). Mặc định nhìn xuống.</summary>
    public Vector2Int FacingDirection { get; private set; } = Vector2Int.down;

    private void Update()
    {
        // Đọc input ngang/dọc từ Input Manager cũ (đơn giản cho game đầu tay).
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Chuẩn hoá hướng để đi chéo không nhanh hơn đi thẳng.
        Vector2 direction = new Vector2(horizontal, vertical).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        UpdateFacing(horizontal, vertical);
    }

    /// <summary>
    /// Cập nhật hướng nhìn theo input. Khi đứng yên thì giữ nguyên hướng cũ.
    /// Dùng trục nào trội hơn để quyết định (4 hướng).
    /// </summary>
    private void UpdateFacing(float horizontal, float vertical)
    {
        if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
        {
            FacingDirection = horizontal > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else if (Mathf.Abs(vertical) > 0f)
        {
            FacingDirection = vertical > 0 ? Vector2Int.up : Vector2Int.down;
        }
    }
}
