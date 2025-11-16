using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour
{
    public float detectionRange = 5f;
    public float attackRange = 1.2f;
    public float moveSpeed = 2f;

    // 🎵 Som ao andar
    public AudioSource walkAudio;
    public AudioClip walkClip;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;

    private Vector2 movement;
    private bool isAttacking = false;

    private PlayerHealth playerHealth;
    private bool wasWalking = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        // Se o jogador morreu, o inimigo para
        if (playerHealth != null && playerHealth.isDead)
        {
            movement = Vector2.zero;
            SetAnimationStates(true, false, false);

            if (walkAudio != null)
                walkAudio.Stop();

            wasWalking = false;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Ataque
        if (distanceToPlayer <= attackRange)
        {
            movement = Vector2.zero;

            if (!isAttacking)
                StartCoroutine(Attack());

            SetAnimationStates(false, false, true);
        }
        // Detecção
        else if (distanceToPlayer <= detectionRange)
        {
            movement = (player.position - transform.position).normalized;
            SetAnimationStates(false, true, false);
        }
        // Idle
        else
        {
            movement = Vector2.zero;
            SetAnimationStates(true, false, false);
        }

        // Flip do inimigo
        Vector3 scale = transform.localScale;
        scale.x = (player.position.x < transform.position.x) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;

        // 🎵 Detecta início/fim do movimento
        bool isWalking = movement.magnitude > 0.1f;

        if (isWalking && !wasWalking)
        {
            if (walkAudio != null && walkClip != null)
            {
                walkAudio.clip = walkClip;
                walkAudio.loop = true;
                walkAudio.Play();
            }
        }
        else if (!isWalking && wasWalking)
        {
            if (walkAudio != null)
                walkAudio.Stop();
        }

        wasWalking = isWalking;
    }

    void FixedUpdate()
    {
        if (!isAttacking)
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        yield return new WaitForSeconds(0.4f);

        if (playerHealth == null || playerHealth.isDead)
        {
            isAttacking = false;
            yield break;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
            playerHealth.TakeDamage(5);

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
