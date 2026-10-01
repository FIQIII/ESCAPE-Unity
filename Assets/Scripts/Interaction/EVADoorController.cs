using UnityEngine;

public class EVADoorController : MonoBehaviour
{
    [Header("Panel Pintu")]
    public Transform doorLeft;
    public Transform doorRight;

    [Header("Gerakan")]
    public float openDistance = 1.25f;
    public float openSpeed = 3.5f;
    public bool autoClose = false;
    public float autoCloseDelay = 3f;

    [Header("Status")]
    public bool locked = false;

    [Header("Indikator")]
    public Renderer indicatorRenderer;
    public Color unlockedColor = new Color(0.1f, 0.9f, 1f, 1f);
    public Color lockedColor = new Color(1f, 0.08f, 0.05f, 1f);
    public Color openColor = new Color(0.15f, 1f, 0.35f, 1f);

    private Vector3 leftClosed;
    private Vector3 rightClosed;
    private Vector3 leftOpen;
    private Vector3 rightOpen;

    private bool isOpen;
    private float closeTimer;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        if (doorLeft != null)
            leftClosed = doorLeft.localPosition;

        if (doorRight != null)
            rightClosed = doorRight.localPosition;

        leftOpen = leftClosed + Vector3.left * openDistance;
        rightOpen = rightClosed + Vector3.right * openDistance;

        UpdateIndicator();
    }

    private void Update()
    {
        if (doorLeft != null)
        {
            doorLeft.localPosition = Vector3.MoveTowards(
                doorLeft.localPosition,
                isOpen ? leftOpen : leftClosed,
                openSpeed * Time.deltaTime
            );
        }

        if (doorRight != null)
        {
            doorRight.localPosition = Vector3.MoveTowards(
                doorRight.localPosition,
                isOpen ? rightOpen : rightClosed,
                openSpeed * Time.deltaTime
            );
        }

        if (autoClose && isOpen)
        {
            closeTimer -= Time.deltaTime;
            if (closeTimer <= 0f)
                CloseDoor();
        }
    }

    public void Interact()
    {
        if (locked)
        {
            Debug.Log("Pintu EVA terkunci.");
            UpdateIndicator();
            return;
        }

        if (isOpen)
            CloseDoor();
        else
            OpenDoor();
    }

    public void OpenDoor()
    {
        if (locked)
            return;

        isOpen = true;
        closeTimer = autoCloseDelay;
        UpdateIndicator();
    }

    public void CloseDoor()
    {
        isOpen = false;
        UpdateIndicator();
    }

    public void SetLocked(bool value)
    {
        locked = value;

        if (locked)
            isOpen = false;

        UpdateIndicator();
    }

    private void UpdateIndicator()
    {
        if (indicatorRenderer == null)
            return;

        Color c = locked ? lockedColor : (isOpen ? openColor : unlockedColor);

        Material mat = indicatorRenderer.material;

        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", c);
        else if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", c);

        if (mat.HasProperty("_EmissionColor"))
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", c * 3f);
        }
    }
}
