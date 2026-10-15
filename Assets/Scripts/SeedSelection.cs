using UnityEngine;

/// <summary>
/// Story 13: Nhớ người chơi đang chọn loại hạt NÀO để gieo, và cho đổi bằng phím:
///   - Phím 1 / 2: chọn thẳng loại tương ứng.
///   - Phím Q: xoay vòng sang loại kế tiếp.
/// FarmInteraction đọc để biết trồng cây gì; GameHUD đọc để hiện hạt đang chọn.
/// </summary>
public class SeedSelection : MonoBehaviour
{
    public int SelectedType { get; private set; }

    private int typeCount = 1;

    public void Setup(int typeCount) => this.typeCount = Mathf.Max(1, typeCount);

    /// <summary>Chọn thẳng 1 loại (dùng khi nạp game đã lưu).</summary>
    public void Select(int type)
    {
        if (type >= 0 && type < typeCount) SelectedType = type;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectedType = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2) && typeCount > 1) SelectedType = 1;
        else if (Input.GetKeyDown(KeyCode.Q)) SelectedType = (SelectedType + 1) % typeCount;
    }
}
