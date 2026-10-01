using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthEVA : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("UI Health")]
    [SerializeField] private Text healthText;

    [Header("Game Over")]
    [SerializeField] private GameOverEVA gameOverManager;

    [Header("Status")]
    [SerializeField] private bool isDead = false;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    private void Start()
    {
        currentHealth = maxHealth;
        isDead = false;

        UpdateHealthUI();

        Debug.Log(
            "PLAYER HP = " +
            currentHealth +
            "/" +
            maxHealth
        );
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        if (damage <= 0f)
            return;

        currentHealth -= damage;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        UpdateHealthUI();

        Debug.Log(
            "PLAYER TERKENA DAMAGE " +
            damage +
            " | HP = " +
            currentHealth +
            "/" +
            maxHealth
        );

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void InstantDeath()
    {
        if (isDead)
            return;

        currentHealth = 0f;

        UpdateHealthUI();

        Debug.Log(
            "PLAYER TERKENA SERANGAN FATAL."
        );

        Die();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log(
            "PLAYER MATI - GAME OVER."
        );

        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }
        else
        {
            Debug.LogWarning(
                "GameOverEVA belum dihubungkan ke PlayerHealthEVA."
            );
        }
    }

    public void RestoreHealth(float amount)
    {
        if (isDead)
            return;

        if (amount <= 0f)
            return;

        currentHealth += amount;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        UpdateHealthUI();

        Debug.Log(
            "PLAYER HEAL +" +
            amount +
            " | HP = " +
            currentHealth +
            "/" +
            maxHealth
        );
    }

    private void UpdateHealthUI()
    {
        if (healthText == null)
            return;

        healthText.text =
            "HP: " +
            Mathf.CeilToInt(currentHealth) +
            " / " +
            Mathf.CeilToInt(maxHealth);
    }
}