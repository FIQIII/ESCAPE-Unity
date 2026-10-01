using UnityEngine;
using UnityEditor;

public static class Stage1SparseRoomProps
{
    private const string MAP_NAME = "Map_Stage1_Generated";
    private const string ENV_ROOT_NAME = "02_ENVIRONMENT_PROPS";
    private const string EXTRA_ROOT_NAME = "SparseRoomProps";

    private static Material metalDark;
    private static Material metalLight;
    private static Material panelDark;
    private static Material screenMat;
    private static Material hazardMat;

    // =========================================================
    // MENU
    // =========================================================

    [MenuItem("Tools/EVA/Add Sparse Room Props")]
    public static void Build()
    {
        GameObject map = GameObject.Find(MAP_NAME);

        if (map == null)
        {
            Debug.LogError("Map_Stage1_Generated tidak ditemukan.");
            return;
        }

        EnsureMaterials();

        Transform environmentRoot =
            FindOrCreateDirectChild(
                map.transform,
                ENV_ROOT_NAME
            );

        Transform oldExtra =
            environmentRoot.Find(EXTRA_ROOT_NAME);

        if (oldExtra != null)
        {
            Object.DestroyImmediate(
                oldExtra.gameObject
            );
        }

        GameObject extraRootObj =
            new GameObject(EXTRA_ROOT_NAME);

        extraRootObj.transform.SetParent(
            environmentRoot,
            false
        );

        Transform extraRoot =
            extraRootObj.transform;

        // =====================================================
        // CARI RUANGAN
        // =====================================================

        Transform stealthRoom =
            FindDeepChild(
                map.transform,
                "Stealth_Zone_Floor"
            );

        Transform storageRoom =
            FindDeepChild(
                map.transform,
                "Side_Lab_Storage_Floor"
            );

        if (stealthRoom != null)
        {
            Bounds stealthBounds =
                GetBounds(stealthRoom);

            BuildStealthZone(
                extraRoot,
                stealthBounds
            );
        }
        else
        {
            Debug.LogWarning(
                "Stealth_Zone_Floor tidak ditemukan."
            );
        }

        if (storageRoom != null)
        {
            Bounds storageBounds =
                GetBounds(storageRoom);

            BuildStorageRoom(
                extraRoot,
                storageBounds
            );
        }
        else
        {
            Debug.LogWarning(
                "Side_Lab_Storage_Floor tidak ditemukan."
            );
        }

        Selection.activeGameObject =
            extraRootObj;

        EditorUtility.SetDirty(map);

        Debug.Log(
            "Sparse room props berhasil dibuat."
        );
    }

    // =========================================================
    // STEALTH ZONE
    // =========================================================

