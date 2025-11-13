using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Inimigo se move normalmente (somente rotação travada)
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player encostou no inimigo!");
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Trava o movimento no X enquanto o player encosta
            if (rb.constraints != (RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation))
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                Debug.Log("Travando posição do inimigo enquanto o player encosta!");
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Libera movimento no X de novo
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            Debug.Log("Player saiu, liberando movimento do inimigo!");
        }
    }
}