using UnityEngine;

public class PadreMoviment : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidade do personagem
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Entrada horizontal (A/D ou Setas)
        moveInput = Input.GetAxisRaw("Horizontal");

        // Atualiza anima��o: Idle ou Andando
        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        // Inverte o sprite conforme a dire��o
        if (moveInput > 0)
            spriteRenderer.flipX = false;
        else if (moveInput < 0)
            spriteRenderer.flipX = true;
    }

    void FixedUpdate()
    {
        // Movimento horizontal
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }
}
