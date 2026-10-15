using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Story 17: Một con quái. Xuất hiện ban đêm, ĐUỔI theo người chơi, chạm vào thì gây
/// sát thương (có nghỉ giữa các đòn). Có máu riêng; chết thì rơi vàng + biến mất.
/// Hoạt hình "đứng" 4 khung chạy bằng code (không cần Animator).
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour
{
    /// <summary>Tất cả quái đang sống — để người chơi chém tìm mục tiêu nhanh.</summary>
    public static readonly List<Enemy> All = new();

    private const float FrameTime = 0.18f;
    private const float TouchRange = 0.6f;
    private const float TouchInterval = 1.0f;

    private SpriteRenderer sr;
    private HitFlash flash;
    private Health health;

    private Transform target;
    private PlayerStatus player;
    private Inventory inventory;

    private Sprite[] frames;
    private float moveSpeed;
    private int touchDamage;
    private int coinReward;

    private int frame;
    private float animTimer;
    private float touchCooldown;

    public Health Health => health;

    public void Configure(PlayerStatus player, Inventory inventory, Sprite[] frames,
        int maxHP, float moveSpeed, int touchDamage, int coinReward)
    {
        this.player = player;
        this.target = player != null ? player.transform : null;
        this.inventory = inventory;
        this.frames = frames;
        this.moveSpeed = moveSpeed;
        this.touchDamage = touchDamage;
        this.coinReward = coinReward;

        sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = 9; // dưới người chơi (10), trên cây (2)
        if (frames != null && frames.Length > 0)
            sr.sprite = frames[0];
        else
            sr.sprite = PlaceholderSprites.CreateBorderedSquare(
                new Color(0.8f, 0.2f, 0.2f), new Color(0.5f, 0.1f, 0.1f)); // thiếu art -> ô đỏ

        flash = gameObject.AddComponent<HitFlash>();

        health = gameObject.AddComponent<Health>();
        health.Setup(maxHP);
        health.Damaged += flash.Flash;
        health.Died += Die;
    }

    private void OnEnable() { if (!All.Contains(this)) All.Add(this); }
    private void OnDisable() { All.Remove(this); }

    private void Update()
    {
        if (target == null || health == null || health.IsDead) return;

        // Hoạt hình đứng.
        if (frames != null && frames.Length > 0)
        {
            animTimer += Time.deltaTime;
            if (animTimer >= FrameTime)
            {
                animTimer -= FrameTime;
                frame = (frame + 1) % frames.Length;
                sr.sprite = frames[frame];
            }
        }

        // Đi về phía người chơi + quay mặt đúng hướng.
        Vector3 to = target.position - transform.position;
        float dist = to.magnitude;
        if (dist > 0.01f)
        {
            transform.position += to / dist * moveSpeed * Time.deltaTime;
            if (Mathf.Abs(to.x) > 0.05f) sr.flipX = to.x < 0f;
        }

        // Chạm người chơi -> gây sát thương (trừ khi người chơi đang bất tử).
        if (touchCooldown > 0f) touchCooldown -= Time.deltaTime;
        if (dist <= TouchRange && touchCooldown <= 0f &&
            player != null && !player.Invulnerable && !player.Health.IsDead)
        {
            player.Health.TakeDamage(touchDamage);
            touchCooldown = TouchInterval;
        }
    }

    /// <summary>Người chơi chém trúng.</summary>
    public void TakeDamage(int amount) => health?.TakeDamage(amount);

    private void Die()
    {
        if (inventory != null) inventory.AddCoins(coinReward);
        GameAudio.Sell(); // tạm dùng tiếng "cha-ching" khi rơi vàng
        All.Remove(this);
        Destroy(gameObject);
    }

    /// <summary>Trời sáng -> quái còn lại biến mất (ban ngày an toàn làm nông).</summary>
    public void BurnAtSunrise()
    {
        All.Remove(this);
        Destroy(gameObject);
    }
}
