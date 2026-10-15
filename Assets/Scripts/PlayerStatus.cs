using UnityEngine;

/// <summary>
/// Story 16: Máu + hồi sinh của NGƯỜI CHƠI.
///   - Bị quái chạm -> mất máu, nhá đỏ, và BẤT TỬ ngắn (tránh mất máu dồn dập).
///   - Hết máu -> hồi sinh ở điểm xuất phát, đầy máu, bất tử vài giây cho đỡ oan.
/// Gắn vào chính GameObject Player.
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    private const float InvulnAfterHit = 0.8f;
    private const float InvulnAfterRespawn = 2f;

    private Health health;
    private HitFlash flash;
    private Vector3 spawnPoint;
    private float invuln;

    public Health Health => health;

    /// <summary>Đang bất tử (quái không gây sát thương được).</summary>
    public bool Invulnerable => invuln > 0f;

    public void Configure(Vector3 spawnPoint, int maxHP)
    {
        this.spawnPoint = spawnPoint;

        health = gameObject.AddComponent<Health>();
        health.Setup(maxHP);
        flash = gameObject.AddComponent<HitFlash>();

        health.Damaged += OnDamaged;
        health.Died += OnDied;
    }

    private void OnDamaged()
    {
        flash.Flash();
        invuln = InvulnAfterHit;
    }

    private void OnDied()
    {
        transform.position = spawnPoint;
        health.FullHeal();
        invuln = InvulnAfterRespawn;
    }

    private void Update()
    {
        if (invuln > 0f) invuln -= Time.deltaTime;
    }
}
