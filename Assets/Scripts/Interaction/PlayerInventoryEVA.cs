using UnityEngine;

public class PlayerInventoryEVA : MonoBehaviour
{
    [Header("Item Misi Stage 1")]
    [SerializeField] private bool hasStage1AccessCard = false;

    public bool HasStage1AccessCard => hasStage1AccessCard;

    public void GiveStage1AccessCard()
    {
        if (hasStage1AccessCard)
            return;

        hasStage1AccessCard = true;
        Debug.Log("ACCESS CARD STAGE 1 DIPEROLEH.");
    }

    public void ResetStage1AccessCard()
    {
        hasStage1AccessCard = false;
    }
}
