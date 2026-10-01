using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Stage1LayoutGenerator : EditorWindow
{
    // =========================
    // UKURAN DASAR
    // =========================

    private const float FloorThickness = 0.20f;
    private const float WallHeight = 4.0f;
    private const float WallThickness = 0.30f;
    private const float DoorWidth = 3.0f;

    private static Material floorMaterial;
    private static Material wallMaterial;
    private static Material darkWallMaterial;

    [MenuItem("Tools/EVA/Generate Stage 1 Layout")]
    public static void GenerateStage1()
    {
        // Cari map hasil generate sebelumnya
        GameObject existing =
            GameObject.Find("Map_Stage1_Generated");

        if (existing != null)
        {
            bool replace = EditorUtility.DisplayDialog(
                "Stage 1 Generator",
                "Map_Stage1_Generated sudah ada.\n\nHapus dan generate ulang?",
                "Ya, Generate Ulang",
                "Batal"
            );

            if (!replace)
                return;

            DestroyImmediate(existing);
        }

        FindMaterials();

        // =========================
        // ROOT
        // =========================

        GameObject root =
            new GameObject("Map_Stage1_Generated");

        GameObject geometry =
            CreateEmpty("01_GEOMETRY", root.transform);

        GameObject floors =
            CreateEmpty("Floors", geometry.transform);

        GameObject walls =
            CreateEmpty("Walls", geometry.transform);

        GameObject markers =
            CreateEmpty("03_GAMEPLAY_MARKERS", root.transform);

        GameObject lighting =
            CreateEmpty("04_LIGHTING", root.transform);

        GameObject enemies =
            CreateEmpty("05_ENEMIES", root.transform);

        // =========================
        // LAB 01
        // 12 x 10
        // =========================

        CreateRoom(
            floors.transform,
            walls.transform,
            "LAB_01_Fasilitas_Penahanan",
            new Vector3(0f, 0f, 0f),
            new Vector2(12f, 10f),
            false,  // north
            true,   // south closed
            true,   // west closed
            true,   // east closed
            true    // opening north
        );

        // =========================
        // AREA PENYINTAS
        // 10 x 8
        // =========================

        CreateRoomCustom(
            floors.transform,
            walls.transform,
            "Area_NPC_Penyintas",
            new Vector3(0f, 0f, 10f),
            new Vector2(10f, 8f),
            true,   // opening north
            true,   // opening south
            false,  // opening west
            false,  // opening east
            false
        );

        // Connector LAB -> Survivor
        CreateFloor(
            floors.transform,
            "Floor_LAB_to_Survivor",
            new Vector3(0f, 0f, 5.5f),
            new Vector3(3f, FloorThickness, 1f),
            floorMaterial
        );

        // =========================
        // MAIN CORRIDOR
        // 5 meter lebar
        // Z 14 -> 49
        // =========================

        CreateFloor(
            floors.transform,
            "Floor_MainCorridor",
            new Vector3(0f, 0f, 31.5f),
            new Vector3(5f, FloorThickness, 35f),
            floorMaterial
        );

        CreateCorridorWalls(
            walls.transform,
            14f,
            49f,
            5f
        );

        // =========================
        // STEALTH ZONE
        // 14 x 14
        // =========================

        CreateSideRoom(
            floors.transform,
            walls.transform,
            "Stealth_Zone",
            new Vector3(-9.5f, 0f, 24f),
            new Vector2(14f, 14f),
            Side.East,
            true
        );

        // =========================
        // ACCESS CARD ROOM
        // 10 x 10
        // =========================

        CreateSideRoom(
            floors.transform,
            walls.transform,
            "Access_Card_Room",
            new Vector3(7.5f, 0f, 24f),
            new Vector2(10f, 10f),
            Side.West,
            false
        );

        // =========================
        // GENERATOR ROOM
        // 12 x 12
        // =========================

        CreateSideRoom(
            floors.transform,
            walls.transform,
            "Power_Generator_Room",
            new Vector3(-8.5f, 0f, 41f),
            new Vector2(12f, 12f),
            Side.East,
            true
        );

        // =========================
        // STORAGE / SIDE LAB
        // 10 x 10
        // =========================

        CreateSideRoom(
            floors.transform,
            walls.transform,
            "Side_Lab_Storage",
            new Vector3(7.5f, 0f, 41f),
            new Vector2(10f, 10f),
            Side.West,
            false
        );

        // =========================
        // EXIT / TERMINAL AREA
        // 14 x 12
        // =========================

        CreateRoomCustom(
            floors.transform,
            walls.transform,
            "Terminal_Exit_Gate_Area",
            new Vector3(0f, 0f, 55f),
            new Vector2(14f, 12f),
            false,
            true,
            false,
            false,
            true
        );

        // Connector corridor -> exit
        CreateFloor(
            floors.transform,
            "Floor_Corridor_to_Exit",
            new Vector3(0f, 0f, 48.5f),
            new Vector3(5f, FloorThickness, 1f),
            floorMaterial
        );

        // =========================
        // GAMEPLAY MARKERS
        // =========================

        CreateMarker(
            markers.transform,
            "PlayerSpawn",
            new Vector3(0f, 0.2f, -2f)
        );

        CreateMarker(
            markers.transform,
            "NPC_Ilmuwan_Position",
            new Vector3(-3f, 0.2f, 1f)
        );

        CreateMarker(
            markers.transform,
            "Flashlight_Pickup_Position",
            new Vector3(2.5f, 0.2f, 1f)
        );

        CreateMarker(
            markers.transform,
            "AccessCard_Position",
            new Vector3(8f, 0.2f, 24f)
        );

        CreateMarker(
            markers.transform,
            "Generator_Position",
            new Vector3(-9f, 0.2f, 41f)
        );

        CreateMarker(
            markers.transform,
            "Terminal_Position",
            new Vector3(-2f, 0.2f, 57f)
        );

        CreateMarker(
            markers.transform,
            "ExitGate_Position",
            new Vector3(0f, 0.2f, 60f)
        );

        // =========================
        // ROBOT MARKERS
        // =========================

        CreateMarker(
            enemies.transform,
            "Robot_Patrol_01_Position",
            new Vector3(-10f, 0.2f, 24f)
        );

        CreateMarker(
            enemies.transform,
            "Robot_Patrol_02_Position",
            new Vector3(8f, 0.2f, 29f)
        );

        CreateMarker(
            enemies.transform,
            "Robot_Patrol_03_Position",
            new Vector3(-7f, 0.2f, 37f)
        );

        CreateMarker(
            enemies.transform,
            "MiniBoss_Position",
            new Vector3(0f, 0.2f, 54f)
        );

        // Pilih map hasil generate
        Selection.activeGameObject = root;

        SceneView.lastActiveSceneView?.FrameSelected();

        Debug.Log(
            "Stage 1 EVA berhasil dibuat: Map_Stage1_Generated"
        );
    }

    // ==========================================================
    // ROOM CREATION
    // ==========================================================

    private static void CreateRoom(
        Transform floorParent,
        Transform wallParent,
        string name,
        Vector3 center,
        Vector2 size,
        bool openNorth,
        bool southClosed,
        bool westClosed,
        bool eastClosed,
        bool northOpening
    )
    {
        CreateRoomCustom(
            floorParent,
            wallParent,
            name,
            center,
            size,
            northOpening,
            false,
            false,
            false,
            false
        );
    }

    private static void CreateRoomCustom(
        Transform floorParent,
        Transform wallParent,
        string name,
        Vector3 center,
        Vector2 size,
        bool openNorth,
        bool openSouth,
        bool openWest,
        bool openEast,
        bool useLabWall
    )
    {
        GameObject roomFloors =
            CreateEmpty(name + "_Floor", floorParent);

        GameObject roomWalls =
            CreateEmpty(name + "_Walls", wallParent);

        CreateFloor(
            roomFloors.transform,
            "Floor",
            center,
            new Vector3(
                size.x,
                FloorThickness,
                size.y
            ),
            floorMaterial
        );

        float halfX = size.x / 2f;
        float halfZ = size.y / 2f;

        Material selectedWall =
            useLabWall ? wallMaterial : wallMaterial;

        // NORTH
        if (openNorth)
        {
            CreateHorizontalWallWithDoor(
                roomWalls.transform,
                center.z + halfZ,
                center.x,
                size.x,
                selectedWall
            );
        }
        else
        {
            CreateWall(
                roomWalls.transform,
                "Wall_North",
                new Vector3(
                    center.x,
                    WallHeight / 2f,
                    center.z + halfZ
                ),
                new Vector3(
                    size.x,
                    WallHeight,
                    WallThickness
                ),
                selectedWall
            );
        }

        // SOUTH
        if (openSouth)
        {
            CreateHorizontalWallWithDoor(
                roomWalls.transform,
                center.z - halfZ,
                center.x,
                size.x,
                selectedWall
            );
        }
        else
        {
            CreateWall(
                roomWalls.transform,
                "Wall_South",
                new Vector3(
                    center.x,
                    WallHeight / 2f,
                    center.z - halfZ
                ),
                new Vector3(
                    size.x,
                    WallHeight,
                    WallThickness
                ),
                selectedWall
            );
        }

        // WEST
        if (openWest)
        {
            CreateVerticalWallWithDoor(
                roomWalls.transform,
                center.x - halfX,
                center.z,
                size.y,
                selectedWall
            );
        }
        else
        {
            CreateWall(
                roomWalls.transform,
                "Wall_West",
                new Vector3(
                    center.x - halfX,
                    WallHeight / 2f,
                    center.z
                ),
                new Vector3(
                    WallThickness,
                    WallHeight,
                    size.y
                ),
                selectedWall
            );
        }

        // EAST
        if (openEast)
        {
            CreateVerticalWallWithDoor(
                roomWalls.transform,
                center.x + halfX,
                center.z,
                size.y,
                selectedWall
            );
        }
        else
        {
            CreateWall(
                roomWalls.transform,
                "Wall_East",
                new Vector3(
                    center.x + halfX,
                    WallHeight / 2f,
                    center.z
                ),
                new Vector3(
                    WallThickness,
                    WallHeight,
                    size.y
                ),
                selectedWall
            );
        }
    }

    private enum Side
    {
        North,
        South,
        West,
        East
    }

    private static void CreateSideRoom(
        Transform floorParent,
        Transform wallParent,
        string name,
        Vector3 center,
        Vector2 size,
        Side openingSide,
        bool dark
    )
    {
        GameObject roomFloors =
            CreateEmpty(name + "_Floor", floorParent);

        GameObject roomWalls =
            CreateEmpty(name + "_Walls", wallParent);

        CreateFloor(
            roomFloors.transform,
            "Floor",
            center,
            new Vector3(
                size.x,
                FloorThickness,
                size.y
            ),
            floorMaterial
        );

        float halfX = size.x / 2f;
        float halfZ = size.y / 2f;

        Material mat =
            dark ? darkWallMaterial : wallMaterial;

        // NORTH
        if (openingSide == Side.North)
            CreateHorizontalWallWithDoor(
                roomWalls.transform,
                center.z + halfZ,
                center.x,
                size.x,
                mat
            );
        else
            CreateWall(
                roomWalls.transform,
                "Wall_North",
                new Vector3(
                    center.x,
                    WallHeight / 2f,
                    center.z + halfZ
                ),
                new Vector3(
                    size.x,
                    WallHeight,
                    WallThickness
                ),
                mat
            );

        // SOUTH
        if (openingSide == Side.South)
            CreateHorizontalWallWithDoor(
                roomWalls.transform,
                center.z - halfZ,
                center.x,
                size.x,
                mat
            );
        else
            CreateWall(
                roomWalls.transform,
                "Wall_South",
                new Vector3(
                    center.x,
                    WallHeight / 2f,
                    center.z - halfZ
                ),
                new Vector3(
                    size.x,
                    WallHeight,
                    WallThickness
                ),
                mat
            );

        // WEST
        if (openingSide == Side.West)
            CreateVerticalWallWithDoor(
                roomWalls.transform,
                center.x - halfX,
                center.z,
                size.y,
                mat
            );
        else
            CreateWall(
                roomWalls.transform,
                "Wall_West",
                new Vector3(
                    center.x - halfX,
                    WallHeight / 2f,
                    center.z
                ),
                new Vector3(
                    WallThickness,
                    WallHeight,
                    size.y
                ),
                mat
            );

        // EAST
        if (openingSide == Side.East)
            CreateVerticalWallWithDoor(
                roomWalls.transform,
                center.x + halfX,
                center.z,
                size.y,
                mat
            );
        else
            CreateWall(
                roomWalls.transform,
                "Wall_East",
                new Vector3(
                    center.x + halfX,
                    WallHeight / 2f,
                    center.z
                ),
                new Vector3(
                    WallThickness,
                    WallHeight,
                    size.y
                ),
                mat
            );
    }

    // ==========================================================
    // CORRIDOR
    // ==========================================================

    private static void CreateCorridorWalls(
        Transform parent,
        float startZ,
        float endZ,
        float corridorWidth
    )
    {
        float leftX = -corridorWidth / 2f;
        float rightX = corridorWidth / 2f;

        float[] doorCenters =
        {
            24f,
            41f
        };

        CreateSegmentedVerticalWall(
            parent,
            "Corridor_West",
            leftX,
            startZ,
            endZ,
            doorCenters,
            darkWallMaterial
        );

        CreateSegmentedVerticalWall(
            parent,
            "Corridor_East",
            rightX,
            startZ,
            endZ,
            doorCenters,
            wallMaterial
        );
    }

    private static void CreateSegmentedVerticalWall(
        Transform parent,
        string baseName,
        float x,
        float startZ,
        float endZ,
        float[] gaps,
        Material material
    )
    {
        List<float> points = new List<float>();

        points.Add(startZ);

        foreach (float gap in gaps)
        {
            points.Add(gap - DoorWidth / 2f);
            points.Add(gap + DoorWidth / 2f);
        }

        points.Add(endZ);

        int segmentIndex = 1;

        for (int i = 0; i < points.Count - 1; i += 2)
        {
            float segmentStart = points[i];
            float segmentEnd = points[i + 1];

            float length =
                segmentEnd - segmentStart;

            if (length <= 0f)
                continue;

            float centerZ =
                (segmentStart + segmentEnd) / 2f;

            CreateWall(
                parent,
                baseName + "_Segment_" + segmentIndex,
                new Vector3(
                    x,
                    WallHeight / 2f,
                    centerZ
                ),
                new Vector3(
                    WallThickness,
                    WallHeight,
                    length
                ),
                material
            );

            segmentIndex++;
        }
    }

    // ==========================================================
    // WALL WITH DOOR
    // ==========================================================

    private static void CreateHorizontalWallWithDoor(
        Transform parent,
        float z,
        float centerX,
        float totalWidth,
        Material material
    )
    {
        float sideWidth =
            (totalWidth - DoorWidth) / 2f;

        if (sideWidth <= 0f)
            return;

        float offset =
            DoorWidth / 2f + sideWidth / 2f;

        CreateWall(
            parent,
            "Wall_Left",
            new Vector3(
                centerX - offset,
                WallHeight / 2f,
                z
            ),
            new Vector3(
                sideWidth,
                WallHeight,
                WallThickness
            ),
            material
        );

        CreateWall(
            parent,
            "Wall_Right",
            new Vector3(
                centerX + offset,
                WallHeight / 2f,
                z
            ),
            new Vector3(
                sideWidth,
                WallHeight,
                WallThickness
            ),
            material
        );
    }

    private static void CreateVerticalWallWithDoor(
        Transform parent,
        float x,
        float centerZ,
        float totalLength,
        Material material
    )
    {
        float sideLength =
            (totalLength - DoorWidth) / 2f;

        if (sideLength <= 0f)
            return;

        float offset =
            DoorWidth / 2f + sideLength / 2f;

        CreateWall(
            parent,
            "Wall_Bottom",
            new Vector3(
                x,
                WallHeight / 2f,
                centerZ - offset
            ),
            new Vector3(
                WallThickness,
                WallHeight,
                sideLength
            ),
            material
        );

        CreateWall(
            parent,
            "Wall_Top",
            new Vector3(
                x,
                WallHeight / 2f,
                centerZ + offset
            ),
            new Vector3(
                WallThickness,
                WallHeight,
                sideLength
            ),
            material
        );
    }

    // ==========================================================
    // BASIC OBJECTS
    // ==========================================================

    private static GameObject CreateFloor(
        Transform parent,
        string name,
        Vector3 position,
        Vector3 scale,
        Material material
    )
    {
        GameObject obj =
            GameObject.CreatePrimitive(
                PrimitiveType.Cube
            );

        obj.name = name;
        obj.transform.SetParent(parent);
        obj.transform.position = position;
        obj.transform.localScale = scale;

        ApplyMaterial(obj, material);

        return obj;
    }

    private static GameObject CreateWall(
        Transform parent,
        string name,
        Vector3 position,
        Vector3 scale,
        Material material
    )
    {
        GameObject obj =
            GameObject.CreatePrimitive(
                PrimitiveType.Cube
            );

        obj.name = name;
        obj.transform.SetParent(parent);
        obj.transform.position = position;
        obj.transform.localScale = scale;

        ApplyMaterial(obj, material);

        return obj;
    }

    private static GameObject CreateEmpty(
        string name,
        Transform parent
    )
    {
        GameObject obj =
            new GameObject(name);

        obj.transform.SetParent(parent);
        obj.transform.localPosition =
            Vector3.zero;

        return obj;
    }

    private static void CreateMarker(
        Transform parent,
        string name,
        Vector3 position
    )
    {
        GameObject marker =
            new GameObject(name);

        marker.transform.SetParent(parent);
        marker.transform.position = position;
    }

    // ==========================================================
    // MATERIAL
    // ==========================================================

    private static void FindMaterials()
    {
        floorMaterial =
            FindMaterial(
                "Mat_Floor_Industrial",
                "MAT_Floor_Industrial"
            );

        wallMaterial =
            FindMaterial(
                "MAT_Wall_Lab",
                "Mat_Wall_Lab"
            );

        darkWallMaterial =
            FindMaterial(
                "MAT_Wall_Dark",
                "Mat_Wall_Dark"
            );

        if (wallMaterial == null)
            wallMaterial = GetDefaultMaterial();

        if (darkWallMaterial == null)
            darkWallMaterial = wallMaterial;

        if (floorMaterial == null)
            floorMaterial = wallMaterial;
    }

    private static Material FindMaterial(
        params string[] names
    )
    {
        foreach (string materialName in names)
        {
            string[] guids =
                AssetDatabase.FindAssets(
                    materialName + " t:Material"
                );

            foreach (string guid in guids)
            {
                string path =
                    AssetDatabase.GUIDToAssetPath(guid);

                Material mat =
                    AssetDatabase.LoadAssetAtPath<Material>(
                        path
                    );

                if (mat != null &&
                    mat.name.Equals(
                        materialName,
                        System.StringComparison.OrdinalIgnoreCase
                    ))
                {
                    return mat;
                }
            }
        }

        return null;
    }

    private static Material GetDefaultMaterial()
    {
        Shader shader =
            Shader.Find(
                "Universal Render Pipeline/Lit"
            );

        if (shader == null)
            shader = Shader.Find("Standard");

        return new Material(shader);
    }

    private static void ApplyMaterial(
        GameObject obj,
        Material material
    )
    {
        Renderer renderer =
            obj.GetComponent<Renderer>();

        if (renderer != null &&
            material != null)
        {
            renderer.sharedMaterial = material;
        }
    }
}