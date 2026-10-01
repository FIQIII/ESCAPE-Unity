using UnityEngine;
using UnityEditor;
using System.IO;

public static class EVADoorPrefabBuilder
{
    private const string PREFAB_FOLDER = "Assets/Prefabs/Environment";
    private const string MATERIAL_FOLDER = "Assets/Materials/EVA_Door";

    [MenuItem("EVA/Create Functional EVA Door Prefab")]
    public static void BuildDoorPrefab()
    {
        EnsureFolder("Assets/Prefabs");
        EnsureFolder(PREFAB_FOLDER);
        EnsureFolder("Assets/Materials");
        EnsureFolder(MATERIAL_FOLDER);

        Material frameMat = CreateMaterial(
            "M_Door_Frame",
            new Color(0.12f, 0.15f, 0.18f),
            0.75f,
            0.42f
        );

        Material panelMat = CreateMaterial(
            "M_Door_Panel",
            new Color(0.08f, 0.11f, 0.14f),
            0.65f,
            0.30f
        );

        Material detailMat = CreateMaterial(
            "M_Door_Detail",
            new Color(0.24f, 0.30f, 0.34f),
            0.55f,
            0.35f
        );

        Material glowMat = CreateEmissionMaterial(
            "M_Door_Glow",
            new Color(0.05f, 0.80f, 1.00f),
            3f
        );

        GameObject root = new GameObject("Door_EVA");
        root.transform.position = Vector3.zero;
        root.transform.rotation = Quaternion.identity;
        root.transform.localScale = Vector3.one;

        Transform frame = NewEmpty("Frame", root.transform);
        Transform panels = NewEmpty("Panels", root.transform);
        Transform details = NewEmpty("Details", root.transform);

        // FRAME
        CreateCube("Frame_Left", frame,
            new Vector3(-1.75f, 1.60f, 0f),
            new Vector3(0.35f, 3.20f, 0.45f),
            frameMat);

        CreateCube("Frame_Right", frame,
            new Vector3(1.75f, 1.60f, 0f),
            new Vector3(0.35f, 3.20f, 0.45f),
            frameMat);

        CreateCube("Frame_Top", frame,
            new Vector3(0f, 3.05f, 0f),
            new Vector3(3.85f, 0.35f, 0.45f),
            frameMat);

        CreateCube("Frame_Bottom_Left", frame,
            new Vector3(-1.35f, 0.10f, 0f),
            new Vector3(1.10f, 0.20f, 0.45f),
            frameMat);

        CreateCube("Frame_Bottom_Right", frame,
            new Vector3(1.35f, 0.10f, 0f),
            new Vector3(1.10f, 0.20f, 0.45f),
            frameMat);

        // SLIDING PANELS
        GameObject left = CreateCube(
            "Door_Left",
            panels,
            new Vector3(-0.72f, 1.55f, 0f),
            new Vector3(1.42f, 2.75f, 0.24f),
            panelMat
        );

        GameObject right = CreateCube(
            "Door_Right",
            panels,
            new Vector3(0.72f, 1.55f, 0f),
            new Vector3(1.42f, 2.75f, 0.24f),
            panelMat
        );

        // PANEL DETAILS
        CreateCube("Left_UpperPlate", details,
            new Vector3(-0.72f, 2.25f, -0.14f),
            new Vector3(0.95f, 0.16f, 0.06f),
            detailMat);

        CreateCube("Right_UpperPlate", details,
            new Vector3(0.72f, 2.25f, -0.14f),
            new Vector3(0.95f, 0.16f, 0.06f),
            detailMat);

        CreateCube("Left_CenterLine", left.transform,
            new Vector3(0.43f, 0f, -0.54f),
            new Vector3(0.07f, 0.78f, 0.06f),
            glowMat);

        CreateCube("Right_CenterLine", right.transform,
            new Vector3(-0.43f, 0f, -0.54f),
            new Vector3(0.07f, 0.78f, 0.06f),
            glowMat);

        // HEADER + EVA BADGE AREA
        CreateCube("Header_Panel", details,
            new Vector3(0f, 3.05f, -0.26f),
            new Vector3(1.70f, 0.25f, 0.08f),
            detailMat);

        CreateCube("Header_Glow", details,
            new Vector3(0f, 3.05f, -0.32f),
            new Vector3(0.95f, 0.06f, 0.04f),
            glowMat);

        // CONTROL PANEL
        CreateCube("ControlPanel_Body", details,
            new Vector3(2.10f, 1.45f, -0.20f),
            new Vector3(0.42f, 0.90f, 0.22f),
            detailMat);

        GameObject indicator = CreateCube("Status_Indicator", details,
            new Vector3(2.10f, 1.68f, -0.34f),
            new Vector3(0.22f, 0.15f, 0.06f),
            glowMat);

        CreateCube("ControlPanel_Button", details,
            new Vector3(2.10f, 1.30f, -0.34f),
            new Vector3(0.18f, 0.18f, 0.06f),
            frameMat);

        // Small overhead light
        GameObject lightObject = new GameObject("Door_Status_Light");
        lightObject.transform.SetParent(root.transform, false);
        lightObject.transform.localPosition = new Vector3(0f, 2.85f, -0.50f);

        Light point = lightObject.AddComponent<Light>();
        point.type = LightType.Point;
        point.color = new Color(0.05f, 0.75f, 1f);
        point.intensity = 1.3f;
        point.range = 3.5f;
        point.shadows = LightShadows.None;

        // CONTROLLER
        EVADoorController controller =
            root.AddComponent<EVADoorController>();

        controller.doorLeft = left.transform;
        controller.doorRight = right.transform;
        controller.indicatorRenderer = indicator.GetComponent<Renderer>();
        controller.openDistance = 1.20f;
        controller.openSpeed = 3.5f;
        controller.locked = false;
        controller.autoClose = false;

        // SAVE PREFAB
        string prefabPath = PREFAB_FOLDER + "/Door_EVA.prefab";

        GameObject prefab =
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);

