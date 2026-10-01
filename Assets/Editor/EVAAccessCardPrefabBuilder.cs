using UnityEngine;
using UnityEditor;

public static class EVAAccessCardPrefabBuilder
{
    private const string PREFAB_FOLDER = "Assets/Prefabs/Items";
    private const string MATERIAL_FOLDER = "Assets/Materials/EVA_AccessCard";

    [MenuItem("EVA/Create Stage 1 Access Card")]
    public static void BuildAccessCard()
    {
        EnsureFolder("Assets/Prefabs");
        EnsureFolder(PREFAB_FOLDER);
        EnsureFolder("Assets/Materials");
        EnsureFolder(MATERIAL_FOLDER);

        Material bodyMat = CreateMaterial(
            "M_AccessCard_Body",
            new Color(0.08f, 0.12f, 0.15f),
            0.35f,
            0.45f
        );

        Material trimMat = CreateMaterial(
            "M_AccessCard_Trim",
            new Color(0.55f, 0.65f, 0.70f),
            0.65f,
            0.50f
        );

        Material glowMat = CreateEmissionMaterial(
            "M_AccessCard_Glow",
            new Color(0.40f, 0.95f, 0.18f),
            2.7f
        );

        GameObject root = new GameObject("AccessCard_EVA_Stage1");
        root.transform.position = Vector3.zero;
        root.transform.rotation = Quaternion.identity;
        root.transform.localScale = Vector3.one;

        // Bentuk kartu sci-fi.
        CreateCube(
            "Card_Body",
            root.transform,
            Vector3.zero,
            new Vector3(0.82f, 0.055f, 0.50f),
            bodyMat
        );

        CreateCube(
            "Card_TopTrim",
            root.transform,
            new Vector3(0f, 0.035f, 0.18f),
            new Vector3(0.72f, 0.035f, 0.055f),
            trimMat
        );

        CreateCube(
            "Card_GlowStripe",
            root.transform,
            new Vector3(0f, 0.045f, -0.12f),
            new Vector3(0.62f, 0.025f, 0.045f),
            glowMat
        );

        CreateCube(
            "Card_Chip",
            root.transform,
            new Vector3(-0.22f, 0.045f, 0.02f),
            new Vector3(0.16f, 0.025f, 0.16f),
            trimMat
        );

        CreateCube(
            "Card_CodeBar_1",
            root.transform,
            new Vector3(0.18f, 0.045f, 0.04f),
            new Vector3(0.035f, 0.025f, 0.18f),
            glowMat
        );

        CreateCube(
            "Card_CodeBar_2",
            root.transform,
            new Vector3(0.25f, 0.045f, 0.04f),
            new Vector3(0.020f, 0.025f, 0.18f),
            glowMat
        );

        CreateCube(
            "Card_CodeBar_3",
            root.transform,
            new Vector3(0.31f, 0.045f, 0.04f),
            new Vector3(0.040f, 0.025f, 0.18f),
            glowMat
        );

        // Label 3D sederhana.
        GameObject textObject = new GameObject("Label_EVA");
        textObject.transform.SetParent(root.transform, false);
        textObject.transform.localPosition = new Vector3(-0.34f, 0.065f, -0.20f);
        textObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        textObject.transform.localScale = Vector3.one * 0.035f;

        TextMesh text = textObject.AddComponent<TextMesh>();
        text.text = "EVA ACCESS\nSECTOR ALPHA";
        text.fontSize = 32;
        text.characterSize = 0.12f;
        text.anchor = TextAnchor.LowerLeft;
        text.alignment = TextAlignment.Left;
        text.color = new Color(0.75f, 0.95f, 1f);

        // Collider untuk raycast interaksi.
        BoxCollider collider = root.AddComponent<BoxCollider>();
        collider.center = Vector3.zero;
        collider.size = new Vector3(0.90f, 0.18f, 0.58f);

        root.AddComponent<AccessCardPickup>();

        string prefabPath = PREFAB_FOLDER + "/AccessCard_EVA_Stage1.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);

        Object.DestroyImmediate(root);

        // Otomatis pasang ke marker/placeholder map jika ditemukan.
        GameObject oldPlaceholder = GameObject.Find("AccessCard_Stage1");

        if (oldPlaceholder != null && prefab != null)
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            instance.name = "AccessCard_EVA_Stage1";
            instance.transform.SetParent(oldPlaceholder.transform.parent, true);
            instance.transform.position = oldPlaceholder.transform.position + new Vector3(0f, 0.18f, 0f);
            instance.transform.rotation = Quaternion.Euler(0f, 25f, 0f);
            instance.transform.localScale = Vector3.one;

            oldPlaceholder.SetActive(false);

            Selection.activeGameObject = instance;
            EditorGUIUtility.PingObject(instance);
        }
        else
        {
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            "Access Card Stage 1 berhasil dibuat. " +
            "Tambahkan PlayerInventoryEVA ke Player. " +
            "Tekan E saat melihat kartu untuk mengambilnya."
        );
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

        Renderer renderer = go.GetComponent<Renderer>();
        if (renderer != null)
            renderer.sharedMaterial = material;

        Collider primitiveCollider = go.GetComponent<Collider>();
        if (primitiveCollider != null)
            Object.DestroyImmediate(primitiveCollider);

        return go;
    }

    private static Material CreateMaterial(
        string name,
        Color color,
        float metallic,
        float smoothness)
    {
        string path = MATERIAL_FOLDER + "/" + name + ".mat";
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (existing != null)
            return existing;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        Material mat = new Material(shader);
        mat.name = name;

        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);
        else if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", color);

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
        Material mat = CreateMaterial(name, color, 0.25f, 0.40f);

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

        string[] parts = path.Split('/');
        string current = parts[0];

        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];

            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);

            current = next;
        }
    }
}
