using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Script Editor tự dựng scene khởi đầu cho game nông trại (Story 1).
///
/// Dùng được 2 cách:
///  - Trong Unity: menu "FarmGame/Build Starter Scene".
///  - Tự động (batch): Unity.exe -batchmode -quit -executeMethod FarmGameBootstrap.Build
///
/// Nó tạo: scene Farm.unity + Camera (nền xanh cỏ) + Player (ô vuông cam có script di chuyển),
/// sinh sprite ô vuông bằng code để chạy được ngay khi chưa có art ansimuz.
/// </summary>
public static class FarmGameBootstrap
{
    private const string ScenesDir = "Assets/Scenes";
    private const string ScenePath = ScenesDir + "/Farm.unity";
    private const string ArtDir = "Assets/Art/Generated";
    private const string SquarePngPath = ArtDir + "/square_white.png";

    [MenuItem("FarmGame/Build Starter Scene")]
    public static void Build()
    {
        // Đặt project về chế độ 2D (import ảnh mới mặc định thành Sprite, Scene view 2D).
        EditorSettings.defaultBehaviorMode = EditorBehaviorMode.Mode2D;

        // Tạo sprite ô vuông trắng (lưu thành PNG asset để tồn tại lâu dài).
        Sprite squareSprite = CreateSquareSprite();

        // Tạo scene trống mới.
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // --- Camera ---
        var cameraGO = new GameObject("Main Camera");
        var camera = cameraGO.AddComponent<Camera>();
        camera.orthographic = true;              // chiếu phẳng cho 2D
        camera.orthographicSize = 5f;            // độ "zoom"
        camera.backgroundColor = new Color(0.45f, 0.65f, 0.35f); // xanh cỏ
        camera.clearFlags = CameraClearFlags.SolidColor;
        cameraGO.transform.position = new Vector3(0f, 0f, -10f);
        cameraGO.tag = "MainCamera";

        // --- Player ---
        var playerGO = new GameObject("Player");
        var renderer = playerGO.AddComponent<SpriteRenderer>();
        renderer.sprite = squareSprite;
        renderer.color = new Color(0.90f, 0.50f, 0.20f); // cam (tạm cho nhân vật)
        playerGO.AddComponent<PlayerMovement>();
        playerGO.transform.position = Vector3.zero;

        // Lưu scene.
        Directory.CreateDirectory(ScenesDir);
        EditorSceneManager.SaveScene(scene, ScenePath);

        // Đặt Farm.unity là scene đầu tiên trong Build Settings.
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };

        AssetDatabase.SaveAssets();
        Debug.Log("[FarmGame] Đã dựng scene khởi đầu: " + ScenePath);
    }

    /// <summary>
    /// Sinh 1 texture ô vuông trắng 16x16, lưu thành PNG asset rồi trả về Sprite.
    /// Lưu thành asset để sprite không bị mất khi đóng/mở lại editor.
    /// </summary>
    private static Sprite CreateSquareSprite()
    {
        const int size = 16;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        texture.SetPixels(pixels);
        texture.Apply();

        byte[] png = texture.EncodeToPNG();
        Object.DestroyImmediate(texture);

        Directory.CreateDirectory(ArtDir);
        File.WriteAllBytes(SquarePngPath, png);
        AssetDatabase.ImportAsset(SquarePngPath, ImportAssetOptions.ForceSynchronousImport);

        // Cấu hình import ảnh thành Sprite (pixel-art: filter Point).
        var importer = (TextureImporter)AssetImporter.GetAtPath(SquarePngPath);
        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = size;      // 16px = 1 đơn vị world
        importer.filterMode = FilterMode.Point;   // nét, không mờ (pixel-art)
        importer.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Sprite>(SquarePngPath);
    }
}
