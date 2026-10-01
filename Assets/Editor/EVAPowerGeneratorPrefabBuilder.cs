using UnityEngine;
using UnityEditor;

public static class EVAPowerGeneratorPrefabBuilder
{
    private const string PREFAB_FOLDER = "Assets/Prefabs/Environment";
    private const string MATERIAL_FOLDER = "Assets/Materials/EVA_Generator";

    [MenuItem("EVA/Create Stage 1 Power Generator")]
    public static void BuildGenerator()
    {
        EnsureFolder("Assets/Prefabs");
        EnsureFolder(PREFAB_FOLDER);
        EnsureFolder("Assets/Materials");
        EnsureFolder(MATERIAL_FOLDER);

        Material bodyMat = CreateMaterial("M_Generator_Body", new Color(0.10f, 0.13f, 0.16f), 0.70f, 0.35f);
        Material metalMat = CreateMaterial("M_Generator_Metal", new Color(0.32f, 0.38f, 0.42f), 0.85f, 0.50f);
        Material darkMat = CreateMaterial("M_Generator_Dark", new Color(0.035f, 0.05f, 0.065f), 0.55f, 0.25f);
        Material orangeMat = CreateEmissionMaterial("M_Generator_Energy", new Color(1.0f, 0.35f, 0.05f), 2.8f);
        Material redMat = CreateEmissionMaterial("M_Generator_Status", new Color(1.0f, 0.05f, 0.03f), 3.0f);

        GameObject root = new GameObject("PowerGenerator_EVA_Stage1");
        root.transform.position = Vector3.zero;
        root.transform.rotation = Quaternion.identity;
        root.transform.localScale = Vector3.one;

        Transform shell = NewEmpty("Shell", root.transform);
        Transform energy = NewEmpty("EnergyCore", root.transform);
        Transform control = NewEmpty("ControlPanel", root.transform);

        CreateCube("Generator_Base", shell, new Vector3(0f, 0.16f, 0f), new Vector3(3.6f, 0.32f, 1.9f), darkMat);
        CreateCube("Generator_MainBody", shell, new Vector3(0f, 1.05f, 0f), new Vector3(3.25f, 1.55f, 1.65f), bodyMat);
        CreateCube("Generator_Top", shell, new Vector3(0f, 1.95f, 0f), new Vector3(3.45f, 0.28f, 1.85f), metalMat);
        CreateCube("Armor_Left", shell, new Vector3(-1.62f, 1.05f, 0f), new Vector3(0.24f, 1.65f, 1.72f), metalMat);
        CreateCube("Armor_Right", shell, new Vector3(1.62f, 1.05f, 0f), new Vector3(0.24f, 1.65f, 1.72f), metalMat);

        for (int i = -1; i <= 1; i++)
        {
            float x = i * 0.78f;
            CreateCylinder("EnergyTube_" + (i + 2), energy, new Vector3(x, 1.12f, -0.88f), 0.22f, 1.20f, orangeMat, Quaternion.Euler(90f, 0f, 0f));
        }

        GameObject turbine = CreateCylinder("Turbine", energy, new Vector3(0f, 1.05f, -0.94f), 0.48f, 0.13f, metalMat, Quaternion.Euler(90f, 0f, 0f));
        CreateCylinder("Turbine_Core", turbine.transform, Vector3.zero, 0.18f, 0.18f, orangeMat, Quaternion.identity);

        for (int i = -2; i <= 2; i++)
        {
            CreateCube("Vent_" + (i + 3), shell, new Vector3(i * 0.45f, 1.55f, 0.86f), new Vector3(0.24f, 0.48f, 0.08f), darkMat);
        }

        CreateCube("Panel_Body", control, new Vector3(1.20f, 1.22f, -1.02f), new Vector3(0.70f, 0.85f, 0.18f), darkMat);
        CreateCube("Panel_Screen", control, new Vector3(1.20f, 1.43f, -1.13f), new Vector3(0.48f, 0.28f, 0.06f), orangeMat);
        GameObject indicator = CreateCube("Status_Indicator", control, new Vector3(1.20f, 1.02f, -1.14f), new Vector3(0.20f, 0.12f, 0.05f), redMat);
        CreateCube("Interact_Button", control, new Vector3(1.20f, 0.82f, -1.14f), new Vector3(0.18f, 0.16f, 0.05f), metalMat);

        GameObject lightObject = new GameObject("Generator_Status_Light");
        lightObject.transform.SetParent(root.transform, false);
        lightObject.transform.localPosition = new Vector3(0f, 2.25f, -0.7f);

        Light statusLight = lightObject.AddComponent<Light>();
        statusLight.type = LightType.Point;
        statusLight.color = new Color(1f, 0.08f, 0.03f);
        statusLight.intensity = 1.1f;
        statusLight.range = 4.5f;
        statusLight.shadows = LightShadows.None;

        BoxCollider box = root.AddComponent<BoxCollider>();
        box.center = new Vector3(0f, 1.05f, 0f);
        box.size = new Vector3(3.7f, 2.2f, 2.0f);

        PowerGeneratorEVA controller = root.AddComponent<PowerGeneratorEVA>();
        controller.statusIndicator = indicator.GetComponent<Renderer>();
        controller.statusLight = statusLight;
        controller.turbine = turbine.transform;

        string prefabPath = PREFAB_FOLDER + "/PowerGenerator_EVA_Stage1.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);

        Object.DestroyImmediate(root);

        GameObject placeholder = GameObject.Find("PowerGenerator");

        if (placeholder != null && prefab != null)
        {
            Vector3 oldPosition = placeholder.transform.position;

            Renderer oldRenderer = placeholder.GetComponent<Renderer>();
            float floorY = 0f;

            if (oldRenderer != null)
                floorY = oldRenderer.bounds.min.y;

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            instance.name = "PowerGenerator_EVA_Stage1";
            instance.transform.SetParent(placeholder.transform.parent, true);
            instance.transform.position = new Vector3(oldPosition.x, floorY, oldPosition.z);
            instance.transform.rotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;

            placeholder.SetActive(false);

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

        Debug.Log("Power Generator EVA Stage 1 berhasil dibuat. Tekan E saat dekat generator untuk mengaktifkannya.");
    }

    private static Transform NewEmpty(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go.transform;
    }

    private static GameObject CreateCube(string name, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = localScale;

        Renderer r = go.GetComponent<Renderer>();
        if (r != null) r.sharedMaterial = material;

        Collider c = go.GetComponent<Collider>();
        if (c != null) Object.DestroyImmediate(c);

        return go;
    }

    private static GameObject CreateCylinder(string name, Transform parent, Vector3 localPosition, float radius, float height, Material material, Quaternion localRotation)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;
        go.transform.localRotation = localRotation;
        go.transform.localScale = new Vector3(radius, height * 0.5f, radius);

        Renderer r = go.GetComponent<Renderer>();
        if (r != null) r.sharedMaterial = material;

        Collider c = go.GetComponent<Collider>();
        if (c != null) Object.DestroyImmediate(c);

        return go;
    }

    private static Material CreateMaterial(string name, Color color, float metallic, float smoothness)
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

    private static Material CreateEmissionMaterial(string name, Color color, float strength)
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