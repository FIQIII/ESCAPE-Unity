using UnityEngine;
using TMPro;

public class NPCDialogueEVA : MonoBehaviour
{
    [Header("Identitas NPC")]
    public string npcName = "Ilmuwan";

    [Header("Dialog Stage 1")]
    [TextArea(2, 5)]
    public string[] dialogueLines =
    {
        "Sistem daya fasilitas mati.",
        "Cari Access Card di laboratorium.",
        "Setelah mendapatkannya, aktifkan Power Generator.",
        "Lalu gunakan Terminal untuk membuka Exit Gate."
    };

    [Header("UI Dialog")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Jarak Dialog")]
    public Transform player;
    public float maxDialogueDistance = 5f;

    private int currentIndex = 0;
    private bool isDialogueActive = false;

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        // Kalau dialog sedang aktif dan player menjauh,
        // dialog otomatis ditutup.
        if (isDialogueActive && player != null)
        {
            float distance = Vector3.Distance(
                transform.position,
                player.position
            );

            if (distance > maxDialogueDistance)
            {
                HideDialogue();
            }
        }
    }

    public string GetPrompt()
    {
        return "[E]  BICARA";
    }

    public void Interact()
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
            return;

        // Kalau dialog belum aktif, mulai dari baris pertama.
        if (!isDialogueActive)
        {
            isDialogueActive = true;
            currentIndex = 0;

            ShowCurrentDialogue();
            return;
        }

        // Kalau sudah aktif, lanjut ke dialog berikutnya.
        currentIndex++;

        // Kalau sudah melewati baris terakhir,
        // tutup dialog.
        if (currentIndex >= dialogueLines.Length)
        {
            HideDialogue();
            return;
        }

        ShowCurrentDialogue();
    }

    private void ShowCurrentDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (dialogueText != null)
        {
            dialogueText.text =
                npcName + ": " + dialogueLines[currentIndex];
        }

        Debug.Log(
            npcName + ": " + dialogueLines[currentIndex]
        );
    }

    public void HideDialogue()
    {
        isDialogueActive = false;
        currentIndex = 0;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }
}