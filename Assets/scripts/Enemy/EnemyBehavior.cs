using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour
{
    public float detectionRange = 5f;
    public float attackRange = 1.2f;
    public float moveSpeed = 2f;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;

    private Vector2 movement;
    private bool isAttacking = false;

    private PlayerHealth playerHealth; // ✅ Referência à vida do jogador

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        // ✅ Se o jogador morreu, o inimigo para e fica em idle
        if (playerHealth != null && playerHealth.isDead)
        {
            movement = Vector2.zero;
            SetAnimationStates(true, false, false);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            movement = Vector2.zero;

            if (!isAttacking)
            {
                StartCoroutine(Attack());
            }

            SetAnimationStates(false, false, true); // Atacando
        }
        else if (distanceToPlayer <= detectionRange)
        {
            movement = (player.position - transform.position).normalized;
            SetAnimationStates(false, true, false); // Andando
        }
        else
        {
            movement = Vector2.zero;
            SetAnimationStates(true, false, false); // Idle
        }

        // Flip do inimigo para olhar na direção do jogador
        Vector3 scale = transform.localScale;

        if (player.position.x < transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x); // Olha para a esquerda
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x); // Olha para a direita
        }

        transform.localScale = scale;
    }

    void FixedUpdate()
    {
        if (!isAttacking)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        // Aguarda antes de atacar (tempo de animação)
        yield return new WaitForSeconds(0.4f);

        // ✅ Verifica se player ainda existe ou já morreu
        if (playerHealth == null || playerHealth.isDead)
        {
            isAttacking = false;
            yield break;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            playerHealth.TakeDamage(5);
        }

        // Tempo até sair do ataque
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }

    void SetAnimationStates(bool idle, bool walking, bool attacking)
    {
        if (animator != null)
        {
            animator.SetBool("isIdle", idle);
            animator.SetBool("isWalking", walking);
            animator.SetBool("isAttacking", attacking);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
