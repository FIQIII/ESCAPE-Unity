using UnityEngine;

public class TerminalEVA : MonoBehaviour
{
    [Header("Referensi Stage 1")]
    public PowerGeneratorEVA generator;
    public EVADoorController exitGate;

    [Header("Mini Boss Stage 1")]
    [SerializeField] private GameObject miniBossStage1;

    [Header("Visual")]
    public Renderer screenRenderer;
    public Light terminalLight;

    [Header("Warna")]
    public Color lockedColor = new Color(1f, 0.12f, 0.05f, 1f);
    public Color readyColor = new Color(0.05f, 0.75f, 1f, 1f);
    public Color completeColor = new Color(0.15f, 1f, 0.35f, 1f);

    [SerializeField] private bool completed = false;

    public bool IsCompleted => completed;

    private void Start()
    {
        UpdateVisual(null);

        // Mini-Boss tidak aktif saat level dimulai.
        if (miniBossStage1 != null)
        {
            miniBossStage1.SetActive(false);
        }
    }

    public string GetPrompt(PlayerInventoryEVA inventory)
    {
        if (completed)
            return "TERMINAL AKTIF";

        if (inventory == null || !inventory.HasStage1AccessCard)
            return "BUTUH ACCESS CARD";

        if (generator == null || !generator.IsActivated)
            return "GENERATOR BELUM AKTIF";

        return "[E]  AKTIFKAN TERMINAL";
    }

    public void Interact(PlayerInventoryEVA inventory)
    {
        if (completed)
        {
            Debug.Log("TERMINAL STAGE 1 SUDAH AKTIF.");
            return;
        }

        if (inventory == null || !inventory.HasStage1AccessCard)
        {
            Debug.Log("TERMINAL: ACCESS CARD DIPERLUKAN.");
            UpdateVisual(inventory);
            return;
        }

        if (generator == null || !generator.IsActivated)
        {
            Debug.Log("TERMINAL: POWER GENERATOR BELUM AKTIF.");
            UpdateVisual(inventory);
            return;
        }

        // ============================================
        // TERMINAL BERHASIL DIRETAS / DIAKTIFKAN
        // ============================================

        completed = true;

        // ============================================
        // BUKA GERBANG KELUAR
        // ============================================

        if (exitGate != null)
        {
            exitGate.SetLocked(false);
            exitGate.OpenDoor();
        }
        else
        {
            Debug.LogWarning(
                "Exit Gate belum dipasang pada TerminalEVA."
            );
        }

        // ============================================
        // AKTIFKAN MINI-BOSS
        // ============================================

        if (miniBossStage1 != null)
        {
            miniBossStage1.SetActive(true);

            Debug.Log(
                "MINI-BOSS STAGE 1 AKTIF DAN MULAI MENGEJAR PLAYER."
            );
        }
        else
        {
            Debug.LogWarning(
                "MiniBoss Stage 1 belum dipasang pada TerminalEVA."
            );
        }

        UpdateVisual(inventory);

        Debug.Log(
            "TERMINAL STAGE 1 BERHASIL DIRETAS."
        );

        Debug.Log(
            "EXIT GATE STAGE 1 TELAH TERBUKA."
        );
    }

    private void UpdateVisual(PlayerInventoryEVA inventory)
    {
        Color target;

        if (completed)
        {
            target = completeColor;
        }
        else if (
            inventory != null &&
            inventory.HasStage1AccessCard &&
            generator != null &&
            generator.IsActivated
        )
        {
            target = readyColor;
        }
        else
        {
            target = lockedColor;
        }

        if (screenRenderer != null)
        {
            Material mat = screenRenderer.material;

            if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", target);
            }
            else if (mat.HasProperty("_Color"))
            {
                mat.SetColor("_Color", target);
            }

            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor(
                    "_EmissionColor",
                    target * 3f
                );
            }
        }

        if (terminalLight != null)
        {
            terminalLight.color = target;
            terminalLight.intensity =
                completed ? 2.0f : 1.2f;
        }
    }
}