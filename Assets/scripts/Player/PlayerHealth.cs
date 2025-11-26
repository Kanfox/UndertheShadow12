using UnityEngine;
using UnityEngine.SceneManagement; // Para reiniciar cenas
using System.Collections; // Necessário para usar IEnumerator
using TMPro; // Para TextMeshProUGUI

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    private Animator animator;
    public bool isDead = false;

    [Header("Painel de Game Over")]
    public GameObject gameOverPanel; // Painel a ser ativado

    [Header("UI")]
    public TextMeshProUGUI healthText; // Texto na tela que exibirá a vida do player (arraste no Inspector)

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false); // Garante que o painel esteja escondido no começo

        UpdateHealthText();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        // VERIFICA A DEFESA DO PLAYER ANTES DE CAUSAR DANO!
        PlayerDefend playerDefend = GetComponent<PlayerDefend>();
        if (playerDefend != null && playerDefend.IsDefending())
        {
            Debug.Log("Defendeu! Não levou dano.");
            return; // Sai da função sem tomar dano
        }

        // CÓDIGO ORIGINAL (não modificado)
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log("Player tomou dano! Vida restante: " + currentHealth);

        UpdateHealthText();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Método público para alterar vida (por exemplo cura)
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

    // Atualiza o texto na UI (seguro para null)
    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"HP: {currentHealth} / {maxHealth}";
        }
    }

    void Die()
    {
        isDead = true;

        Debug.Log("Player morreu!");

        // Ativa a animação de morte
        if (animator != null)
            animator.SetBool("isDead", true);

        // Desativa o controle do jogador
        PlayerController movementScript = GetComponent<PlayerController>();
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        // Garante que o texto mostre 0 ao morrer
        currentHealth = 0;
        UpdateHealthText();

        // Inicia corrotina para mostrar o painel com delay
        StartCoroutine(ShowGameOverPanelWithDelay(1.5f)); // Delay de 1.5 segundos (ajustável)
    }

    private IEnumerator ShowGameOverPanelWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    // Chamado pelo botão "Reiniciar"
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Chamado pelo botão "Sair para Menu"
    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu"); // Certifique-se que a cena "Menu" está adicionada no Build Settings
    }
}