using UnityEngine;

/// <summary>
/// Story 15: Chu kỳ NGÀY → ĐÊM lặp lại. Ban ngày sáng (an toàn làm nông), ban đêm
/// màn hình tối dần (quái xuất hiện). Tạo một lớp phủ tối phủ kín màn hình, bám theo
/// camera, độ mờ thay đổi theo thời gian trong ngày.
/// </summary>
public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private float dayLength = 45f;    // giây ban ngày
    [SerializeField] private float nightLength = 30f;  // giây ban đêm
    [SerializeField] private float maxDarkness = 0.62f;

    private float clock; // 0 .. (dayLength + nightLength)
    private Camera cam;
    private SpriteRenderer overlay;

    public int DayNumber { get; private set; } = 1;

    private float Cycle => dayLength + nightLength;
    public bool IsNight => clock >= dayLength;
    public string PhaseLabel => IsNight ? "BAN ĐÊM" : "Ban ngày";

    /// <summary>0 = ban ngày; tăng dần lên 1 giữa đêm rồi hạ về 0 trước khi sáng.</summary>
    public float NightAmount
    {
        get
        {
            if (clock < dayLength) return 0f;
            float t = (clock - dayLength) / nightLength; // 0..1 trong đêm
            return Mathf.Sin(t * Mathf.PI);              // 0 -> 1 -> 0
        }
    }

    public void Configure(Camera camera)
    {
        cam = camera;

        var go = new GameObject("NightOverlay");
        overlay = go.AddComponent<SpriteRenderer>();
        overlay.sprite = PlaceholderSprites.CreateSolidSquare(Color.white);
        overlay.color = new Color(0.05f, 0.07f, 0.20f, 0f); // xanh đêm, trong suốt lúc ban ngày
        overlay.sortingOrder = 50; // trên mọi thứ trong thế giới; HUD (OnGUI) vẫn nằm trên
    }

    private void Update()
    {
        clock += Time.deltaTime;
        if (clock >= Cycle)
        {
            clock -= Cycle;
            DayNumber++;
        }
    }

    private void LateUpdate()
    {
        if (overlay == null || cam == null) return;

        // Phủ kín vùng nhìn của camera (thêm lề cho chắc).
        float h = cam.orthographicSize * 2f + 2f;
        float w = h * cam.aspect + 2f;
        overlay.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
        overlay.transform.localScale = new Vector3(w, h, 1f);

        var c = overlay.color;
        c.a = NightAmount * maxDarkness;
        overlay.color = c;
    }
}
