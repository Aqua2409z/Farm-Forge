using UnityEngine;

/// <summary>
/// Vệt chém hình lưỡi liềm hiện chớp nhoáng trước mặt khi người chơi tấn công.
/// Tạo hình bằng code (không cần art), tự to ra + mờ dần rồi biến mất.
/// </summary>
public class SlashEffect : MonoBehaviour
{
    private const float Life = 0.16f;

    private static Sprite sprite;
    private SpriteRenderer sr;
    private float age;

    public static void Spawn(Vector3 pos, Vector2 facing)
    {
        if (sprite == null) sprite = BuildSprite();

        var go = new GameObject("Slash");
        go.transform.position = pos;
        float ang = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;
        go.transform.rotation = Quaternion.Euler(0f, 0f, ang);

        var fx = go.AddComponent<SlashEffect>();
        fx.sr = go.AddComponent<SpriteRenderer>();
        fx.sr.sprite = sprite;
        fx.sr.sortingOrder = 11; // trên người chơi (10)
        fx.sr.color = new Color(1f, 1f, 1f, 0.9f);
    }

    private void Update()
    {
        age += Time.deltaTime;
        float t = age / Life;
        if (sr != null)
        {
            var c = sr.color;
            c.a = Mathf.Lerp(0.9f, 0f, t);
            sr.color = c;
            transform.localScale = Vector3.one * Mathf.Lerp(1.0f, 1.6f, t);
        }
        if (age >= Life) Destroy(gameObject);
    }

    private static Sprite BuildSprite()
    {
        const int size = 32;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
        var clear = new Color(0f, 0f, 0f, 0f);

        float c = (size - 1) / 2f;
        const float rOut = 15f, rIn = 10f;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dx = x - c, dy = y - c;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                bool ring = d <= rOut && d >= rIn;
                bool rightHalf = dx >= -2f; // chỉ lấy nửa cung bên phải -> hình lưỡi liềm
                tex.SetPixel(x, y, ring && rightHalf ? Color.white : clear);
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 24f);
    }
}
