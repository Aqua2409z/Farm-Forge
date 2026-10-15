using System;
using UnityEngine;

/// <summary>
/// Story 16: Máu cho bất kỳ ai (người chơi hoặc quái). Gắn vào GameObject rồi Setup(max).
/// Phát sự kiện Damaged (khi mất máu) và Died (khi hết máu) để nơi khác phản ứng
/// (nhá đỏ, rơi vàng, hồi sinh...).
/// </summary>
public class Health : MonoBehaviour
{
    public int Max { get; private set; } = 1;
    public int Current { get; private set; } = 1;
    public bool IsDead => Current <= 0;

    public event Action Damaged;
    public event Action Died;

    public void Setup(int max)
    {
        Max = Mathf.Max(1, max);
        Current = Max;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || amount <= 0) return;
        Current = Mathf.Max(0, Current - amount);
        Damaged?.Invoke();
        if (Current == 0) Died?.Invoke();
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        Current = Mathf.Min(Max, Current + amount);
    }

    public void FullHeal() => Current = Max;
}
