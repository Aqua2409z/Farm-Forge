using UnityEngine;

/// <summary>
/// Nhá màu đỏ trong tích tắc khi bị trúng đòn (cho người chơi và quái).
/// Chỉ đổi màu SpriteRenderer rồi trả lại trắng — không đụng tới sprite nên
/// hoạt hình vẫn chạy bình thường.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class HitFlash : MonoBehaviour
{
    private const float Duration = 0.12f;
    private static readonly Color FlashColor = new Color(1f, 0.35f, 0.35f);

    private SpriteRenderer sr;
    private float timer;

    private void Awake() => sr = GetComponent<SpriteRenderer>();

    public void Flash()
    {
        if (sr == null) return;
        timer = Duration;
        sr.color = FlashColor;
    }

    private void Update()
    {
        if (timer <= 0f) return;
        timer -= Time.deltaTime;
        if (timer <= 0f) sr.color = Color.white;
    }
}
