using UnityEngine;

public class FlashlightPickup : MonoBehaviour
{
    [Header("Referensi Player")]
    public GameObject flashlightInHand;
    public Animator playerAnimator;

    [Header("Animator")]
    public string hasFlashlightParameter = "HasFlashlight";

    private bool pickedUp = false;

    public void Interact()
    {
        if (pickedUp)
            return;

        pickedUp = true;

        // Munculkan senter yang sudah terpasang di RightHand
        if (flashlightInHand != null)
        {
            flashlightInHand.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "Flashlight In Hand belum diisi pada FlashlightPickup."
            );
        }

        // Ubah Animator ke mode membawa senter
        if (playerAnimator != null)
        {
            playerAnimator.SetBool(
                hasFlashlightParameter,
                true
            );
        }
        else
        {
            Debug.LogWarning(
                "Player Animator belum diisi pada FlashlightPickup."
            );
        }

        Debug.Log("Player mengambil senter.");

        // Hilangkan senter dari lantai
        gameObject.SetActive(false);
    }
}