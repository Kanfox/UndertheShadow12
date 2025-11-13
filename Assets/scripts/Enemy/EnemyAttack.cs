using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Transform player;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public Animator animator;

    private bool isAttacking = false;
    private float lastAttackTime = 0f;

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

        // Espera até o fim da animação para permitir novo ataque
        // Você pode ajustar isso para tempo fixo se preferir
        float attackDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
    }
}
