using System.Collections.Generic;

/// <summary>
/// Story 14: Khuôn dữ liệu lưu game. JsonUtility của Unity biến các lớp [Serializable]
/// này thành chuỗi JSON (và ngược lại). Chỉ dùng field public kiểu đơn giản / List.
/// </summary>
[System.Serializable]
public class SaveData
{
    public int coins;
    public int[] seeds;             // số hạt theo loại
    public int[] crops;             // số nông sản theo loại
    public int selectedType;        // loại hạt đang chọn
    public List<TileSave> tilled = new List<TileSave>();
    public List<CropSave> plantedCrops = new List<CropSave>();
}

[System.Serializable]
public class TileSave
{
    public int x;
    public int y;
}

[System.Serializable]
public class CropSave
{
    public int x;
    public int y;
    public int typeId;
    public int stage;
    public float timer;
}
