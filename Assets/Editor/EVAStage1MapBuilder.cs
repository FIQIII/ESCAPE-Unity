using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public static class EVAStage1MapBuilder
{
    private const string ROOT_NAME = "Map_Stage1";
    private const string MATERIAL_FOLDER = "Assets/Stage1_Generated/Materials";

    [MenuItem("EVA/Build Stage 1 Map - Laporan (Terang)")]
    public static void BuildStage1()
    {
        GameObject root = GameObject.Find(ROOT_NAME);

        if (root == null)
        {
            root = new GameObject(ROOT_NAME);
            root.transform.position = Vector3.zero;
        }
        else if (root.transform.childCount > 0)
        {
            bool ok = EditorUtility.DisplayDialog(
                "Build Stage 1",
                "Map_Stage1 sudah berisi objek. Hapus isi Map_Stage1 lalu buat ulang sesuai laporan?",
                "Ya, Buat Ulang",
                "Batal"
            );

            if (!ok) return;

            for (int i = root.transform.childCount - 1; i >= 0; i--)
                Object.DestroyImmediate(root.transform.GetChild(i).gameObject);
        }

        EnsureFolder("Assets/Stage1_Generated");
        EnsureFolder(MATERIAL_FOLDER);

        Material floorMat   = MakeMaterial("M_Floor",     Hex("#596773"), 0.15f, 0.35f);
        Material wallMat    = MakeMaterial("M_Wall",      Hex("#66737F"), 0.10f, 0.25f);
        Material ceilingMat = MakeMaterial("M_Ceiling",   Hex("#4A5661"), 0.05f, 0.20f);
        Material metalMat   = MakeMaterial("M_Metal",     Hex("#74818C"), 0.45f, 0.40f);
        Material coverMat   = MakeMaterial("M_Cover",     Hex("#5E6973"), 0.20f, 0.25f);
        Material cyanMat    = MakeEmissionMaterial("M_Cyan",  Hex("#28C7E8"), 2.0f);
        Material amberMat   = MakeEmissionMaterial("M_Amber", Hex("#E5822C"), 2.0f);
        Material redMat     = MakeEmissionMaterial("M_Red",   Hex("#E33A35"), 2.3f);
        Material cardMat    = MakeEmissionMaterial("M_AccessCard", Hex("#D9F54A"), 1.7f);

        Transform geo = NewEmpty("01_GEOMETRY", root.transform);
        Transform props = NewEmpty("02_PROPS", root.transform);
        Transform markers = NewEmpty("03_GAMEPLAY_MARKERS", root.transform);
        Transform lighting = NewEmpty("04_LIGHTING", root.transform);

        CreateCube("Floor_Main", geo, new Vector3(0f, -0.10f, 0f), new Vector3(36f, 0.20f, 24f), floorMat);

        CreateCube("Wall_North", geo, new Vector3(0f, 1.6f, 12f), new Vector3(36f, 3.2f, 0.3f), wallMat);
        CreateCube("Wall_South", geo, new Vector3(0f, 1.6f, -12f), new Vector3(36f, 3.2f, 0.3f), wallMat);
        CreateCube("Wall_West", geo, new Vector3(-18f, 1.6f, 0f), new Vector3(0.3f, 3.2f, 24f), wallMat);
        CreateCube("Wall_East_North", geo, new Vector3(18f, 1.6f, 7.5f), new Vector3(0.3f, 3.2f, 9f), wallMat);
        CreateCube("Wall_East_South", geo, new Vector3(18f, 1.6f, -7.5f), new Vector3(0.3f, 3.2f, 9f), wallMat);

        GameObject ceiling = CreateCube("Ceiling_Main_ENABLE_LATER", geo, new Vector3(0f, 3.25f, 0f), new Vector3(36f, 0.25f, 24f), ceilingMat);
        ceiling.SetActive(false);

        Transform lab01 = NewEmpty("LAB_01_Fasilitas_Penahanan", geo);
        CreateCube("LAB01_EastWall_A", lab01, new Vector3(-10f, 1.6f, -10.2f), new Vector3(0.3f, 3.2f, 3.6f), wallMat);
        CreateCube("LAB01_EastWall_B", lab01, new Vector3(-10f, 1.6f, -5.8f), new Vector3(0.3f, 3.2f, 3.6f), wallMat);
        CreateCube("LAB01_NorthWall_A", lab01, new Vector3(-15.2f, 1.6f, -4f), new Vector3(5.6f, 3.2f, 0.3f), wallMat);
        CreateCube("LAB01_NorthWall_B", lab01, new Vector3(-11f, 1.6f, -4f), new Vector3(2.0f, 3.2f, 0.3f), wallMat);

        CreateCube("LAB01_Bed", props, new Vector3(-15.4f, 0.35f, -9.4f), new Vector3(3.0f, 0.7f, 1.2f), metalMat);
        CreateCube("LAB01_Desk", props, new Vector3(-14.8f, 0.55f, -5.5f), new Vector3(2.4f, 1.1f, 0.9f), metalMat);
        CreateCube("LAB01_TutorialTerminal", props, new Vector3(-14.8f, 1.25f, -5.4f), new Vector3(1.0f, 0.5f, 0.20f), cyanMat);
        NewMarker("Player_Spawn_LAB01", markers, new Vector3(-15f, 0.95f, -9f));
        NewMarker("Tutorial_Interaction", markers, new Vector3(-14.8f, 1.0f, -5.7f));

        Transform survivor = NewEmpty("Area_NPC_Penyintas", geo);
        CreateCube("Survivor_SouthWall_A", survivor, new Vector3(-15.5f, 1.6f, -1.8f), new Vector3(5.0f, 3.2f, 0.3f), wallMat);
        CreateCube("Survivor_SouthWall_B", survivor, new Vector3(-10.8f, 1.6f, -1.8f), new Vector3(1.6f, 3.2f, 0.3f), wallMat);
        CreateCube("Survivor_EastWall_A", survivor, new Vector3(-9.9f, 1.6f, -0.3f), new Vector3(0.3f, 3.2f, 2.8f), wallMat);
        CreateCube("Survivor_EastWall_B", survivor, new Vector3(-9.9f, 1.6f, 2.1f), new Vector3(0.3f, 3.2f, 1.8f), wallMat);
        CreateCube("Survivor_Cover", props, new Vector3(-14.0f, 0.8f, 0.7f), new Vector3(2.5f, 1.6f, 1.0f), coverMat);
        NewMarker("NPC_Penyintas_01_Spawn", markers, new Vector3(-13.5f, 0f, 0.4f));

        Transform corridor = NewEmpty("Lorong_Utama_Stealth", geo);
        CreateCube("Divider_West", corridor, new Vector3(-6.5f, 1.6f, 5f), new Vector3(0.3f, 3.2f, 8f), wallMat);
        CreateCube("Divider_Middle", corridor, new Vector3(0.5f, 1.6f, -4.5f), new Vector3(0.3f, 3.2f, 7f), wallMat);
        CreateCube("Divider_East", corridor, new Vector3(7.0f, 1.6f, 4.8f), new Vector3(0.3f, 3.2f, 7.6f), wallMat);

        CreateCube("Cover_01", props, new Vector3(-5.0f, 0.9f, -0.5f), new Vector3(2.0f, 1.8f, 1.2f), coverMat);
        CreateCube("Cover_02", props, new Vector3(1.8f, 0.9f, 2.0f), new Vector3(2.2f, 1.8f, 1.2f), coverMat);
        CreateCube("Cover_03", props, new Vector3(6.2f, 0.9f, -1.5f), new Vector3(2.0f, 1.8f, 1.2f), coverMat);

        CreateCylinder("Lab_Pillar_01", props, new Vector3(-3.5f, 1.4f, 7.5f), 0.75f, 2.8f, metalMat);
        CreateCylinder("Lab_Pillar_02", props, new Vector3(1.5f, 1.4f, 7.5f), 0.75f, 2.8f, metalMat);
        CreateCylinder("Lab_Pillar_03", props, new Vector3(5.5f, 1.4f, 7.5f), 0.75f, 2.8f, metalMat);

        Transform accessRoom = NewEmpty("Access_Card_Room", geo);
        CreateCube("Access_WestWall_A", accessRoom, new Vector3(9f, 1.6f, -9.5f), new Vector3(0.3f, 3.2f, 4.5f), wallMat);
        CreateCube("Access_WestWall_B", accessRoom, new Vector3(9f, 1.6f, -5.0f), new Vector3(0.3f, 3.2f, 2.0f), wallMat);
        CreateCube("Access_NorthWall", accessRoom, new Vector3(13.5f, 1.6f, -4f), new Vector3(9f, 3.2f, 0.3f), wallMat);

        CreateCube("Access_Table", props, new Vector3(13f, 0.55f, -9.0f), new Vector3(3.0f, 1.1f, 1.1f), metalMat);
        CreateCube("AccessCard_Stage1", props, new Vector3(13f, 1.18f, -9.0f), new Vector3(0.45f, 0.08f, 0.70f), cardMat);
        NewMarker("AccessCard_Interaction_Point", markers, new Vector3(13f, 1f, -8.2f));

        Transform generatorRoom = NewEmpty("Power_Generator_Room", geo);
        CreateCube("Generator_WestWall_A", generatorRoom, new Vector3(9f, 1.6f, 5.2f), new Vector3(0.3f, 3.2f, 2.4f), wallMat);
        CreateCube("Generator_WestWall_B", generatorRoom, new Vector3(9f, 1.6f, 9.7f), new Vector3(0.3f, 3.2f, 4.6f), wallMat);
        CreateCube("Generator_SouthWall", generatorRoom, new Vector3(13.5f, 1.6f, 4f), new Vector3(9f, 3.2f, 0.3f), wallMat);

        CreateCube("PowerGenerator", props, new Vector3(14.2f, 0.95f, 9.0f), new Vector3(3.8f, 1.9f, 2.2f), amberMat);
        CreateCube("Terminal_Generator", props, new Vector3(11.0f, 1.0f, 6.2f), new Vector3(1.2f, 2.0f, 0.7f), cyanMat);

        NewMarker("PowerGenerator_Interaction", markers, new Vector3(13.2f, 1f, 8.0f));
        NewMarker("Terminal_Misi_Stage1", markers, new Vector3(11.0f, 1f, 5.4f));

        Transform exit = NewEmpty("Exit_Gate_Area", geo);
        CreateCube("ExitFrame_North", exit, new Vector3(17.85f, 2.65f, 1.8f), new Vector3(0.4f, 0.9f, 1.5f), redMat);
        CreateCube("ExitFrame_South", exit, new Vector3(17.85f, 2.65f, -1.8f), new Vector3(0.4f, 0.9f, 1.5f), redMat);
        CreateCube("ExitFrame_Top", exit, new Vector3(17.85f, 2.9f, 0f), new Vector3(0.4f, 0.6f, 2.2f), redMat);
        CreateCube("ExitGate_Door_LOCKED", exit, new Vector3(17.72f, 1.35f, 0f), new Vector3(0.25f, 2.7f, 2.6f), wallMat);

        NewMarker("ExitGate_Trigger", markers, new Vector3(16.6f, 0f, 0f));
        NewMarker("Stage2_Transition", markers, new Vector3(19.5f, 0f, 0f));

        Transform patrol1 = NewEmpty("RobotPatrol_01_Route", markers);
        NewMarker("P1_A", patrol1, new Vector3(-8f, 0f, -1f));
        NewMarker("P1_B", patrol1, new Vector3(-2f, 0f, -1f));
        NewMarker("P1_C", patrol1, new Vector3(-2f, 0f, 4f));

        Transform patrol2 = NewEmpty("RobotPatrol_02_Route", markers);
        NewMarker("P2_A", patrol2, new Vector3(1.8f, 0f, -7f));
        NewMarker("P2_B", patrol2, new Vector3(5.5f, 0f, -4f));
        NewMarker("P2_C", patrol2, new Vector3(8.0f, 0f, -7f));

        Transform patrol3 = NewEmpty("RobotPatrol_03_Route", markers);
        NewMarker("P3_A", patrol3, new Vector3(8.5f, 0f, 2f));
        NewMarker("P3_B", patrol3, new Vector3(12f, 0f, 2f));
        NewMarker("P3_C", patrol3, new Vector3(15f, 0f, 3f));

        NewMarker("MiniBoss_Stage1_Spawn", markers, new Vector3(14.5f, 0f, 0.5f));
        NewMarker("MiniBoss_Chase_Trigger", markers, new Vector3(15.5f, 0f, 0f));

        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = Hex("#596B7A");
        RenderSettings.reflectionIntensity = 0.30f;

        GameObject directionalObject = GameObject.Find("Directional Light");
        if (directionalObject != null)
        {
            Light directional = directionalObject.GetComponent<Light>();
            if (directional != null)
            {
                directional.intensity = 0.65f;
                directional.color = Hex("#C8D7E6");
            }
        }

        CreatePointLight("Light_LAB01", lighting, new Vector3(-14f, 2.3f, -7.5f), Hex("#8EC6DE"), 3.0f, 8f);
        CreatePointLight("Light_Survivor", lighting, new Vector3(-13f, 2.3f, 0.5f), Hex("#8FA7AF"), 2.2f, 7f);
        CreatePointLight("Light_Corridor_01", lighting, new Vector3(-3f, 2.4f, 0f), Hex("#6CA8B8"), 2.4f, 8f);
        CreatePointLight("Light_Corridor_02", lighting, new Vector3(4f, 2.4f, 0f), Hex("#6CA8B8"), 2.4f, 8f);
        CreatePointLight("Light_AccessCard", lighting, new Vector3(13f, 2.2f, -8.0f), Hex("#3BC9E8"), 2.0f, 6f);
        CreatePointLight("Light_Generator", lighting, new Vector3(14f, 2.2f, 8.5f), Hex("#E47A27"), 2.0f, 6f);
        CreatePointLight("Light_Exit", lighting, new Vector3(16f, 2.3f, 0f), Hex("#E74747"), 2.2f, 6f);

        Selection.activeGameObject = root;
        EditorGUIUtility.PingObject(root);

        Debug.Log("Stage 1 berhasil dibuat sesuai laporan. Ceiling dinonaktifkan sementara agar mudah diedit.");
    }

    private static Transform NewEmpty(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go.transform;
    }

    private static Transform NewMarker(string name, Transform parent, Vector3 position)
    {
        Transform t = NewEmpty(name, parent);
        t.position = position;
        return t;
    }

    private static GameObject CreateCube(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.position = position;
        go.transform.localScale = scale;

        Renderer renderer = go.GetComponent<Renderer>();
        if (renderer != null && material != null)
            renderer.sharedMaterial = material;

        return go;
    }

    private static GameObject CreateCylinder(string name, Transform parent, Vector3 position, float radius, float height, Material material)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.position = position;
        go.transform.localScale = new Vector3(radius, height * 0.5f, radius);

        Renderer renderer = go.GetComponent<Renderer>();
        if (renderer != null && material != null)
            renderer.sharedMaterial = material;

        return go;
    }

    private static void CreatePointLight(string name, Transform parent, Vector3 position, Color color, float intensity, float range)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.position = position;

        Light light = go.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.intensity = intensity;
        light.range = range;
        light.shadows = LightShadows.None;
    }

    private static Material MakeMaterial(string name, Color color, float metallic, float smoothness)
    {
        string path = MATERIAL_FOLDER + "/" + name + ".mat";
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (existing != null)
        {
            SetBaseColor(existing, color);
            SetFloat(existing, "_Metallic", metallic);
            SetFloat(existing, "_Smoothness", smoothness);
            EditorUtility.SetDirty(existing);
            return existing;
        }

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        Material mat = new Material(shader);
        mat.name = name;

        SetBaseColor(mat, color);
        SetFloat(mat, "_Metallic", metallic);
        SetFloat(mat, "_Smoothness", smoothness);

        AssetDatabase.CreateAsset(mat, path);
        AssetDatabase.SaveAssets();
        return mat;
    }

    private static Material MakeEmissionMaterial(string name, Color color, float emissionStrength)
    {
        Material mat = MakeMaterial(name, color, 0.25f, 0.35f);

        if (mat.HasProperty("_EmissionColor"))
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", color * emissionStrength);
        }

        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();
        return mat;
    }

    private static void SetBaseColor(Material mat, Color color)
    {
        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);
        else if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", color);
    }

    private static void SetFloat(Material mat, string property, float value)
    {
        if (mat.HasProperty(property))
            mat.SetFloat(property, value);
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

    private static Color Hex(string value)
    {
        Color color = Color.white;
        ColorUtility.TryParseHtmlString(value, out color);
        return color;
    }
}