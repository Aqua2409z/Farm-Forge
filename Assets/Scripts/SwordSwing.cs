using UnityEngine;

/// <summary>
/// Cây kiếm VUNG một nhát hình vòng cung trước mặt khi người chơi chém (chuột trái).
/// Dùng art kiếm thật (WeaponArt). Thiếu art -> tự quay về vệt chém lưỡi liềm (SlashEffect).
/// </summary>
public class SwordSwing : MonoBehaviour
{
    private const float Life = 0.18f;
    private const float ArcDegrees = 120f; // tổng góc quét của nhát chém
    private const float NativeAngle = 45f; // lưỡi kiếm gốc chỉ hướng Đông-Bắc (45°)

    private SpriteRenderer sr;
    private float age;
    private float baseAngle;

    public static void Spawn(Vector3 handPos, Vector2 facing)
    {
        var sword = WeaponArt.Sword();
        if (sword == null) { SlashEffect.Spawn(handPos, facing); return; } // thiếu art -> dùng vệt chém

        var go = new GameObject("SwordSwing");
        go.transform.position = handPos;

        var fx = go.AddComponent<SwordSwing>();
        fx.sr = go.AddComponent<SpriteRenderer>();
        fx.sr.sprite = sword;
        fx.sr.sortingOrder = 11; // trên người chơi (10)

        float aim = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;
        fx.baseAngle = aim - NativeAngle; // xoay để lưỡi chỉ đúng hướng đang nhìn
    }

    private void Update()
    {
        age += Time.deltaTime;
        float t = Mathf.Clamp01(age / Life);

        // Quét từ +nửa cung xuống -nửa cung (chém từ trên xuống).
        float swing = Mathf.Lerp(ArcDegrees * 0.5f, -ArcDegrees * 0.5f, t);
        transform.rotation = Quaternion.Euler(0f, 0f, baseAngle + swing);

        var c = sr.color;
        c.a = Mathf.Lerp(1f, 0.1f, t); // mờ dần cuối nhát
        sr.color = c;

        if (age >= Life) Destroy(gameObject);
    }
}
