using UnityEngine;

/// <summary>
/// Story 4: Một cây trồng nằm trên ô đất, có nhiều GIAI ĐOẠN lớn (stage).
/// Story 5: Cây TỰ LỚN theo thời gian (mỗi giai đoạn cách nhau secondsPerStage giây).
/// </summary>
public class Crop : MonoBehaviour
{
    private SpriteRenderer sr;
    private Sprite[] stageSprites;
    private int stage;

    private float secondsPerStage;
    private float timer;

    /// <summary>Loại cây (0 = củ vàng, 1 = củ cải tím...). Dùng để bán đúng giá & lưu game.</summary>
    public int TypeId { get; private set; }

    /// <summary>Giai đoạn hiện tại (0 = mới gieo).</summary>
    public int Stage => stage;

    /// <summary>Đồng hồ lớn đang chạy dở (giây) — để lưu game cho đúng.</summary>
    public float Timer => timer;

    /// <summary>Giai đoạn cao nhất (cây chín).</summary>
    public int MaxStage => stageSprites.Length - 1;

    /// <summary>Cây đã chín chưa (có thể thu hoạch).</summary>
    public bool IsMature => stage >= MaxStage;

    public void Init(Sprite[] stageSprites, float secondsPerStage, int typeId)
    {
        this.stageSprites = stageSprites;
        this.secondsPerStage = secondsPerStage;
        TypeId = typeId;

        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 2; // trên đất (0), dưới nhân vật (10)
        SetStage(0);
    }

    /// <summary>Đặt lại trạng thái khi NẠP game đã lưu (giai đoạn + đồng hồ dở).</summary>
    public void LoadState(int savedStage, float savedTimer)
    {
        timer = savedTimer;
        SetStage(savedStage);
    }

    private void Update()
    {
        if (stageSprites == null || IsMature) return;

        // Đếm giờ; đủ thời gian thì lớn thêm 1 giai đoạn.
        timer += Time.deltaTime;
        if (timer >= secondsPerStage)
        {
            timer -= secondsPerStage;
            Grow();
        }
    }

    public void SetStage(int newStage)
    {
        stage = Mathf.Clamp(newStage, 0, MaxStage);
        sr.sprite = stageSprites[stage];
    }

    /// <summary>Lớn thêm 1 giai đoạn (nếu chưa chín).</summary>
    public void Grow()
    {
        if (!IsMature) SetStage(stage + 1);
    }
}
