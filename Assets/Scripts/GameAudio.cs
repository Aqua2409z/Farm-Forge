using UnityEngine;

/// <summary>
/// Âm thanh đơn giản TẠO BẰNG CODE (không cần tải file nhạc): mỗi hành động một tiếng
/// "bíp" ngắn với cao độ khác nhau. Gọi tĩnh: GameAudio.Till(), .Plant(), .Harvest()...
/// </summary>
public class GameAudio : MonoBehaviour
{
    private static GameAudio instance;

    private AudioSource source;
    private AudioClip tillClip, plantClip, harvestClip, sellClip, buyClip;

    private void Awake()
    {
        instance = this;
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;

        // Cần 1 AudioListener để nghe được tiếng; scene chưa có thì gắn vào camera.
        if (Object.FindFirstObjectByType<AudioListener>() == null)
        {
            var cam = Camera.main;
            if (cam != null) cam.gameObject.AddComponent<AudioListener>();
            else gameObject.AddComponent<AudioListener>();
        }

        // (tần số Hz, thời lượng giây) — nhiều đoạn = giai điệu ngắn.
        tillClip    = Build(new[] { (150f, 0.12f) }, 0.5f, square: true);   // "thụp" trầm (cuốc)
        plantClip   = Build(new[] { (430f, 0.10f) }, 0.35f);               // "tích" (gieo)
        harvestClip = Build(new[] { (523f, 0.09f), (784f, 0.12f) }, 0.4f); // "ting-ting" lên (thu hoạch)
        sellClip    = Build(new[] { (880f, 0.07f), (1175f, 0.13f) }, 0.4f);// "cha-ching" (bán)
        buyClip     = Build(new[] { (660f, 0.10f) }, 0.35f);               // "bíp" (mua)
    }

    public static void Till()    => instance?.Play(instance.tillClip);
    public static void Plant()   => instance?.Play(instance.plantClip);
    public static void Harvest() => instance?.Play(instance.harvestClip);
    public static void Sell()    => instance?.Play(instance.sellClip);
    public static void Buy()     => instance?.Play(instance.buyClip);

    private void Play(AudioClip clip)
    {
        if (clip != null) source.PlayOneShot(clip);
    }

    /// <summary>Ghép các đoạn (tần số, thời lượng) thành 1 clip; mỗi đoạn tắt dần (envelope).</summary>
    private AudioClip Build((float freq, float dur)[] segments, float volume, bool square = false)
    {
        const int rate = 44100;

        int total = 0;
        foreach (var s in segments) total += Mathf.CeilToInt(s.dur * rate);

        var data = new float[total];
        int idx = 0;
        foreach (var s in segments)
        {
            int n = Mathf.CeilToInt(s.dur * rate);
            for (int i = 0; i < n; i++)
            {
                float t = (float)i / rate;
                float env = Mathf.Exp(-t * 12f); // tiếng tắt dần cho êm
                float phase = 2f * Mathf.PI * s.freq * t;
                float wave = square ? Mathf.Sign(Mathf.Sin(phase)) : Mathf.Sin(phase);
                data[idx++] = wave * env * volume;
            }
        }

        var clip = AudioClip.Create("sfx", total, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
