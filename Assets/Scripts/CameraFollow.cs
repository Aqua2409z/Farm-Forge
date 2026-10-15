using UnityEngine;

/// <summary>
/// Camera bám MƯỢT theo nhân vật (kiểu top-down). Giữ nguyên độ sâu z của camera
/// (camera 2D thường ở z = -10 để nhìn vào mặt phẳng game).
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Tooltip("Càng nhỏ camera càng bám nhanh (giây). ~0.15 là mượt vừa phải.")]
    [SerializeField] private float smoothTime = 0.15f;

    private Transform target;
    private Vector3 velocity; // bộ nhớ nội bộ cho SmoothDamp

    public void Configure(Transform target)
    {
        this.target = target;

        // Nhảy thẳng tới chỗ nhân vật ngay lần đầu (khỏi trượt từ giữa map ra).
        if (target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 goal = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, goal, ref velocity, smoothTime);
    }
}
