using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Stage1GameplayPropBuilder
{
    // ============================================================
    // NAMA OBJECT UTAMA
    // ============================================================

    private const string MAP_NAME = "Map_Stage1_Generated";
    private const string MARKER_ROOT_NAME = "03_GAMEPLAY_MARKERS";

    private const string GAMEPLAY_ROOT_NAME = "03_GAMEPLAY_PROPS";
    private const string ENVIRONMENT_ROOT_NAME = "02_ENVIRONMENT_PROPS";

    private const string GENERATED_VISUAL_NAME = "__NEW_VISUAL";

    // ============================================================
    // MATERIAL
    // ============================================================

    private static Material metalDark;
    private static Material metalLight;
    private static Material panelDark;
    private static Material blackMat;
    private static Material screenMat;
    private static Material hazardMat;
    private static Material fabricMat;
    private static Material medicalMat;

    // ============================================================
    // ROOT
    // ============================================================

    private static Transform mapRoot;
    private static Transform markerRoot;
    private static Transform gameplayRoot;
    private static Transform environmentRoot;

    // ============================================================
    // MENU UTAMA
    // ============================================================

    [MenuItem("Tools/EVA/Build Stage 1 Gameplay Props")]
    public static void Build()
    {
        if (!PrepareMap())
            return;

        EnsureMaterials();

        Debug.Log("==============================================");
        Debug.Log("EVA STAGE 1 GAMEPLAY PROP BUILDER");
        Debug.Log("==============================================");

        // ========================================================
        // 1. ACCESS CARD
        // ========================================================

        AccessCardPickup accessCard =
            FindSceneComponent<AccessCardPickup>();

        if (accessCard != null)
        {
            Transform marker =
                FindMarker(
                    new string[]
                    {
                        "access",
                        "card"
                    }
                );

            if (marker != null)
            {
                PrepareExistingGameplayObject(
                    accessCard.gameObject,
                    marker,
                    "AccessCard_Gameplay"
                );

                HideOldVisuals(
                    accessCard.gameObject
                );

                BuildAccessCardVisual(
                    accessCard.transform
                );

                Debug.Log(
                    "✓ Access Card lama dipakai + visual baru dibuat."
                );
            }
            else
            {
                Debug.LogWarning(
                    "Marker Access Card tidak ditemukan."
                );
            }
        }
        else
        {
            Debug.LogWarning(
                "Component AccessCardPickup tidak ditemukan di scene."
            );
        }

        // ========================================================
        // 2. POWER GENERATOR
        // ========================================================

        PowerGeneratorEVA generator =
            FindSceneComponent<PowerGeneratorEVA>();

        if (generator != null)
        {
            Transform marker =
                FindMarker(
                    new string[]
                    {
                        "generator"
                    }
                );

            if (marker != null)
            {
                PrepareExistingGameplayObject(
                    generator.gameObject,
                    marker,
                    "PowerGenerator_Gameplay"
                );

                HideOldVisuals(
                    generator.gameObject
                );

                BuildGeneratorVisual(
                    generator.transform
                );

                Debug.Log(
                    "✓ Power Generator lama dipakai + visual baru dibuat."
                );
            }
            else
            {
                Debug.LogWarning(
                    "Marker Generator tidak ditemukan."
                );
            }
        }
        else
        {
            Debug.LogWarning(
                "Component PowerGeneratorEVA tidak ditemukan di scene."
            );
        }

        // ========================================================
        // 3. TERMINAL
        // ========================================================

        TerminalEVA terminal =
            FindSceneComponent<TerminalEVA>();

        if (terminal != null)
        {
            Transform marker =
                FindMarker(
                    new string[]
                    {
                        "terminal"
                    }
                );

            if (marker != null)
            {
                PrepareExistingGameplayObject(
                    terminal.gameObject,
                    marker,
                    "Terminal_Gameplay"
                );

                HideOldVisuals(
                    terminal.gameObject
                );

                BuildTerminalVisual(
                    terminal.transform
                );

                Debug.Log(
                    "✓ Terminal lama dipakai + visual baru dibuat."
                );
            }
            else
            {
                Debug.LogWarning(
                    "Marker Terminal tidak ditemukan."
                );
            }
        }
        else
        {
            Debug.LogWarning(
                "Component TerminalEVA tidak ditemukan di scene."
            );
        }

        // ========================================================
        // 4. EXIT GATE
        // ========================================================

        EVADoorController door =
            FindSceneComponent<EVADoorController>();

        if (door != null)
        {
            Transform marker =
                FindMarker(
                    new string[]
                    {
                        "exit",
                        "gate"
                    }
                );

            if (marker == null)
            {
                marker =
                    FindMarker(
                        new string[]
                        {
                            "gate"
                        }
                    );
            }

            if (marker != null)
            {
                PrepareExistingGameplayObject(
                    door.gameObject,
                    marker,
                    "ExitGate_Gameplay"
                );

                /*
                 * PENTING:
                 *
                 * Gate lama TIDAK kita hide.
                 *
                 * EVADoorController bisa jadi menggerakkan child tertentu.
                 * Kalau visual lama langsung dimatikan, animasi pintunya
                 * bisa tidak kelihatan.
                 *
                 * Jadi kita tambahkan FRAME BARU mengelilingi gate lama.
                 */

                BuildExitGateUpgrade(
                    door.transform
                );

                Debug.Log(
                    "✓ Exit Gate lama tetap digunakan + frame baru dibuat."
                );
            }
            else
            {
                Debug.LogWarning(
                    "Marker Exit Gate tidak ditemukan."
                );
            }
        }
        else
        {
            Debug.LogWarning(
                "Component EVADoorController tidak ditemukan di scene."
            );
        }

        // ========================================================
        // 5. PROPS LINGKUNGAN
        // ========================================================

        BuildEnvironmentProps();

        // ========================================================
        // SAVE
        // ========================================================

        Physics.SyncTransforms();

        EditorSceneManager.MarkSceneDirty(
            SceneManager.GetActiveScene()
        );

        Selection.activeGameObject =
            gameplayRoot.gameObject;

        Debug.Log("==============================================");
        Debug.Log("BUILD SELESAI.");
        Debug.Log("NPC dan Flashlight TIDAK DIUBAH.");
        Debug.Log("Play Test objective sebelum lanjut robot.");
        Debug.Log("==============================================");
    }

    // ============================================================
    // PREPARE MAP
    // ============================================================

    private static bool PrepareMap()
    {
        GameObject map =
            GameObject.Find(MAP_NAME);

        if (map == null)
        {
            Debug.LogError(
                "Map_Stage1_Generated tidak ditemukan."
            );

            return false;
        }

        mapRoot =
            map.transform;

        markerRoot =
            FindDeepChild(
                mapRoot,
                MARKER_ROOT_NAME
            );

        if (markerRoot == null)
        {
            Debug.LogError(
                "03_GAMEPLAY_MARKERS tidak ditemukan."
            );

            return false;
        }

        // Hapus HANYA root hasil builder sebelumnya.
        Transform oldGameplayRoot =
            FindDirectChild(
                mapRoot,
                GAMEPLAY_ROOT_NAME
            );

        if (oldGameplayRoot != null)
        {
            /*
             * Jangan Destroy root kalau di dalamnya sudah terdapat
             * gameplay object asli.
             *
             * Keluarkan dahulu children yang bukan generated visual.
             */

            List<Transform> children =
                new List<Transform>();

            foreach (Transform child in oldGameplayRoot)
            {
                children.Add(child);
            }

            foreach (Transform child in children)
            {
                child.SetParent(
                    null,
                    true
                );
            }

            UnityEngine.Object.DestroyImmediate(
                oldGameplayRoot.gameObject
            );
        }

        Transform oldEnvironment =
            FindDirectChild(
                mapRoot,
                ENVIRONMENT_ROOT_NAME
            );

        if (oldEnvironment != null)
        {
            UnityEngine.Object.DestroyImmediate(
                oldEnvironment.gameObject
            );
        }

        GameObject gp =
            new GameObject(
                GAMEPLAY_ROOT_NAME
            );

        gp.transform.SetParent(
            mapRoot,
            false
        );

        gameplayRoot =
            gp.transform;

        GameObject env =
            new GameObject(
                ENVIRONMENT_ROOT_NAME
            );

        env.transform.SetParent(
            mapRoot,
            false
        );

        environmentRoot =
            env.transform;

        return true;
    }

    // ============================================================
    // FIND COMPONENT TERMASUK OBJECT INACTIVE
    // ============================================================

    private static T FindSceneComponent<T>()
        where T : Component
    {
        T[] components =
            Resources.FindObjectsOfTypeAll<T>();

        foreach (T component in components)
        {
            if (component == null)
                continue;

            GameObject go =
                component.gameObject;

            if (!go.scene.IsValid())
                continue;

            if (EditorUtility.IsPersistent(go))
                continue;

            return component;
        }

        return null;
    }

    // ============================================================
    // PREPARE GAMEPLAY OBJECT LAMA
    // ============================================================

    private static void PrepareExistingGameplayObject(
        GameObject gameplayObject,
        Transform marker,
        string newName
    )
    {
        if (gameplayObject == null)
            return;

        DeleteGeneratedVisual(
            gameplayObject.transform
        );

        Undo.RecordObject(
            gameplayObject.transform,
            "Move Stage 1 Gameplay Object"
        );

        gameplayObject.name =
            newName;

        /*
         * Object mekanik asli dipindahkan ke gameplay root map baru.
         *
         * Component/collider/script TIDAK dihapus.
         */

        gameplayObject.transform.SetParent(
            gameplayRoot,
            true
        );

        gameplayObject.transform.position =
            marker.position;

        gameplayObject.transform.rotation =
            marker.rotation;

        gameplayObject.SetActive(
            true
        );
    }

    // ============================================================
    // HIDE VISUAL LAMA, MEKANIK TETAP HIDUP
    // ============================================================

    private static void HideOldVisuals(
        GameObject gameplayObject
    )
    {
        Renderer[] renderers =
            gameplayObject.GetComponentsInChildren<Renderer>(
                true
            );

        foreach (Renderer renderer in renderers)
        {
            if (
                renderer.transform.name ==
                GENERATED_VISUAL_NAME
            )
            {
                continue;
            }

            if (
                IsChildOfGeneratedVisual(
                    renderer.transform
                )
            )
            {
                continue;
            }

            renderer.enabled =
                false;
        }

        /*
         * Collider TIDAK dimatikan.
         * Script TIDAK dimatikan.
         * Audio TIDAK dimatikan.
         */
    }

    private static bool IsChildOfGeneratedVisual(
        Transform transform
    )
    {
        Transform t =
            transform;

        while (t != null)
        {
            if (
                t.name ==
                GENERATED_VISUAL_NAME
            )
            {
                return true;
            }

            t =
                t.parent;
        }

        return false;
    }

    // ============================================================
    // ACCESS CARD VISUAL
    // ============================================================

    private static void BuildAccessCardVisual(
        Transform gameplayRootObject
    )
    {
        Transform visual =
            CreateVisualRoot(
                gameplayRootObject
            );

        // pedestal kecil
        Part(
            visual,
            "Pedestal_Base",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.12f,
                0f
            ),
            new Vector3(
                0.75f,
                0.20f,
                0.65f
            ),
            metalDark
        );

        Part(
            visual,
            "Pedestal_Top",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.27f,
                0f
            ),
            new Vector3(
                0.60f,
                0.10f,
                0.50f
            ),
            metalLight
        );

        // card body
        Part(
            visual,
            "AccessCard_Body",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.47f,
                0f
            ),
            new Vector3(
                0.48f,
                0.055f,
                0.30f
            ),
            panelDark
        );

        // cyan strip
        Part(
            visual,
            "AccessCard_LightStrip",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.505f,
                -0.06f
            ),
            new Vector3(
                0.34f,
                0.015f,
                0.055f
            ),
            screenMat
        );

        // chip
        Part(
            visual,
            "AccessCard_Chip",
            PrimitiveType.Cube,
            new Vector3(
                -0.12f,
                0.507f,
                0.07f
            ),
            new Vector3(
                0.10f,
                0.012f,
                0.09f
            ),
            metalLight
        );

        // warning marking
        Part(
            visual,
            "Card_Status",
            PrimitiveType.Cube,
            new Vector3(
                0.13f,
                0.508f,
                0.07f
            ),
            new Vector3(
                0.11f,
                0.012f,
                0.04f
            ),
            hazardMat
        );
    }

    // ============================================================
    // GENERATOR VISUAL
    // ============================================================

    private static void BuildGeneratorVisual(
        Transform gameplayRootObject
    )
    {
        Transform visual =
            CreateVisualRoot(
                gameplayRootObject
            );

        // base
        Part(
            visual,
            "Generator_Base",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.20f,
                0f
            ),
            new Vector3(
                2.8f,
                0.40f,
                1.65f
            ),
            blackMat
        );

        // main body
        Part(
            visual,
            "Generator_Body",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.05f,
                0f
            ),
            new Vector3(
                2.55f,
                1.45f,
                1.40f
            ),
            metalDark
        );

        // front inset
        Part(
            visual,
            "Generator_FrontPanel",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.08f,
                -0.73f
            ),
            new Vector3(
                2.05f,
                1.0f,
                0.08f
            ),
            panelDark
        );

        // vents
        for (int i = 0; i < 6; i++)
        {
            Part(
                visual,
                "Vent_" + i,
                PrimitiveType.Cube,
                new Vector3(
                    -0.75f +
                    i * 0.30f,
                    0.93f,
                    -0.79f
                ),
                new Vector3(
                    0.12f,
                    0.48f,
                    0.025f
                ),
                blackMat
            );
        }

        // display
        Part(
            visual,
            "DisplayHousing",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.48f,
                -0.80f
            ),
            new Vector3(
                0.82f,
                0.30f,
                0.06f
            ),
            blackMat
        );

        Part(
            visual,
            "GeneratorScreen",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.48f,
                -0.84f
            ),
            new Vector3(
                0.64f,
                0.19f,
                0.015f
            ),
            screenMat
        );

        // warning strip
        Part(
            visual,
            "WarningStrip",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.48f,
                -0.80f
            ),
            new Vector3(
                1.75f,
                0.12f,
                0.025f
            ),
            hazardMat
        );

        // side power coils
        CreateGeneratorCoil(
            visual,
            -1.05f
        );

        CreateGeneratorCoil(
            visual,
            1.05f
        );

        // top pipes
        Part(
            visual,
            "TopPipeLeft",
            PrimitiveType.Cylinder,
            new Vector3(
                -0.65f,
                1.90f,
                0.15f
            ),
            new Vector3(
                0.10f,
                0.45f,
                0.10f
            ),
            metalLight
        );

        Part(
            visual,
            "TopPipeRight",
            PrimitiveType.Cylinder,
            new Vector3(
                0.65f,
                1.90f,
                0.15f
            ),
            new Vector3(
                0.10f,
                0.45f,
                0.10f
            ),
            metalLight
        );
    }

    private static void CreateGeneratorCoil(
        Transform parent,
        float x
    )
    {
        Part(
            parent,
            "PowerCoil",
            PrimitiveType.Cylinder,
            new Vector3(
                x,
                1.05f,
                0f
            ),
            new Vector3(
                0.30f,
                0.62f,
                0.30f
            ),
            metalLight
        );

        Part(
            parent,
            "PowerCore",
            PrimitiveType.Cylinder,
            new Vector3(
                x,
                1.05f,
                0f
            ),
            new Vector3(
                0.20f,
                0.66f,
                0.20f
            ),
            screenMat
        );
    }

    // ============================================================
    // TERMINAL VISUAL
    // ============================================================

    private static void BuildTerminalVisual(
        Transform gameplayRootObject
    )
    {
        Transform visual =
            CreateVisualRoot(
                gameplayRootObject
            );

        // floor base
        Part(
            visual,
            "Terminal_Base",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.35f,
                0f
            ),
            new Vector3(
                1.05f,
                0.70f,
                0.75f
            ),
            metalDark
        );

        // lower inset
        Part(
            visual,
            "Terminal_LowerInset",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.42f,
                -0.39f
            ),
            new Vector3(
                0.74f,
                0.42f,
                0.05f
            ),
            panelDark
        );

        // neck
        Part(
            visual,
            "Terminal_Neck",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.92f,
                0.02f
            ),
            new Vector3(
                0.48f,
                0.70f,
                0.42f
            ),
            metalLight
        );

        // monitor
        Part(
            visual,
            "Terminal_MonitorHousing",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.38f,
                -0.10f
            ),
            new Vector3(
                1.02f,
                0.65f,
                0.12f
            ),
            blackMat
        );

        Part(
            visual,
            "Terminal_Screen",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.38f,
                -0.17f
            ),
            new Vector3(
                0.82f,
                0.47f,
                0.015f
            ),
            screenMat
        );

        // control pad
        Part(
            visual,
            "ControlPad",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.93f,
                -0.34f
            ),
            new Vector3(
                0.78f,
                0.08f,
                0.30f
            ),
            panelDark
        );

        Part(
            visual,
            "Button_A",
            PrimitiveType.Cylinder,
            new Vector3(
                -0.23f,
                0.99f,
                -0.38f
            ),
            new Vector3(
                0.045f,
                0.025f,
                0.045f
            ),
            screenMat
        );

        Part(
            visual,
            "Button_B",
            PrimitiveType.Cylinder,
            new Vector3(
                0f,
                0.99f,
                -0.38f
            ),
            new Vector3(
                0.045f,
                0.025f,
                0.045f
            ),
            hazardMat
        );

        Part(
            visual,
            "Button_C",
            PrimitiveType.Cylinder,
            new Vector3(
                0.23f,
                0.99f,
                -0.38f
            ),
            new Vector3(
                0.045f,
                0.025f,
                0.045f
            ),
            metalLight
        );
    }

    // ============================================================
    // EXIT GATE
    // ============================================================

    private static void BuildExitGateUpgrade(
        Transform doorGameplayRoot
    )
    {
        DeleteGeneratedVisual(
            doorGameplayRoot
        );

        Transform visual =
            CreateVisualRoot(
                doorGameplayRoot
            );

        /*
         * Ini FRAME di sekitar gate lama.
         * Tidak mengganti panel pintu yang digerakkan EVADoorController.
         */

        Part(
            visual,
            "Gate_Frame_Left",
            PrimitiveType.Cube,
            new Vector3(
                -2.25f,
                2.0f,
                0f
            ),
            new Vector3(
                0.55f,
                4.0f,
                0.75f
            ),
            metalDark
        );

        Part(
            visual,
            "Gate_Frame_Right",
            PrimitiveType.Cube,
            new Vector3(
                2.25f,
                2.0f,
                0f
            ),
            new Vector3(
                0.55f,
                4.0f,
                0.75f
            ),
            metalDark
        );

        Part(
            visual,
            "Gate_Frame_Top",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                3.75f,
                0f
            ),
            new Vector3(
                5.0f,
                0.50f,
                0.75f
            ),
            metalDark
        );

        // inner metallic trims
        Part(
            visual,
            "Gate_Trim_Left",
            PrimitiveType.Cube,
            new Vector3(
                -1.87f,
                2.0f,
                -0.42f
            ),
            new Vector3(
                0.16f,
                3.3f,
                0.08f
            ),
            metalLight
        );

        Part(
            visual,
            "Gate_Trim_Right",
            PrimitiveType.Cube,
            new Vector3(
                1.87f,
                2.0f,
                -0.42f
            ),
            new Vector3(
                0.16f,
                3.3f,
                0.08f
            ),
            metalLight
        );

        // status panel kiri
        Part(
            visual,
            "Gate_StatusHousing",
            PrimitiveType.Cube,
            new Vector3(
                -2.28f,
                1.45f,
                -0.48f
            ),
            new Vector3(
                0.42f,
                0.80f,
                0.08f
            ),
            blackMat
        );

        Part(
            visual,
            "Gate_StatusScreen",
            PrimitiveType.Cube,
            new Vector3(
                -2.28f,
                1.58f,
                -0.535f
            ),
            new Vector3(
                0.28f,
                0.32f,
                0.015f
            ),
            screenMat
        );

        // hazard top
        Part(
            visual,
            "Gate_HazardStrip",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                3.46f,
                -0.43f
            ),
            new Vector3(
                3.35f,
                0.14f,
                0.04f
            ),
            hazardMat
        );

        // emergency lights
        Part(
            visual,
            "Gate_Light_L",
            PrimitiveType.Cube,
            new Vector3(
                -1.45f,
                3.48f,
                -0.50f
            ),
            new Vector3(
                0.30f,
                0.12f,
                0.05f
            ),
            screenMat
        );

        Part(
            visual,
            "Gate_Light_R",
            PrimitiveType.Cube,
            new Vector3(
                1.45f,
                3.48f,
                -0.50f
            ),
            new Vector3(
                0.30f,
                0.12f,
                0.05f
            ),
            screenMat
        );
    }

    // ============================================================
    // ENVIRONMENT PROPS
    // ============================================================

    private static void BuildEnvironmentProps()
    {
        /*
         * Kita hanya isi props yang relevan dengan Stage 1.
         *
         * Tidak memenuhi seluruh ruangan dengan crate.
         * Center/jalur objective tetap kosong.
         */

        BuildLab01Environment();
        BuildAccessRoomEnvironment();
        BuildGeneratorRoomEnvironment();
        BuildStorageEnvironment();
        BuildExitEnvironment();
        BuildCorridorEnvironment();
    }

    // ============================================================
    // LAB 01
    // ============================================================

    private static void BuildLab01Environment()
    {
        Transform room =
            FindDeepChild(
                mapRoot,
                "LAB_01_Fasilitas_Penahanan_Floor"
            );

        if (room == null)
            return;

        Bounds b =
            GetBounds(room);

        Transform group =
            NewEnvironmentGroup(
                "LAB01_ENV"
            );

        /*
         * Props hanya sekitar perimeter.
         * Tengah dibiarkan kosong untuk Player/NPC/Flashlight.
         */

        BuildBed(
            group,
            new Vector3(
                b.min.x + 1.5f,
                b.max.y + 0.02f,
                b.min.z + 1.1f
            ),
            0f
        );

        BuildBed(
            group,
            new Vector3(
                b.max.x - 1.5f,
                b.max.y + 0.02f,
                b.min.z + 1.1f
            ),
            0f
        );

        BuildLabDesk(
            group,
            new Vector3(
                b.min.x + 1.4f,
                b.max.y + 0.02f,
                b.max.z - 1.0f
            ),
            180f
        );

        BuildMedicalCabinet(
            group,
            new Vector3(
                b.max.x - 0.65f,
                b.max.y + 0.02f,
                b.max.z - 1.2f
            ),
            -90f
        );
    }

    // ============================================================
    // ACCESS CARD ROOM
    // ============================================================

    private static void BuildAccessRoomEnvironment()
    {
        Transform room =
            FindDeepChild(
                mapRoot,
                "Access_Card_Room_Floor"
            );

        if (room == null)
            return;

        Bounds b =
            GetBounds(room);

        Transform group =
            NewEnvironmentGroup(
                "ACCESS_CARD_ROOM_ENV"
            );

        BuildLabDesk(
            group,
            new Vector3(
                b.center.x,
                b.max.y + 0.02f,
                b.max.z - 0.95f
            ),
            180f
        );

        BuildLocker(
            group,
            new Vector3(
                b.min.x + 0.60f,
                b.max.y + 0.02f,
                b.center.z
            ),
            90f
        );

        BuildLocker(
            group,
            new Vector3(
                b.max.x - 0.60f,
                b.max.y + 0.02f,
                b.center.z
            ),
            -90f
        );
    }

    // ============================================================
    // GENERATOR ROOM
    // ============================================================

    private static void BuildGeneratorRoomEnvironment()
    {
        Transform room =
            FindDeepChild(
                mapRoot,
                "Power_Generator_Room_Floor"
            );

        if (room == null)
            return;

        Bounds b =
            GetBounds(room);

        Transform group =
            NewEnvironmentGroup(
                "GENERATOR_ROOM_ENV"
            );

        BuildPowerTank(
            group,
            new Vector3(
                b.min.x + 1.0f,
                b.max.y + 0.02f,
                b.max.z - 1.0f
            )
        );

        BuildPowerTank(
            group,
            new Vector3(
                b.max.x - 1.0f,
                b.max.y + 0.02f,
                b.max.z - 1.0f
            )
        );

        BuildElectricalCabinet(
            group,
            new Vector3(
                b.min.x + 0.65f,
                b.max.y + 0.02f,
                b.center.z
            ),
            90f
        );

        BuildElectricalCabinet(
            group,
            new Vector3(
                b.max.x - 0.65f,
                b.max.y + 0.02f,
                b.center.z
            ),
            -90f
        );
    }

    // ============================================================
    // STORAGE
    // ============================================================

    private static void BuildStorageEnvironment()
    {
        Transform room =
            FindDeepChild(
                mapRoot,
                "Side_Lab_Storage_Floor"
            );

        if (room == null)
            return;

        Bounds b =
            GetBounds(room);

        Transform group =
            NewEnvironmentGroup(
                "STORAGE_ENV"
            );

        BuildStorageRack(
            group,
            new Vector3(
                b.min.x + 0.60f,
                b.max.y + 0.02f,
                b.min.z + 1.6f
            ),
            90f
        );

        BuildStorageRack(
            group,
            new Vector3(
                b.min.x + 0.60f,
                b.max.y + 0.02f,
                b.max.z - 1.6f
            ),
            90f
        );

        BuildCrate(
            group,
            new Vector3(
                b.max.x - 1.0f,
                b.max.y + 0.02f,
                b.max.z - 1.1f
            )
        );
    }

    // ============================================================
    // EXIT
    // ============================================================

    private static void BuildExitEnvironment()
    {
        Transform room =
            FindDeepChild(
                mapRoot,
                "Terminal_Exit_Gate_Area_Floor"
            );

        if (room == null)
            return;

        Bounds b =
            GetBounds(room);

        Transform group =
            NewEnvironmentGroup(
                "EXIT_AREA_ENV"
            );

        BuildElectricalCabinet(
            group,
            new Vector3(
                b.min.x + 0.65f,
                b.max.y + 0.02f,
                b.center.z
            ),
            90f
        );

        BuildElectricalCabinet(
            group,
            new Vector3(
                b.max.x - 0.65f,
                b.max.y + 0.02f,
                b.center.z
            ),
            -90f
        );

        /*
         * Tengah sengaja kosong untuk chase Mini-Boss.
         */
    }

    // ============================================================
    // CORRIDOR
    // ============================================================

    private static void BuildCorridorEnvironment()
    {
        Transform corridor =
            FindDeepChild(
                mapRoot,
                "Floor_MainCorridor"
            );

        if (corridor == null)
            return;

        Bounds b =
            GetBounds(corridor);

        Transform group =
            NewEnvironmentGroup(
                "CORRIDOR_ENV"
            );

        bool alongZ =
            b.size.z >
            b.size.x;

        if (!alongZ)
            return;

        for (
            float z = b.min.z + 4f;
            z < b.max.z - 3f;
            z += 6f
        )
        {
            BuildWallUtilityPanel(
                group,
                new Vector3(
                    b.min.x + 0.18f,
                    b.max.y + 1.25f,
                    z
                ),
                90f
            );

            BuildWallUtilityPanel(
                group,
                new Vector3(
                    b.max.x - 0.18f,
                    b.max.y + 1.25f,
                    z + 2.5f
                ),
                -90f
            );
        }

        // overhead pipes kiri/kanan
        BuildLongPipe(
            group,
            new Vector3(
                b.min.x + 0.45f,
                b.max.y + 3.25f,
                b.center.z
            ),
            b.size.z - 2f
        );

        BuildLongPipe(
            group,
            new Vector3(
                b.max.x - 0.45f,
                b.max.y + 3.25f,
                b.center.z
            ),
            b.size.z - 2f
        );
    }

    // ============================================================
    // PROP: BED
    // ============================================================

    private static void BuildBed(
        Transform parent,
        Vector3 position,
        float rotation
    )
    {
        Transform root =
            NewProp(
                parent,
                "Containment_Bed",
                position,
                rotation
            );

        Part(
            root,
            "Frame",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.20f,
                0f
            ),
            new Vector3(
                2.15f,
                0.22f,
                0.90f
            ),
            metalDark
        );

        Part(
            root,
            "Mattress",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.39f,
                0f
            ),
            new Vector3(
                1.95f,
                0.17f,
                0.76f
            ),
            fabricMat
        );

        Part(
            root,
            "Head",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.72f,
                -0.42f
            ),
            new Vector3(
                2.05f,
                0.78f,
                0.11f
            ),
            panelDark
        );

        Part(
            root,
            "Pillow",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.52f,
                -0.22f
            ),
            new Vector3(
                0.70f,
                0.10f,
                0.28f
            ),
            medicalMat
        );

        Part(
            root,
            "BedStatus",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.90f,
                -0.49f
            ),
            new Vector3(
                0.75f,
                0.06f,
                0.02f
            ),
            screenMat
        );
    }

    // ============================================================
    // PROP: LAB DESK
    // ============================================================

    private static void BuildLabDesk(
        Transform parent,
        Vector3 position,
        float rotation
    )
    {
        Transform root =
            NewProp(
                parent,
                "Laboratory_Workstation",
                position,
                rotation
            );

        Part(
            root,
            "Desk",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.76f,
                0f
            ),
            new Vector3(
                1.70f,
                0.12f,
                0.65f
            ),
            metalDark
        );

        Part(
            root,
            "Monitor",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.25f,
                -0.18f
            ),
            new Vector3(
                0.82f,
                0.50f,
                0.08f
            ),
            blackMat
        );

        Part(
            root,
            "Screen",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.25f,
                -0.23f
            ),
            new Vector3(
                0.66f,
                0.36f,
                0.012f
            ),
            screenMat
        );

        Part(
            root,
            "Keyboard",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.84f,
                0.16f
            ),
            new Vector3(
                0.60f,
                0.035f,
                0.20f
            ),
            blackMat
        );

        Part(
            root,
            "LegLeft",
            PrimitiveType.Cube,
            new Vector3(
                -0.65f,
                0.38f,
                0f
            ),
            new Vector3(
                0.10f,
                0.72f,
                0.55f
            ),
            metalLight
        );

        Part(
            root,
            "LegRight",
            PrimitiveType.Cube,
            new Vector3(
                0.65f,
                0.38f,
                0f
            ),
            new Vector3(
                0.10f,
                0.72f,
                0.55f
            ),
            metalLight
        );
    }

    // ============================================================
    // PROP: MEDICAL CABINET
    // ============================================================

    private static void BuildMedicalCabinet(
        Transform parent,
        Vector3 position,
        float rotation
    )
    {
        Transform root =
            NewProp(
                parent,
                "Medical_Cabinet",
                position,
                rotation
            );

        Part(
            root,
            "Body",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.0f,
                0f
            ),
            new Vector3(
                1.0f,
                2.0f,
                0.55f
            ),
            metalLight
        );

        Part(
            root,
            "Door",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.0f,
                -0.30f
            ),
            new Vector3(
                0.82f,
                1.72f,
                0.05f
            ),
            medicalMat
        );

        // cross
        Part(
            root,
            "MedicalCrossVertical",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.15f,
                -0.34f
            ),
            new Vector3(
                0.12f,
                0.42f,
                0.02f
            ),
            screenMat
        );

        Part(
            root,
            "MedicalCrossHorizontal",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.15f,
                -0.34f
            ),
            new Vector3(
                0.42f,
                0.12f,
                0.02f
            ),
            screenMat
        );
    }

    // ============================================================
    // LOCKER
    // ============================================================

    private static void BuildLocker(
        Transform parent,
        Vector3 position,
        float rotation
    )
    {
        Transform root =
            NewProp(
                parent,
                "Lab_Locker",
                position,
                rotation
            );

        Part(
            root,
            "Body",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.0f,
                0f
            ),
            new Vector3(
                0.90f,
                2.0f,
                0.55f
            ),
            metalDark
        );

        Part(
            root,
            "Door",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.0f,
                -0.30f
            ),
            new Vector3(
                0.74f,
                1.78f,
                0.05f
            ),
            panelDark
        );

        for (int i = 0; i < 4; i++)
        {
            Part(
                root,
                "Vent",
                PrimitiveType.Cube,
                new Vector3(
                    0f,
                    1.65f - i * 0.13f,
                    -0.34f
                ),
                new Vector3(
                    0.43f,
                    0.045f,
                    0.015f
                ),
                blackMat
            );
        }
    }

    // ============================================================
    // POWER TANK
    // ============================================================

    private static void BuildPowerTank(
        Transform parent,
        Vector3 position
    )
    {
        Transform root =
            NewProp(
                parent,
                "Power_Tank",
                position,
                0f
            );

        Part(
            root,
            "Tank",
            PrimitiveType.Cylinder,
            new Vector3(
                0f,
                1.15f,
                0f
            ),
            new Vector3(
                0.52f,
                1.1f,
                0.52f
            ),
            metalLight
        );

        Part(
            root,
            "Base",
            PrimitiveType.Cylinder,
            new Vector3(
                0f,
                0.10f,
                0f
            ),
            new Vector3(
                0.62f,
                0.10f,
                0.62f
            ),
            metalDark
        );

        Part(
            root,
            "RingTop",
            PrimitiveType.Cylinder,
            new Vector3(
                0f,
                2.05f,
                0f
            ),
            new Vector3(
                0.59f,
                0.07f,
                0.59f
            ),
            blackMat
        );

        Part(
            root,
            "TankStatus",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.30f,
                -0.56f
            ),
            new Vector3(
                0.30f,
                0.20f,
                0.035f
            ),
            screenMat
        );
    }

    // ============================================================
    // ELECTRICAL CABINET
    // ============================================================

    private static void BuildElectricalCabinet(
        Transform parent,
        Vector3 position,
        float rotation
    )
    {
        Transform root =
            NewProp(
                parent,
                "Electrical_Cabinet",
                position,
                rotation
            );

        Part(
            root,
            "Body",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.05f,
                0f
            ),
            new Vector3(
                1.0f,
                2.1f,
                0.58f
            ),
            metalDark
        );

        Part(
            root,
            "Front",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.05f,
                -0.32f
            ),
            new Vector3(
                0.78f,
                1.75f,
                0.06f
            ),
            panelDark
        );

        Part(
            root,
            "Screen",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                1.48f,
                -0.36f
            ),
            new Vector3(
                0.40f,
                0.25f,
                0.012f
            ),
            screenMat
        );

        Part(
            root,
            "Warning",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.60f,
                -0.36f
            ),
            new Vector3(
                0.55f,
                0.10f,
                0.012f
            ),
            hazardMat
        );
    }

    // ============================================================
    // STORAGE RACK
    // ============================================================

    private static void BuildStorageRack(
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
            "SideL",
            PrimitiveType.Cube,
            new Vector3(
                -0.55f,
                1.0f,
                0f
            ),
            new Vector3(
                0.09f,
                2.0f,
                0.60f
            ),
            metalDark
        );

        Part(
            root,
            "SideR",
            PrimitiveType.Cube,
            new Vector3(
                0.55f,
                1.0f,
                0f
            ),
            new Vector3(
                0.09f,
                2.0f,
                0.60f
            ),
            metalDark
        );

        for (int i = 0; i < 4; i++)
        {
            Part(
                root,
                "Shelf",
                PrimitiveType.Cube,
                new Vector3(
                    0f,
                    0.25f + i * 0.52f,
                    0f
                ),
                new Vector3(
                    1.15f,
                    0.08f,
                    0.60f
                ),
                metalLight
            );
        }
    }

    // ============================================================
    // CRATE
    // ============================================================

    private static void BuildCrate(
        Transform parent,
        Vector3 position
    )
    {
        Transform root =
            NewProp(
                parent,
                "Industrial_Crate",
                position,
                0f
            );

        Part(
            root,
            "Body",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.43f,
                0f
            ),
            new Vector3(
                0.90f,
                0.86f,
                0.90f
            ),
            metalDark
        );

        Part(
            root,
            "Inset",
            PrimitiveType.Cube,
            new Vector3(
                0f,
                0.43f,
                -0.47f
            ),
            new Vector3(
                0.64f,
                0.55f,
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
                0.20f,
                -0.50f
            ),
            new Vector3(
                0.55f,
                0.10f,
                0.015f
            ),
            hazardMat
        );
    }

    // ============================================================
    // WALL UTILITY PANEL
    // ============================================================

    private static void BuildWallUtilityPanel(
        Transform parent,
        Vector3 position,
        float rotation
    )
    {
        Transform root =
            NewProp(
                parent,
                "Corridor_TechPanel",
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
                0.80f,
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
                0.58f,
                0.025f
            ),
            panelDark
        );

        Part(
            root,
            "Display",
            PrimitiveType.Cube,
            new Vector3(
                -0.25f,
                0.08f,
                -0.075f
            ),
            new Vector3(
                0.37f,
                0.22f,
                0.012f
            ),
            screenMat
        );

        Part(
            root,
            "Warning",
            PrimitiveType.Cube,
            new Vector3(
                0.30f,
                -0.13f,
                -0.075f
            ),
            new Vector3(
                0.32f,
                0.08f,
                0.012f
            ),
            hazardMat
        );
    }

    // ============================================================
    // LONG PIPE
    // ============================================================

    private static void BuildLongPipe(
        Transform parent,
        Vector3 position,
        float length
    )
    {
        GameObject pipe =
            GameObject.CreatePrimitive(
                PrimitiveType.Cylinder
            );

        pipe.name =
            "Ceiling_Pipe";

        pipe.transform.SetParent(
            parent
        );

        pipe.transform.position =
            position;

        pipe.transform.rotation =
            Quaternion.Euler(
                90f,
                0f,
                0f
            );

        pipe.transform.localScale =
            new Vector3(
                0.10f,
                Mathf.Max(
                    0.5f,
                    length / 2f
                ),
                0.10f
            );

        Renderer renderer =
            pipe.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.sharedMaterial =
                metalLight;
        }
    }

    // ============================================================
    // VISUAL ROOT
    // ============================================================

    private static Transform CreateVisualRoot(
        Transform gameplayObject
    )
    {
        DeleteGeneratedVisual(
            gameplayObject
        );

        GameObject obj =
            new GameObject(
                GENERATED_VISUAL_NAME
            );

        obj.transform.SetParent(
            gameplayObject,
            false
        );

        obj.transform.localPosition =
            Vector3.zero;

        obj.transform.localRotation =
            Quaternion.identity;

        obj.transform.localScale =
            Vector3.one;

        return obj.transform;
    }

    private static void DeleteGeneratedVisual(
        Transform gameplayObject
    )
    {
        Transform existing =
            FindDirectChild(
                gameplayObject,
                GENERATED_VISUAL_NAME
            );

        if (existing != null)
        {
            UnityEngine.Object.DestroyImmediate(
                existing.gameObject
            );
        }
    }

    // ============================================================
    // MARKER SEARCH
    // ============================================================

    private static Transform FindMarker(
        string[] requiredWords
    )
    {
        if (markerRoot == null)
            return null;

        Transform[] all =
            markerRoot.GetComponentsInChildren<Transform>(
                true
            );

        foreach (Transform t in all)
        {
            if (t == markerRoot)
                continue;

            string lower =
                t.name.ToLowerInvariant();

            bool match =
                true;

            foreach (string word in requiredWords)
            {
                if (
                    !lower.Contains(
                        word.ToLowerInvariant()
                    )
                )
                {
                    match =
                        false;

                    break;
                }
            }

            if (match)
                return t;
        }

        return null;
    }

    // ============================================================
    // BOUNDS
    // ============================================================

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
            Bounds b =
                renderers[0].bounds;

            for (
                int i = 1;
                i < renderers.Length;
                i++
            )
            {
                b.Encapsulate(
                    renderers[i].bounds
                );
            }

            return b;
        }

        Collider[] colliders =
            root.GetComponentsInChildren<Collider>(
                true
            );

        if (colliders.Length > 0)
        {
            Bounds b =
                colliders[0].bounds;

            for (
                int i = 1;
                i < colliders.Length;
                i++
            )
            {
                b.Encapsulate(
                    colliders[i].bounds
                );
            }

            return b;
        }

        return new Bounds(
            root.position,
            new Vector3(
                8f,
                0.2f,
                8f
            )
        );
    }

    // ============================================================
    // PROP HELPERS
    // ============================================================

    private static Transform NewEnvironmentGroup(
        string name
    )
    {
        GameObject obj =
            new GameObject(
                name
            );

        obj.transform.SetParent(
            environmentRoot,
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
            new GameObject(
                name
            );

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

    // ============================================================
    // HIERARCHY SEARCH
    // ============================================================

    private static Transform FindDeepChild(
        Transform parent,
        string name
    )
    {
        foreach (Transform child in parent)
        {
            if (
                child.name ==
                name
            )
            {
                return child;
            }

            Transform result =
                FindDeepChild(
                    child,
                    name
                );

            if (result != null)
                return result;
        }

        return null;
    }

    private static Transform FindDirectChild(
        Transform parent,
        string name
    )
    {
        foreach (Transform child in parent)
        {
            if (
                child.name ==
                name
            )
            {
                return child;
            }
        }

        return null;
    }

    // ============================================================
    // MATERIALS
    // ============================================================

    private static void EnsureMaterials()
    {
        EnsureFolder(
            "Assets/Materials"
        );

        EnsureFolder(
            "Assets/Materials/Generated"
        );

        metalDark =
            MakeMaterial(
                "EVA_GP_MetalDark",
                new Color(
                    0.16f,
                    0.19f,
                    0.23f
                ),
                0.82f,
                0.32f
            );

        metalLight =
            MakeMaterial(
                "EVA_GP_MetalLight",
                new Color(
                    0.48f,
                    0.53f,
                    0.58f
                ),
                0.68f,
                0.38f
            );

        panelDark =
            MakeMaterial(
                "EVA_GP_Panel",
                new Color(
                    0.10f,
                    0.14f,
                    0.18f
                ),
                0.48f,
                0.24f
            );

        blackMat =
            MakeMaterial(
                "EVA_GP_Black",
                new Color(
                    0.025f,
                    0.03f,
                    0.035f
                ),
                0.38f,
                0.22f
            );

        hazardMat =
            MakeMaterial(
                "EVA_GP_Hazard",
                new Color(
                    0.92f,
                    0.58f,
                    0.06f
                ),
                0.18f,
                0.30f
            );

        fabricMat =
            MakeMaterial(
                "EVA_GP_Fabric",
                new Color(
                    0.29f,
                    0.34f,
                    0.39f
                ),
                0.02f,
                0.16f
            );

        medicalMat =
            MakeMaterial(
                "EVA_GP_Medical",
                new Color(
                    0.72f,
                    0.79f,
                    0.82f
                ),
                0.08f,
                0.34f
            );

        screenMat =
            MakeEmissionMaterial(
                "EVA_GP_Screen",
                new Color(
                    0.04f,
                    0.65f,
                    1.0f
                ),
                2.5f
            );

        AssetDatabase.SaveAssets();
    }

    private static Material MakeMaterial(
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
                new Material(
                    shader
                );

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

        EditorUtility.SetDirty(
            mat
        );

        return mat;
    }

    private static Material MakeEmissionMaterial(
        string name,
        Color color,
        float intensity
    )
    {
        Material mat =
            MakeMaterial(
                name,
                new Color(
                    0.035f,
                    0.055f,
                    0.075f
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
                color *
                intensity
            );
        }

        EditorUtility.SetDirty(
            mat
        );

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