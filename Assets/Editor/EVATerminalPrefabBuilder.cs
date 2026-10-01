using UnityEngine;
using UnityEditor;

public static class EVATerminalPrefabBuilder
{
    private const string PREFAB_FOLDER = "Assets/Prefabs/Environment";
    private const string MATERIAL_FOLDER = "Assets/Materials/EVA_Terminal";

    [MenuItem("EVA/Create Stage 1 Terminal")]
    public static void BuildTerminal()
    {
        EnsureFolder("Assets/Prefabs");
        EnsureFolder(PREFAB_FOLDER);
        EnsureFolder("Assets/Materials");
        EnsureFolder(MATERIAL_FOLDER);

        Material bodyMat = CreateMaterial("M_Terminal_Body", new Color(0.07f,0.10f,0.13f), 0.65f, 0.35f);
        Material metalMat = CreateMaterial("M_Terminal_Metal", new Color(0.30f,0.38f,0.43f), 0.80f, 0.48f);
        Material darkMat = CreateMaterial("M_Terminal_Dark", new Color(0.025f,0.035f,0.045f), 0.45f, 0.25f);
        Material screenMat = CreateEmissionMaterial("M_Terminal_Screen", new Color(1.0f,0.10f,0.04f), 3.0f);

        GameObject root = new GameObject("Terminal_EVA_Stage1");
        Transform body = NewEmpty("Body", root.transform);
        Transform panel = NewEmpty("Panel", root.transform);

        CreateCube("Terminal_Base", body, new Vector3(0f,0.12f,0f), new Vector3(1.35f,0.24f,0.85f), darkMat);
        CreateCube("Terminal_Column", body, new Vector3(0f,0.95f,0.15f), new Vector3(0.92f,1.65f,0.62f), bodyMat);
        CreateCube("Terminal_Top", body, new Vector3(0f,1.82f,0.02f), new Vector3(1.15f,0.25f,0.85f), metalMat);

        GameObject screen = CreateCube("Terminal_Screen", panel, new Vector3(0f,1.34f,-0.35f), new Vector3(0.68f,0.44f,0.06f), screenMat);
        CreateCube("Terminal_Button_A", panel, new Vector3(-0.22f,0.92f,-0.36f), new Vector3(0.14f,0.11f,0.05f), metalMat);
        CreateCube("Terminal_Button_B", panel, new Vector3(0f,0.92f,-0.36f), new Vector3(0.14f,0.11f,0.05f), metalMat);
        CreateCube("Terminal_Button_C", panel, new Vector3(0.22f,0.92f,-0.36f), new Vector3(0.14f,0.11f,0.05f), metalMat);

        for (int i = -2; i <= 2; i++)
            CreateCube("Vent_" + (i + 3), body, new Vector3(i*0.14f,0.50f,0.47f), new Vector3(0.07f,0.28f,0.05f), darkMat);

        GameObject lightObject = new GameObject("Terminal_Status_Light");
        lightObject.transform.SetParent(root.transform, false);
        lightObject.transform.localPosition = new Vector3(0f,1.95f,-0.45f);

        Light terminalLight = lightObject.AddComponent<Light>();
        terminalLight.type = LightType.Point;
        terminalLight.color = new Color(1f,0.10f,0.04f);
        terminalLight.intensity = 1.2f;
        terminalLight.range = 3.5f;
        terminalLight.shadows = LightShadows.None;

        BoxCollider box = root.AddComponent<BoxCollider>();
        box.center = new Vector3(0f,0.95f,0f);
        box.size = new Vector3(1.45f,2.0f,1.0f);

        TerminalEVA terminal = root.AddComponent<TerminalEVA>();
        terminal.screenRenderer = screen.GetComponent<Renderer>();
        terminal.terminalLight = terminalLight;

        string prefabPath = PREFAB_FOLDER + "/Terminal_EVA_Stage1.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        Object.DestroyImmediate(root);

        GameObject placeholder = GameObject.Find("Terminal_Generator");

        if (placeholder != null && prefab != null)
        {
            Vector3 oldPosition = placeholder.transform.position;
            Renderer oldRenderer = placeholder.GetComponent<Renderer>();
            float floorY = oldRenderer != null ? oldRenderer.bounds.min.y : 0f;

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.name = "Terminal_EVA_Stage1";
            instance.transform.SetParent(placeholder.transform.parent, true);
            instance.transform.position = new Vector3(oldPosition.x, floorY, oldPosition.z);
            instance.transform.rotation = Quaternion.Euler(0f,180f,0f);
            instance.transform.localScale = Vector3.one;

            placeholder.SetActive(false);

            TerminalEVA instanceTerminal = instance.GetComponent<TerminalEVA>();

            GameObject generatorObject = GameObject.Find("PowerGenerator_EVA_Stage1");
            if (generatorObject != null)
                instanceTerminal.generator = generatorObject.GetComponent<PowerGeneratorEVA>();

            GameObject doorObject = GameObject.Find("Door_EVA");
            if (doorObject != null)
            {
                EVADoorController door = doorObject.GetComponent<EVADoorController>();
                instanceTerminal.exitGate = door;

                if (door != null)
                {
                    door.locked = true;
                    EditorUtility.SetDirty(door);
                }
            }

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

        Debug.Log("Terminal EVA Stage 1 berhasil dibuat. Butuh Access Card + Generator aktif. Setelah terminal aktif, Exit Gate unlock.");
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

    private static Material CreateMaterial(string name, Color color, float metallic, float smoothness)
    {
        string path = MATERIAL_FOLDER + "/" + name + ".mat";
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) return existing;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        Material mat = new Material(shader);
        mat.name = name;

        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
        else if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);

        if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);

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
        if (AssetDatabase.IsValidFolder(path)) return;

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