    private static void BuildStealthZone(
        Transform root,
        Bounds b
    )
    {
        Transform group =
            NewGroup(
                root,
                "STEALTH_ZONE_EXTRA"
            );

        float y =
            b.max.y + 0.02f;

        // =====================================================
        // LEFT REAR COVER
        // =====================================================

        CreateCoverWall(
            group,
            new Vector3(
                Mathf.Lerp(
                    b.min.x,
                    b.max.x,
                    0.20f
                ),
                y,
                Mathf.Lerp(
                    b.min.z,
                    b.max.z,
                    0.25f
                )
            ),
            0f
        );

        // =====================================================
        // RIGHT REAR COVER
        // =====================================================

        CreateCoverWall(
            group,
            new Vector3(
                Mathf.Lerp(
                    b.min.x,
                    b.max.x,
                    0.75f
                ),
                y,
                Mathf.Lerp(
                    b.min.z,
                    b.max.z,
                    0.28f
                )
            ),
            90f
        );

        // =====================================================
        // LEFT FRONT MACHINERY
        // =====================================================

        CreateMachineryBlock(
            group,
            new Vector3(
                Mathf.Lerp(
                    b.min.x,
                    b.max.x,
                    0.22f
                ),
                y,
                Mathf.Lerp(
                    b.min.z,
                    b.max.z,
                    0.72f
                )
            ),
            0f
        );

        // =====================================================
        // RIGHT FRONT CRATE STACK
        // =====================================================

        CreateCrateStack(
            group,
            new Vector3(
                Mathf.Lerp(
                    b.min.x,
                    b.max.x,
                    0.78f
                ),
                y,
                Mathf.Lerp(
                    b.min.z,
                    b.max.z,
                    0.73f
                )
            )
        );

        // =====================================================
        // SIDE CANISTERS
        // =====================================================

        CreateCanister(
            group,
            new Vector3(
                b.min.x + 0.9f,
                y,
                b.center.z
            ),
            1.2f
        );

        CreateCanister(
            group,
            new Vector3(
                b.min.x + 1.65f,
                y,
                b.center.z + 0.2f
            ),
            0.95f
        );

        // =====================================================
        // FLOOR WARNING STRIPS
        // =====================================================

        CreateFloorStrip(
            group,
            new Vector3(
                b.center.x - 2.0f,
                y + 0.015f,
                b.center.z - 1.2f
            ),
            new Vector3(
                2.0f,
                0.025f,
                0.18f
            )
        );

        CreateFloorStrip(
            group,
            new Vector3(
                b.center.x + 2.0f,
                y + 0.015f,
                b.center.z + 1.5f
            ),
            new Vector3(
                2.0f,
                0.025f,
                0.18f
            )
        );

        // =====================================================
        // WALL PANELS
        // =====================================================

        CreateWallPanel(
            group,
            new Vector3(
                b.min.x + 0.18f,
                y + 1.35f,
                b.center.z - 1.9f
            ),
            90f
        );

        CreateWallPanel(
            group,
            new Vector3(
                b.max.x - 0.18f,
                y + 1.35f,
                b.center.z + 1.9f
            ),
            -90f
        );
    }

    // =========================================================
    // STORAGE / SIDE LAB
    // =========================================================

    private static void BuildStorageRoom(
        Transform root,
        Bounds b
    )
    {
        Transform group =
            NewGroup(
                root,
                "SIDE_STORAGE_EXTRA"
            );

        float y =
            b.max.y + 0.02f;

        // =====================================================
        // RACK LEFT
        // =====================================================

        CreateStorageRack(
            group,
            new Vector3(
                b.min.x + 0.75f,
                y,
                Mathf.Lerp(
                    b.min.z,
                    b.max.z,
                    0.30f
                )
            ),
            90f
        );

        // =====================================================
        // RACK LEFT BACK
        // =====================================================

        CreateStorageRack(
            group,
            new Vector3(
                b.min.x + 0.75f,
                y,
                Mathf.Lerp(
                    b.min.z,
                    b.max.z,
                    0.72f
                )
            ),
            90f
        );

        // =====================================================
        // WORKBENCH BACK
        // =====================================================

        CreateWorkbench(
            group,
            new Vector3(
                b.center.x,
                y,
                b.max.z - 0.95f
            ),
            180f
        );

        // =====================================================
        // RIGHT SIDE CANISTERS
        // =====================================================

        CreateCanister(
            group,
            new Vector3(
                b.max.x - 0.75f,
                y,
                b.center.z - 1.1f
            ),
            1.35f
        );

        CreateCanister(
            group,
            new Vector3(
                b.max.x - 1.35f,
                y,
                b.center.z - 1.2f
            ),
            1.0f
        );

        // =====================================================
        // CRATE STACK
        // =====================================================

        CreateCrateStack(
            group,
            new Vector3(
                b.max.x - 1.15f,
                y,
                b.max.z - 1.25f
            )
        );

        // =====================================================
        // SMALL CABINET
        // =====================================================

        CreateElectricalCabinet(
            group,
            new Vector3(
                b.max.x - 0.60f,
                y,
                b.center.z + 1.0f
            ),
            -90f
        );

        // =====================================================
        // FLOOR STRIPS
        // =====================================================

        CreateFloorStrip(
            group,
            new Vector3(
                b.center.x,
                y + 0.015f,
                b.center.z + 1.0f
            ),
            new Vector3(
                3.0f,
                0.025f,
                0.16f
            )
        );

        CreateFloorStrip(
            group,
            new Vector3(
                b.center.x + 1.5f,
                y + 0.015f,
                b.center.z - 1.5f
            ),
            new Vector3(
                1.6f,
                0.025f,
                0.16f
            )
        );

        // =====================================================
        // TECH PANEL
        // =====================================================

        CreateWallPanel(
            group,
            new Vector3(
                b.max.x - 0.18f,
                y + 1.35f,
                b.center.z
            ),
            -90f
        );
    }

