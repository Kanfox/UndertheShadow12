using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    private Animator animator;
    public bool isDead = false;

    [Header("Painel de Game Over")]
    public GameObject gameOverPanel;

    [Header("UI")]
    public TextMeshProUGUI healthText;

    [Header("Som ao morrer")]
    public AudioSource audioSource;
    public AudioClip deathSound;

    [Header("Som pós-morte (toca depois de 2s)")]
    public AudioClip afterDeathSound;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateHealthText();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        PlayerDefend playerDefend = GetComponent<PlayerDefend>();
        if (playerDefend != null && playerDefend.IsDefending())
        {
            Debug.Log("Defendeu! Não levou dano.");
            return;
        }

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log("Player tomou dano! Vida restante: " + currentHealth);

        UpdateHealthText();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ChangeHealth(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthText();

        if (currentHealth <= 0)
            Die();
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"HP: {currentHealth} / {maxHealth}";
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player morreu!");

        // 🔊 SOM DE MORTE IMEDIATO
        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        // 🔊 SOM 2 SEGUNDOS APÓS A MORTE
        StartCoroutine(PlayAfterDeathSound());

        if (animator != null)
            animator.SetBool("isDead", true);

        PlayerController movementScript = GetComponent<PlayerController>();
        if (movementScript != null)
            movementScript.enabled = false;

        currentHealth = 0;
        UpdateHealthText();

        StartCoroutine(ShowGameOverPanelWithDelay(1.5f));
    }

    private IEnumerator PlayAfterDeathSound()
    {
        yield return new WaitForSeconds(2f);

        if (audioSource != null && afterDeathSound != null)
            audioSource.PlayOneShot(afterDeathSound);
    }

    private IEnumerator ShowGameOverPanelWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
