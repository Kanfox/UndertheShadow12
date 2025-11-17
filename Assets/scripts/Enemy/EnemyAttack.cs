using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Configurações de Ataque")]
    public Transform player;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public Animator animator;

    [Header("Configurações de Som")]
    public AudioSource audioSource;
    public AudioClip attackSound;
    [Tooltip("Intervalo mínimo em segundos entre reproduções do som")]
    public float soundInterval = 0.5f; // Agora configurável no Inspector

    private bool isAttacking = false;
    private float lastAttackTime = 0f;
    private float lastSoundTime = 0f;

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        // Se jogador está no alcance, não está atacando, e cooldown acabou
        if (distance <= attackRange && !isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(Attack());
        }
    }

    private System.Collections.IEnumerator Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // Inicia animação de ataque
        animator.SetTrigger("Attack");

        // Toca som de ataque se passou intervalo mínimo desde o último som
        if (audioSource != null && attackSound != null && Time.time >= lastSoundTime + soundInterval)
        {
            audioSource.PlayOneShot(attackSound);
            lastSoundTime = Time.time;
        }

        // Espera até o fim da animação para permitir novo ataque
        float attackDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
    }
}
