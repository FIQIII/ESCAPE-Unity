using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public static class EVAInteractionUIBuilder
{
    [MenuItem("EVA/Create Interaction UI")]
    public static void CreateInteractionUI()
    {
        GameObject oldUI = GameObject.Find("EVA_UI");

        if (oldUI != null)
        {
            bool rebuild = EditorUtility.DisplayDialog(
                "EVA Interaction UI",
                "EVA_UI sudah ada. Hapus lalu buat ulang?",
                "Ya, Buat Ulang",
                "Batal"
            );

            if (!rebuild) return;

            Object.DestroyImmediate(oldUI);
        }

        GameObject canvasObject = new GameObject(
            "EVA_UI",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode =
            CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        GameObject prompt = new GameObject(
            "InteractionPrompt",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );

        prompt.transform.SetParent(canvasObject.transform, false);

        RectTransform promptRect =
            prompt.GetComponent<RectTransform>();

        promptRect.anchorMin = new Vector2(0.5f, 0f);
        promptRect.anchorMax = new Vector2(0.5f, 0f);
        promptRect.pivot = new Vector2(0.5f, 0.5f);
        promptRect.anchoredPosition = new Vector2(0f, 125f);
        promptRect.sizeDelta = new Vector2(430f, 64f);

        Image background = prompt.GetComponent<Image>();
        background.color =
            new Color(0.015f, 0.025f, 0.035f, 0.82f);
        background.raycastTarget = false;

        GameObject accent = new GameObject(
            "Accent",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );

        accent.transform.SetParent(prompt.transform, false);

        RectTransform accentRect =
            accent.GetComponent<RectTransform>();

        accentRect.anchorMin = new Vector2(0f, 0f);
        accentRect.anchorMax = new Vector2(0f, 1f);
        accentRect.pivot = new Vector2(0f, 0.5f);
        accentRect.sizeDelta = new Vector2(6f, 0f);

        Image accentImage = accent.GetComponent<Image>();
        accentImage.color =
            new Color(0f, 0.85f, 1f, 1f);
        accentImage.raycastTarget = false;

        GameObject textObject = new GameObject(
            "PromptText",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Text)
        );

        textObject.transform.SetParent(prompt.transform, false);

        RectTransform textRect =
            textObject.GetComponent<RectTransform>();

        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(22f, 4f);
        textRect.offsetMax = new Vector2(-14f, -4f);

        Text text = textObject.GetComponent<Text>();
        text.text = "[E]  INTERACT";
        text.font =
            Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 25;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color =
            new Color(0.80f, 0.96f, 1f, 1f);
        text.raycastTarget = false;

        prompt.SetActive(false);

        GameObject player = GameObject.Find("Player");

        if (player == null)
        {
            Debug.LogWarning(
                "EVA_UI dibuat, tetapi GameObject Player tidak ditemukan."
            );
            Selection.activeGameObject = canvasObject;
            return;
        }

        PlayerInteract playerInteract =
            player.GetComponent<PlayerInteract>();

        if (playerInteract == null)
        {
            Debug.LogWarning(
                "EVA_UI dibuat, tetapi Player belum memiliki PlayerInteract."
            );
            Selection.activeGameObject = canvasObject;
            return;
        }

        playerInteract.interactionPrompt = prompt;
        playerInteract.interactionText = text;

        EditorUtility.SetDirty(playerInteract);

        Selection.activeGameObject = canvasObject;
        EditorGUIUtility.PingObject(canvasObject);

        Debug.Log(
            "Interaction UI berhasil dibuat dan dihubungkan ke Player."
        );
    }
}