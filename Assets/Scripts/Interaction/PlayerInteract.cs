using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaksi")]
    public Camera playerCamera;
    public float interactDistance = 4f;
    public float interactRadius = 0.18f;
    public LayerMask interactMask = ~0;

    [Header("UI Interaksi")]
    public GameObject interactionPrompt;
    public Text interactionText;

    private PlayerInventoryEVA inventory;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        inventory = GetComponent<PlayerInventoryEVA>();

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    private void Update()
    {
        if (playerCamera == null)
            return;

        UpdateInteractionPrompt();

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryInteract();
        }
    }

    private RaycastHit[] GetHits()
    {
        RaycastHit[] hits = Physics.SphereCastAll(
            playerCamera.transform.position,
            interactRadius,
            playerCamera.transform.forward,
            interactDistance,
            interactMask,
            QueryTriggerInteraction.Ignore
        );

        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        return hits;
    }

    private void UpdateInteractionPrompt()
    {
        if (interactionPrompt == null || interactionText == null)
            return;

        RaycastHit[] hits = GetHits();

        // =========================
        // FLASHLIGHT
        // =========================
        foreach (RaycastHit hit in hits)
        {
            FlashlightPickup flashlight =
                hit.collider.GetComponentInParent<FlashlightPickup>();

            if (flashlight != null)
            {
                ShowPrompt("[E]  AMBIL SENTER");
                return;
            }
        }

        // =========================
        // ACCESS CARD
        // =========================
        foreach (RaycastHit hit in hits)
        {
            AccessCardPickup card =
                hit.collider.GetComponentInParent<AccessCardPickup>();

            if (card != null)
            {
                ShowPrompt("[E]  AMBIL ACCESS CARD");
                return;
            }
        }

        // =========================    
        // POWER GENERATOR
        // =========================
        foreach (RaycastHit hit in hits)
        {
            PowerGeneratorEVA generator =
                hit.collider.GetComponentInParent<PowerGeneratorEVA>();

            if (generator != null)
            {
                if (generator.IsActivated)
                {
                    ShowPrompt("GENERATOR AKTIF");
                }
                else if (inventory == null || !inventory.HasStage1AccessCard)
                {
                    ShowPrompt("ACCESS CARD DIPERLUKAN");
                }
                else
                {
                    ShowPrompt("[E]  AKTIFKAN GENERATOR");
                }

                return;
            }
        }

        // =========================
        // TERMINAL
        // =========================
        foreach (RaycastHit hit in hits)
        {
            TerminalEVA terminal =
                hit.collider.GetComponentInParent<TerminalEVA>();

            if (terminal != null)
            {
                ShowPrompt(terminal.GetPrompt(inventory));
                return;
            }
        }

        // =========================
        // NPC
        // =========================
        foreach (RaycastHit hit in hits)
        {
            NPCDialogueEVA npc =
                hit.collider.GetComponentInParent<NPCDialogueEVA>();

            if (npc != null)
            {
                ShowPrompt(npc.GetPrompt());
                return;
            }
        }

        // =========================
        // DOOR
        // =========================
        foreach (RaycastHit hit in hits)
        {
            EVADoorController door =
                hit.collider.GetComponentInParent<EVADoorController>();

            if (door != null)
            {
                if (door.locked)
                {
                    ShowPrompt("PINTU TERKUNCI");
                }
                else if (door.IsOpen)
                {
                    ShowPrompt("[E]  TUTUP PINTU");
                }
                else
                {
                    ShowPrompt("[E]  BUKA PINTU");
                }

                return;
            }
        }

        HidePrompt();
    }

    private void TryInteract()
    {
        RaycastHit[] hits = GetHits();

        // =========================
        // FLASHLIGHT
        // =========================
        foreach (RaycastHit hit in hits)
        {
            FlashlightPickup flashlight =
                hit.collider.GetComponentInParent<FlashlightPickup>();

            if (flashlight != null)
            {
                flashlight.Interact();
                HidePrompt();
                return;
            }
        }

        // =========================
        // ACCESS CARD
        // =========================
        foreach (RaycastHit hit in hits)
        {
            AccessCardPickup card =
                hit.collider.GetComponentInParent<AccessCardPickup>();

            if (card != null)
            {
                if (inventory == null)
                {
                    Debug.LogWarning(
                        "Player belum memiliki komponen PlayerInventoryEVA."
                    );

                    return;
                }

                card.Interact(inventory);
                HidePrompt();
                return;
            }
        }

        // =========================
        // POWER GENERATOR
        // =========================
        foreach (RaycastHit hit in hits)
        {
            PowerGeneratorEVA generator =
                hit.collider.GetComponentInParent<PowerGeneratorEVA>();

            if (generator != null)
            {
                generator.Interact();
                return;
            }
        }

        // =========================
        // TERMINAL
        // =========================
        foreach (RaycastHit hit in hits)
        {
            TerminalEVA terminal =
                hit.collider.GetComponentInParent<TerminalEVA>();

            if (terminal != null)
            {
                terminal.Interact(inventory);
                return;
            }
        }

        // =========================
        // NPC
        // =========================
        foreach (RaycastHit hit in hits)
        {
            NPCDialogueEVA npc =
                hit.collider.GetComponentInParent<NPCDialogueEVA>();

            if (npc != null)
            {
                npc.Interact();
                return;
            }
        }

        // =========================
        // DOOR
        // =========================
        foreach (RaycastHit hit in hits)
        {
            EVADoorController door =
                hit.collider.GetComponentInParent<EVADoorController>();

            if (door != null)
            {
                door.Interact();
                return;
            }
        }
    }

    private void ShowPrompt(string message)
    {
        if (interactionText != null)
            interactionText.text = message;

        if (interactionPrompt != null &&
            !interactionPrompt.activeSelf)
        {
            interactionPrompt.SetActive(true);
        }
    }

    private void HidePrompt()
    {
        if (interactionPrompt != null &&
            interactionPrompt.activeSelf)
        {
            interactionPrompt.SetActive(false);
        }
    }
}