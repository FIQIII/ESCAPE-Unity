using UnityEngine;

public class AccessCardPickup : MonoBehaviour
{
    [Header("Access Card")]
    public string cardName = "Access Card - Sector Alpha";
    public bool destroyAfterPickup = true;

    private bool pickedUp = false;

    public void Interact(PlayerInventoryEVA inventory)
    {
        if (pickedUp || inventory == null)
            return;

        inventory.GiveStage1AccessCard();
        pickedUp = true;

        Debug.Log(cardName + " berhasil diambil.");

        if (destroyAfterPickup)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}
