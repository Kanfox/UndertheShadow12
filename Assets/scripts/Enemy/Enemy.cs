using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movimentação")]
    public float moveSpeed = 2f;
    public Transform pointA;
    public Transform pointB;

    private Vector3 target;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        target = pointB.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Mover();
    }

    void Mover()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        // Virar sprite de acordo com direção
        if (target.x < transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        // Trocar o ponto-alvo
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            if (target == pointA.position)
                target = pointB.position;
            else
                target = pointA.position;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Aqui você pode chamar um método para dar dano no jogador
            Debug.Log("O inimigo causou dano ao jogador!");
        }
    }
}

