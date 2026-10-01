using UnityEngine;

public class PowerGeneratorEVA : MonoBehaviour
{
    [Header("Status Generator")]
    [SerializeField] private bool activated = false;

    [Header("Referensi Player")]
    [SerializeField] private PlayerInventoryEVA playerInventory;

    [Header("Visual")]
    public Renderer statusIndicator;
    public Light statusLight;
    public Transform turbine;

    [Header("Warna")]
    public Color offColor = new Color(1f, 0.08f, 0.05f, 1f);
    public Color onColor = new Color(0.15f, 1f, 0.35f, 1f);

    [Header("Animasi")]
    public float turbineSpeed = 180f;

    public bool IsActivated => activated;

    private void Start()
    {
        ApplyVisualState();

        if (playerInventory == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                playerInventory =
                    playerObject.GetComponent<PlayerInventoryEVA>();

                if (playerInventory == null)
                {
                    playerInventory =
                        playerObject.GetComponentInChildren<PlayerInventoryEVA>();
                }
            }
        }
    }

    private void Update()
    {
        if (activated && turbine != null)
        {
            turbine.Rotate(
                Vector3.forward,
                turbineSpeed * Time.deltaTime,
                Space.Self
            );
        }
    }

    public void Interact()
    {
        if (activated)
        {
            Debug.Log("POWER GENERATOR SUDAH AKTIF.");
            return;
        }

        if (playerInventory == null)
        {
            Debug.LogWarning(
                "PlayerInventoryEVA belum terhubung ke Generator."
            );
            return;
        }

        if (!playerInventory.HasStage1AccessCard)
        {
            Debug.Log(
                "ACCESS CARD STAGE 1 DIPERLUKAN UNTUK MENGAKTIFKAN GENERATOR."
            );
            return;
        }

        ActivateGenerator();
    }

    public void ActivateGenerator()
    {
        if (activated)
            return;

        activated = true;

        ApplyVisualState();

        Debug.Log(
            "POWER GENERATOR STAGE 1 BERHASIL DIAKTIFKAN."
        );
    }

    private void ApplyVisualState()
    {
        Color target = activated ? onColor : offColor;

        if (statusIndicator != null)
        {
            Material mat = statusIndicator.material;

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

        if (statusLight != null)
        {
            statusLight.color = target;
            statusLight.intensity =
                activated ? 2.2f : 1.1f;
        }
    }
}