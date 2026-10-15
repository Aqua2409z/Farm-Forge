using UnityEngine;

/// <summary>
/// Story 18: Người chơi tấn công bằng CHUỘT TRÁI (phím J vẫn chém được cho ai thích bàn phím) —
/// vung cây kiếm thật hình vòng cung trước mặt, gây sát thương mọi quái nằm trong tầm
/// & ~phía trước (theo hướng đang nhìn). Gắn vào chính GameObject Player.
/// </summary>
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private KeyCode attackKey = KeyCode.J; // phím phụ (tuỳ chọn)
    [SerializeField] private int attackDamage = 2;
    [SerializeField] private float attackRange = 1.4f;
    [SerializeField] private float cooldown = 0.35f;

    private PlayerMovement movement;
    private float timer;

    public void Configure(PlayerMovement movement) => this.movement = movement;

    private void Update()
    {
        if (movement == null) return;
        if (timer > 0f) timer -= Time.deltaTime;

        bool pressed = Input.GetMouseButtonDown(0) || Input.GetKeyDown(attackKey);
        if (timer <= 0f && pressed) Attack();
    }

    private void Attack()
    {
        timer = cooldown;

        Vector2 facing = movement.FacingDirection;
        Vector3 hand = transform.position + (Vector3)(facing * 0.15f);
        SwordSwing.Spawn(hand, facing); // vung kiếm thật (thiếu art thì tự ra vệt chém)
        GameAudio.Till(); // tạm dùng tiếng "thụp" làm tiếng chém

        // Gây sát thương quái trong tầm & phía trước.
        var list = Enemy.All;
        for (int i = list.Count - 1; i >= 0; i--)
        {
            var e = list[i];
            if (e == null) continue;

            Vector2 to = (Vector2)(e.transform.position - transform.position);
            if (to.magnitude > attackRange) continue;
            if (Vector2.Dot(to.normalized, facing) < 0.35f) continue; // chỉ trúng phía trước

            e.TakeDamage(attackDamage);
        }
    }
}
