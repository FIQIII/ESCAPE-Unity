using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    [Header("Referensi")]
    public GameObject flashlightInHand;
    public Light flashlightLight;
    public Animator playerAnimator;

    private bool isOn = false;

    private void Start()
    {
        if (flashlightLight != null)
            flashlightLight.enabled = false;
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        // Belum punya senter = tombol F tidak berfungsi
        if (!HasFlashlight())
            return;

        // Tekan F untuk ON / OFF
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            ToggleFlashlight();
        }
    }

    private bool HasFlashlight()
    {
        if (playerAnimator == null)
            return false;

        return playerAnimator.GetBool("HasFlashLight");
    }

    private void ToggleFlashlight()
    {
        if (flashlightInHand == null || flashlightLight == null)
            return;

        if (!flashlightInHand.activeInHierarchy)
            return;

        isOn = !isOn;
        flashlightLight.enabled = isOn;

        Debug.Log(
            isOn
                ? "Senter dinyalakan."
                : "Senter dimatikan."
        );
    }
}