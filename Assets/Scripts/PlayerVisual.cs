using UnityEngine;

/// <summary>
/// Thay ô vuông placeholder bằng NHÂN VẬT THẬT (Sprout Lands) + animation đi bộ 4 hướng.
/// Chạy hoàn toàn bằng code (không cần Animator): mỗi frame tự chọn đúng sprite theo
/// hướng nhìn và đổi khung khi đang di chuyển.
///
/// Không có art thật (thiếu file) thì component tự "ngủ" và giữ nguyên ô vuông cũ.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerVisual : MonoBehaviour
{
    [Tooltip("Số khung animation mỗi giây khi đang đi.")]
    [SerializeField] private float framesPerSecond = 8f;

    private const int DirCount = 4;   // xuống / lên / phải / trái
    private const int FrameCount = 4; // 4 khung mỗi hướng

    private PlayerMovement movement;
    private SpriteRenderer sr;
    private Sprite[,] frames; // [dir, frame]

    private float animTimer;
    private int animFrame;

    public void Configure(PlayerMovement movement)
    {
        this.movement = movement;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        LoadFrames();
    }

    private void LoadFrames()
    {
        var loaded = new Sprite[DirCount, FrameCount];
        for (int dir = 0; dir < DirCount; dir++)
        {
            for (int f = 0; f < FrameCount; f++)
            {
                var s = SproutLandsArt.Character(dir, f);
                if (s == null) return; // thiếu art -> bỏ, giữ placeholder
                loaded[dir, f] = s;
            }
        }

        frames = loaded;
        sr.sortingOrder = 10; // vẽ đè lên cỏ/cây
    }

    private void Update()
    {
        if (frames == null || movement == null) return;

        bool moving = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f
                   || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.01f;

        if (moving)
        {
            animTimer += Time.deltaTime;
            float frameTime = 1f / framesPerSecond;
            if (animTimer >= frameTime)
            {
                animTimer -= frameTime;
                animFrame = (animFrame + 1) % FrameCount;
            }
        }
        else
        {
            animFrame = 0;  // đứng yên = khung đứng
            animTimer = 0f;
        }

        sr.sprite = frames[DirIndex(movement.FacingDirection), animFrame];
    }

    private int DirIndex(Vector2Int facing)
    {
        if (facing == Vector2Int.up) return 1;
        if (facing == Vector2Int.right) return 3; // hàng 3 mới là mặt quay phải (đã sửa)
        if (facing == Vector2Int.left) return 2;  // hàng 2 là mặt quay trái
        return 0; // xuống (mặc định)
    }
}