    // =========================================================
    // COVER WALL
    // =========================================================

    private static void CreateCoverWall(
        Transform parent,
        Vector3 position,
        float rotation
    )
    {
        Transform root =
            NewProp(
                parent,
                "Stealth_Cover",
                position,
                rotation
            );

        Part(
            root,
            "MainBody",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.70f,
                0f
            ),
            new Vector3(
                1.8f,
                1.4f,
                0.60f
            ),
            metalDark
        );

        Part(
            root,
            "Inset",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.72f,
                -0.32f
            ),
            new Vector3(
                1.40f,
                0.90f,
                0.045f
            ),
            panelDark
        );

        Part(
            root,
            "TopTrim",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.43f,
                0f
            ),
            new Vector3(
                1.70f,
                0.08f,
                0.52f
            ),
            metalLight
        );

        Part(
            root,
            "WarningStrip",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.25f,
                -0.35f
            ),
            new Vector3(
                1.25f,
                0.12f,
                0.02f
            ),
            hazardMat
        );
    }

    // =========================================================
    // MACHINERY BLOCK
    // =========================================================

    private static void CreateMachineryBlock(
        Transform parent,
        Vector3 position,
        float rotation
    )
    {
        Transform root =
            NewProp(
                parent,
                "Utility_Machinery",
                position,
                rotation
            );

        Part(
            root,
            "Base",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.18f,
                0f
            ),
            new Vector3(
                1.8f,
                0.35f,
                1.2f
            ),
            metalDark
        );

        Part(
            root,
            "Body",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.90f,
                0f
            ),
            new Vector3(
                1.55f,
                1.15f,
                1.0f
            ),
            panelDark
        );

        Part(
            root,
            "FrontPanel",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.95f,
                -0.53f
            ),
            new Vector3(
                1.10f,
                0.72f,
                0.05f
            ),
            metalLight
        );

        Part(
            root,
            "Screen",
            PrimitiveType.Cube,
            new Vector3(
                -0.30f,
                1.08f,
                -0.57f
            ),
            new Vector3(
                0.40f,
                0.25f,
                0.015f
            ),
            screenMat
        );

        Part(
            root,
            "Warning",
            PrimitiveType.Cube,
            new Vector3(
                0.32f,
                0.75f,
                -0.57f
            ),
            new Vector3(
                0.42f,
                0.11f,
                0.015f
            ),
            hazardMat
        );

        Part(
            root,
            "TopCylinder",
            PrimitiveType.Cylinder,
            new Vector3(
                0f,
                1.72f,
                0f
            ),
            new Vector3(
                0.20f,
                0.32f,
                0.20f
            ),
            metalLight
        );
    }

    // =========================================================
    // STORAGE RACK
    // =========================================================

    private static void CreateStorageRack(
        Transform parent,
        Vector3 position,
        float rotation
    )
    {
        Transform root =
            NewProp(
                parent,
                "Storage_Rack",
                position,
                rotation
            );

        Part(
            root,
            "Side_Left",
            PrimitiveType.Cube,
            new Vector3(
                -0.70f,
                1.05f,
                0f
            ),
            new Vector3(
                0.10f,
                2.10f,
                0.65f
            ),
            metalDark
        );

        Part(
            root,
            "Side_Right",
            PrimitiveType.Cube,
            new Vector3(
                0.70f,
                1.05f,
                0f
            ),
            new Vector3(
                0.10f,
                2.10f,
                0.65f
            ),
            metalDark
        );

        for (int i = 0; i < 4; i++)
        {
            Part(
                root,
                "Shelf_" + i,
                PrimitiveType.Cube,
                new Vector3(
                    0f,
                    0.25f +
                    i * 0.55f,
                    0f
                ),
                new Vector3(
                    1.5f,
                    0.08f,
                    0.65f
                ),
                metalLight
            );
        }

        Part(
            root,
            "Box_01",
            PrimitiveType.Cube,
            new Vector3(
                -0.35f,
                0.47f,
                0f
            ),
            new Vector3(
                0.50f,
                0.35f,
                0.45f
            ),
            panelDark
        );

        Part(
            root,
            "Box_02",
            PrimitiveType.Cube,
            new Vector3(
                0.35f,
                1.02f,
                0f
            ),
            new Vector3(
                0.50f,
                0.35f,
                0.45f
            ),
            metalDark
        );

        Part(
            root,
            "Box_03",
            PrimitiveType.Cube,
            new Vector3(
                -0.25f,
                1.57f,
                0f
            ),
            new Vector3(
                0.60f,
                0.35f,
                0.45f
            ),
            hazardMat
        );
    }

    // =========================================================
    // WORKBENCH
    // =========================================================

    private static void CreateWorkbench(
        Transform parent,
        Vector3 position,
        float rotation
    )
    {
        Transform root =
            NewProp(
                parent,
                "Storage_Workbench",
                position,
                rotation
            );

        Part(
            root,
            "Top",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.85f,
                0f
            ),
            new Vector3(
                2.2f,
                0.12f,
                0.75f
            ),
            metalDark
        );

        Part(
            root,
            "Leg_L",
            PrimitiveType.Cube,
            new Vector3(
                -0.85f,
                0.42f,
                0f
            ),
            new Vector3(
                0.12f,
                0.80f,
                0.62f
            ),
            metalLight
        );

        Part(
            root,
            "Leg_R",
            PrimitiveType.Cube,
            new Vector3(
                0.85f,
                0.42f,
                0f
            ),
            new Vector3(
                0.12f,
                0.80f,
                0.62f
            ),
            metalLight
        );

        Part(
            root,
            "MonitorHousing",
            PrimitiveType.Cube,
            new Vector3(
                0.50f,
                1.32f,
                -0.15f
            ),
            new Vector3(
                0.65f,
                0.43f,
                0.08f
            ),
            panelDark
        );

        Part(
            root,
            "MonitorScreen",
            PrimitiveType.Cube,
            new Vector3(
                0.50f,
                1.32f,
                -0.20f
            ),
            new Vector3(
                0.52f,
                0.32f,
                0.015f
            ),
            screenMat
        );

        Part(
            root,
            "Toolbox",
            PrimitiveType.Cube,
            new Vector3(
                -0.50f,
                1.02f,
                0.06f
            ),
            new Vector3(
                0.55f,
                0.24f,
                0.34f
            ),
            hazardMat
        );
    }

    // =========================================================
    // CANISTER
    // =========================================================

    private static void CreateCanister(
        Transform parent,
        Vector3 position,
        float height
    )
    {
        Transform root =
            NewProp(
                parent,
                "Utility_Canister",
                position,
                0f
            );

        Part(
            root,
            "Base",
            PrimitiveType.Cylinder,
            new Vector3(
                0f,
                0.08f,
                0f
            ),
            new Vector3(
                0.40f,
                0.08f,
                0.40f
            ),
            metalDark
        );

        Part(
            root,
            "Body",
            PrimitiveType.Cylinder,
            new Vector3(
                0f,
                height * 0.5f,
                0f
            ),
            new Vector3(
                0.33f,
                height * 0.5f,
                0.33f
            ),
            metalLight
        );

        Part(
            root,
            "Top",
            PrimitiveType.Cylinder,
            new Vector3(
                0f,
                height,
                0f
            ),
            new Vector3(
                0.37f,
                0.07f,
                0.37f
            ),
            panelDark
        );

        Part(
            root,
            "Status",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                height * 0.60f,
                -0.35f
            ),
            new Vector3(
                0.18f,
                0.16f,
                0.02f
            ),
            screenMat
        );
    }

    // =========================================================
    // CRATE STACK
    // =========================================================

    private static void CreateCrateStack(
        Transform parent,
        Vector3 position
    )
    {
        Transform root =
            NewProp(
                parent,
                "Crate_Stack",
                position,
                0f
            );

        CreateCratePiece(
            root,
            new Vector3(
                0f,
                0f,
                0f
            )
        );

        CreateCratePiece(
            root,
            new Vector3(
                0.15f,
                0.78f,
                0.05f
            )
        );
    }

    private static void CreateCratePiece(
        Transform parent,
        Vector3 localPosition
    )
    {
        Transform root =
            NewLocalProp(
                parent,
                "Industrial_Crate",
                localPosition
            );

        Part(
            root,
            "Body",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.35f,
                0f
            ),
            new Vector3(
                0.72f,
                0.70f,
                0.72f
            ),
            metalDark
        );

        Part(
            root,
            "Top",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.73f,
                0f
            ),
            new Vector3(
                0.62f,
                0.07f,
                0.62f
            ),
            metalLight
        );

        Part(
            root,
            "Front",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.36f,
                -0.38f
            ),
            new Vector3(
                0.48f,
                0.40f,
                0.025f
            ),
            panelDark
        );

        Part(
            root,
            "Hazard",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.18f,
                -0.40f
            ),
            new Vector3(
                0.42f,
                0.08f,
                0.015f
            ),
            hazardMat
        );
    }

    // =========================================================
    // ELECTRICAL CABINET
    // =========================================================

    private static void CreateElectricalCabinet(
        Transform parent,
        Vector3 position,
        float rotation
    )
    {
        Transform root =
            NewProp(
                parent,
                "Storage_Control_Cabinet",
                position,
                rotation
            );

        Part(
            root,
            "Body",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.95f,
                0f
            ),
            new Vector3(
                0.90f,
                1.90f,
                0.55f
            ),
            metalDark
        );

        Part(
            root,
            "Front",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.95f,
                -0.30f
            ),
            new Vector3(
                0.70f,
                1.55f,
                0.05f
            ),
            panelDark
        );

        Part(
            root,
            "Screen",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.35f,
                -0.34f
            ),
            new Vector3(
                0.38f,
                0.23f,
                0.015f
            ),
            screenMat
        );

        Part(
            root,
            "Warning",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.62f,
                -0.34f
            ),
            new Vector3(
                0.42f,
                0.10f,
                0.015f
            ),
            hazardMat
        );
    }

    // =========================================================
    // WALL PANEL
    // =========================================================

    private static void CreateWallPanel(
        Transform parent,
        Vector3 position,
        float rotation
    )
    {
        Transform root =
            NewProp(
                parent,
                "Tech_Wall_Panel",
                position,
                rotation
            );

        Part(
            root,
            "Backing",
            PrimitiveType.Cube,
            Vector3.zero,
            new Vector3(
                1.25f,
                0.82f,
                0.08f
            ),
            metalDark
        );

        Part(
            root,
            "Inset",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0f,
                -0.055f
            ),
            new Vector3(
                1.02f,
                0.60f,
                0.025f
            ),
            panelDark
        );

        Part(
            root,
            "Screen",
            PrimitiveType.Cube,
            new Vector3(
                -0.25f,
                0.08f,
                -0.08f
            ),
            new Vector3(
                0.38f,
                0.23f,
                0.012f
            ),
            screenMat
        );

        Part(
            root,
            "Hazard",
            PrimitiveType.Cube,
            new Vector3(
                0.30f,
                -0.13f,
                -0.08f
            ),
            new Vector3(
                0.32f,
                0.08f,
                0.012f
            ),
            hazardMat
        );
    }

    // =========================================================
    // FLOOR STRIP
    // =========================================================

    private static void CreateFloorStrip(
        Transform parent,
        Vector3 position,
        Vector3 scale
    )
    {
        GameObject strip =
            GameObject.CreatePrimitive(
                PrimitiveType.Cube
            );

        strip.name =
            "Floor_Hazard_Strip";

        strip.transform.SetParent(
            parent
        );

        strip.transform.position =
            position;

        strip.transform.rotation =
            Quaternion.identity;

        strip.transform.localScale =
            scale;

        Renderer renderer =
            strip.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.sharedMaterial =
                hazardMat;
        }
    }

    // =========================================================
    // BASIC PROP
    // =========================================================

    private static Transform NewGroup(
        Transform parent,
        string name
    )
    {
        GameObject obj =
            new GameObject(name);

        obj.transform.SetParent(
            parent,
            false
        );

        return obj.transform;
    }

    private static Transform NewProp(
        Transform parent,
        string name,
        Vector3 position,
        float rotationY
    )
    {
        GameObject obj =
            new GameObject(name);

        obj.transform.SetParent(
            parent
        );

        obj.transform.position =
            position;

        obj.transform.rotation =
            Quaternion.Euler(
                0f,
                rotationY,
                0f
            );

        return obj.transform;
    }

    private static Transform NewLocalProp(
        Transform parent,
        string name,
        Vector3 localPosition
    )
    {
        GameObject obj =
            new GameObject(name);

        obj.transform.SetParent(
            parent,
            false
        );

        obj.transform.localPosition =
            localPosition;

        obj.transform.localRotation =
            Quaternion.identity;

        return obj.transform;
    }

    private static GameObject Part(
        Transform parent,
        string name,
        PrimitiveType primitive,
        Vector3 localPosition,
        Vector3 localScale,
        Material material
    )
    {
        GameObject obj =
            GameObject.CreatePrimitive(
                primitive
            );

        obj.name =
            name;

        obj.transform.SetParent(
            parent,
            false
        );

        obj.transform.localPosition =
            localPosition;

        obj.transform.localRotation =
            Quaternion.identity;

        obj.transform.localScale =
            localScale;

        Renderer renderer =
            obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.sharedMaterial =
                material;
        }

        return obj;
    }

    // =========================================================
    // GET BOUNDS
    // =========================================================

    private static Bounds GetBounds(
        Transform root
    )
    {
        Renderer[] renderers =
            root.GetComponentsInChildren<Renderer>(
                true
            );

        if (renderers.Length > 0)
        {
            Bounds bounds =
                renderers[0].bounds;

            for (
                int i = 1;
                i < renderers.Length;
                i++
            )
            {
                bounds.Encapsulate(
                    renderers[i].bounds
                );
            }

            return bounds;
        }

        Collider[] colliders =
            root.GetComponentsInChildren<Collider>(
                true
            );

        if (colliders.Length > 0)
        {
            Bounds bounds =
                colliders[0].bounds;

            for (
                int i = 1;
                i < colliders.Length;
                i++
            )
            {
                bounds.Encapsulate(
                    colliders[i].bounds
                );
            }

            return bounds;
        }

        return new Bounds(
            root.position,
            new Vector3(
                10f,
                0.2f,
                10f
            )
        );
    }

    // =========================================================
    // HIERARCHY
    // =========================================================

    private static Transform FindOrCreateDirectChild(
        Transform parent,
        string childName
    )
    {
        Transform child =
            parent.Find(childName);

        if (child != null)
            return child;

        GameObject obj =
            new GameObject(childName);

        obj.transform.SetParent(
            parent,
            false
        );

        return obj.transform;
    }

    private static Transform FindDeepChild(
        Transform parent,
        string targetName
    )
    {
        foreach (Transform child in parent)
        {
            if (
                child.name ==
                targetName
            )
            {
                return child;
            }

            Transform found =
                FindDeepChild(
                    child,
                    targetName
                );

            if (found != null)
                return found;
        }

        return null;
    }

    // =========================================================
    // MATERIALS
    // =========================================================

    private static void EnsureMaterials()
    {
        EnsureFolder(
            "Assets/Materials"
        );

        EnsureFolder(
            "Assets/Materials/Generated"
        );

        metalDark =
            GetOrCreateMaterial(
                "EVA_Sparse_MetalDark",
                new Color(
                    0.14f,
                    0.17f,
                    0.20f
                ),
                0.80f,
                0.30f
            );

        metalLight =
            GetOrCreateMaterial(
                "EVA_Sparse_MetalLight",
                new Color(
                    0.46f,
                    0.51f,
                    0.55f
                ),
                0.68f,
                0.38f
            );

        panelDark =
            GetOrCreateMaterial(
                "EVA_Sparse_PanelDark",
                new Color(
                    0.08f,
                    0.12f,
                    0.16f
                ),
                0.45f,
                0.25f
            );

        hazardMat =
            GetOrCreateMaterial(
                "EVA_Sparse_Hazard",
                new Color(
                    0.95f,
                    0.60f,
                    0.05f
                ),
                0.15f,
                0.28f
            );

        screenMat =
            GetOrCreateEmissionMaterial(
                "EVA_Sparse_Screen",
                new Color(
                    0.04f,
                    0.70f,
                    1.0f
                ),
                2.5f
            );

        AssetDatabase.SaveAssets();
    }

    private static Material GetOrCreateMaterial(
        string name,
        Color color,
        float metallic,
        float smoothness
    )
    {
        string path =
            "Assets/Materials/Generated/" +
            name +
            ".mat";

        Material mat =
            AssetDatabase.LoadAssetAtPath<Material>(
                path
            );

        if (mat == null)
        {
            Shader shader =
                Shader.Find(
                    "Universal Render Pipeline/Lit"
                );

            if (shader == null)
            {
                shader =
                    Shader.Find(
                        "Standard"
                    );
            }

            mat =
                new Material(shader);

            AssetDatabase.CreateAsset(
                mat,
                path
            );
        }

        if (
            mat.HasProperty(
                "_BaseColor"
            )
        )
        {
            mat.SetColor(
                "_BaseColor",
                color
            );
        }
        else
        {
            mat.color =
                color;
        }

        if (
            mat.HasProperty(
                "_Metallic"
            )
        )
        {
            mat.SetFloat(
                "_Metallic",
                metallic
            );
        }

        if (
            mat.HasProperty(
                "_Smoothness"
            )
        )
        {
            mat.SetFloat(
                "_Smoothness",
                smoothness
            );
        }

        EditorUtility.SetDirty(mat);

        return mat;
    }

    private static Material GetOrCreateEmissionMaterial(
        string name,
        Color color,
        float intensity
    )
    {
        Material mat =
            GetOrCreateMaterial(
                name,
                new Color(
                    0.03f,
                    0.05f,
                    0.07f
                ),
                0.12f,
                0.80f
            );

        mat.EnableKeyword(
            "_EMISSION"
        );

        if (
            mat.HasProperty(
                "_EmissionColor"
            )
        )
        {
            mat.SetColor(
                "_EmissionColor",
                color * intensity
            );
        }

        EditorUtility.SetDirty(mat);

        return mat;
    }

    private static void EnsureFolder(
        string path
    )
    {
        string[] parts =
            path.Split('/');

        string current =
            parts[0];

        for (
            int i = 1;
            i < parts.Length;
            i++
        )
        {
            string next =
                current +
                "/" +
                parts[i];

            if (
                !AssetDatabase.IsValidFolder(
                    next
                )
            )
            {
                AssetDatabase.CreateFolder(
                    current,
                    parts[i]
                );
            }

            current =
                next;
        }
    }
}