using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{
    [Header("Referensi")]
    [SerializeField] private PlayerHealthEVA playerHealth;
    [SerializeField] private RectTransform hpFill;
    [SerializeField] private Text hpText;

    private Image hpFillImage;

    private readonly Color32 fullColor =
        new Color32(30, 255, 70, 255);   // #1EFF46

    private readonly Color32 midColor =
        new Color32(255, 200, 0, 255);   // kuning

    private readonly Color32 lowColor =
        new Color32(255, 50, 50, 255);   // merah

    private void Awake()
    {
        if (hpFill != null)
        {
            hpFillImage = hpFill.GetComponent<Image>();
        }
    }

    private void Update()
    {
        if (playerHealth == null || hpFill == null)
            return;

        float currentHP = playerHealth.CurrentHealth;
        float maxHP = playerHealth.MaxHealth;

        if (maxHP <= 0f)
            return;

        float normalizedHP = Mathf.Clamp01(currentHP / maxHP);

        // Bar menyusut dari kanan ke kiri.
        hpFill.localScale = new Vector3(
            normalizedHP,
            1f,
            1f
        );

        // Ubah warna berdasarkan sisa HP.
        if (hpFillImage != null)
        {
            if (normalizedHP > 0.6f)
            {
                hpFillImage.color = fullColor;
            }
            else if (normalizedHP > 0.3f)
            {
                hpFillImage.color = midColor;
            }
            else
            {
                hpFillImage.color = lowColor;
            }
        }

        // Update angka HP.
        if (hpText != null)
        {
            hpText.text =
                "HP: " +
                Mathf.CeilToInt(currentHP) +
                " / " +
                Mathf.CeilToInt(maxHP);
        }
    }
}