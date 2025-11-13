using UnityEngine;

public class PlayerController : MonoBehaviour
{
   
    public float speed = 5;
    public float jumpForce = 7;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded = true;
    private Vector3 originalScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        float mov = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(mov * speed, rb.linearVelocity.y);

        // Virar personagem
        if (mov > 0)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else if (mov < 0)
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        // Pulo
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        // Atualizar animações movimento
        bool isJumping = !isGrounded;
        bool isRunning = Mathf.Abs(mov) > 0 && isGrounded;
        bool isIdle = mov == 0 && isGrounded;

        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isIdle", isIdle);

      
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Chao"))
        {
            isGrounded = true;
        }
    }
}
