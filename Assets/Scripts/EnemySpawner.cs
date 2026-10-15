using UnityEngine;

/// <summary>
/// Story 17: Sinh quái Ở RÌA bản đồ khi TRỜI TỐI (ban đêm). Ban ngày ngừng sinh và
/// dọn sạch quái còn sót. Quái mạnh dần theo số ngày đã qua.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int maxAlive = 6;
    [SerializeField] private float spawnInterval = 2.5f;

    private FarmGridSystem grid;
    private PlayerStatus player;
    private Inventory inventory;
    private DayNightCycle dayNight;

    private Sprite[] orcFrames;
    private Sprite[] skeletonFrames;

    private float timer;
    private bool wasNight;
    private readonly System.Random rng = new System.Random();

    public void Configure(FarmGridSystem grid, PlayerStatus player, Inventory inventory, DayNightCycle dayNight)
    {
        this.grid = grid;
        this.player = player;
        this.inventory = inventory;
        this.dayNight = dayNight;

        orcFrames = PixelCrawlerArt.OrcFrames();
        skeletonFrames = PixelCrawlerArt.SkeletonFrames();
    }

    private void Update()
    {
        if (dayNight == null || player == null) return;

        bool night = dayNight.IsNight;
        if (night)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f && CountAlive() < maxAlive)
            {
                Spawn();
                timer = spawnInterval;
            }
        }
        else if (wasNight) // vừa chuyển sang ngày
        {
            BurnAll();
        }

        wasNight = night;
    }

    private int CountAlive()
    {
        int n = 0;
        foreach (var e in Enemy.All)
            if (e != null) n++;
        return n;
    }

    private void Spawn()
    {
        var cell = grid.RandomEdgeCell(rng);

        var go = new GameObject("Enemy");
        go.transform.position = grid.CellToWorldCenter(cell);
        var enemy = go.AddComponent<Enemy>();

        // Chọn ngẫu nhiên orc / skeleton (ưu tiên loại nào có art).
        bool orc = rng.Next(2) == 0;
        Sprite[] frames = orc ? orcFrames : skeletonFrames;
        if (frames == null) frames = orcFrames ?? skeletonFrames;

        int day = dayNight.DayNumber;
        int hp = 3 + day;                 // mạnh dần theo ngày
        float speed = 1.6f + 0.05f * day;
        int touch = 1;                    // mỗi đòn trừ 1 tim
        int coin = orc ? 6 : 4;           // orc trâu hơn -> thưởng nhiều hơn

        enemy.Configure(player, inventory, frames, hp, speed, touch, coin);
    }

    private void BurnAll()
    {
        for (int i = Enemy.All.Count - 1; i >= 0; i--)
        {
            var e = Enemy.All[i];
            if (e != null) e.BurnAtSunrise();
        }
    }
}