        Object.DestroyImmediate(root);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = prefab;
        EditorGUIUtility.PingObject(prefab);

        Debug.Log(
            "Door_EVA.prefab berhasil dibuat di " + prefabPath +
            ". Drag prefab ke Scene. Tambahkan PlayerInteract ke Player untuk membuka dengan tombol E."
        );
    }

    private static Transform NewEmpty(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go.transform;
    }

    private static GameObject CreateCube(
        string name,
        Transform parent,
        Vector3 localPosition,
        Vector3 localScale,
        Material material)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = localScale;

        Renderer r = go.GetComponent<Renderer>();
        if (r != null)
            r.sharedMaterial = material;

        return go;
    }

    private static Material CreateMaterial(
        string name,
        Color color,
        float metallic,
        float smoothness)
    {
        string path = MATERIAL_FOLDER + "/" + name + ".mat";

        Material existing =
            AssetDatabase.LoadAssetAtPath<Material>(path);

        if (existing != null)
            return existing;

        Shader shader =
            Shader.Find("Universal Render Pipeline/Lit");

        if (shader == null)
            shader = Shader.Find("Standard");

        Material mat = new Material(shader);
        mat.name = name;

        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);
        else
            mat.color = color;

        if (mat.HasProperty("_Metallic"))
            mat.SetFloat("_Metallic", metallic);

        if (mat.HasProperty("_Smoothness"))
            mat.SetFloat("_Smoothness", smoothness);

        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    private static Material CreateEmissionMaterial(
        string name,
        Color color,
        float strength)
    {
        Material mat = CreateMaterial(name, color, 0.35f, 0.40f);

        if (mat.HasProperty("_EmissionColor"))
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", color * strength);
        }

        EditorUtility.SetDirty(mat);
        return mat;
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = Path.GetDirectoryName(path)
            .Replace("\\", "/");

        string folderName = Path.GetFileName(path);

        if (!AssetDatabase.IsValidFolder(parent))
            EnsureFolder(parent);

        AssetDatabase.CreateFolder(parent, folderName);
    }
}
