using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Dano")]
    public int weakAttackDamage = 20;
    public int mediumAttackDamage = 40;

    [Header("Tempo / Cooldowns")]
    public float weakAttackHitDelay = 0.12f;
    public float mediumAttackHitDelay = 0.22f;
    public float weakAttackCooldown = 0.5f;
    public float mediumAttackCooldown = 0.9f;

    [Header("Gizmos / Range")]
    public float weakAttackRange = 1.0f;
    public Vector2 weakAttackOffset = new Vector2(0.6f, 0f);
    public float mediumAttackRange = 1.6f;
    public Vector2 mediumAttackOffset = new Vector2(0.9f, 0f);

    [Header("Detecção")]
    public string enemyTag = "NPC";
    public LayerMask enemyLayer = ~0;

    [Header("Opções")]
    public bool drawGizmos = true;

    // NOMES DOS TRIGGERS FIXOS — não precisam ser atribuídos no Inspector.
    // Se você quiser mudar os nomes das triggers, altere aqui no código.
    const string weakAttackTrigger = "WeakAttack";
    const string mediumAttackTrigger = "MediumAttack";

    // Animator será obtido automaticamente (GetComponent). Não precisa arrastar no Inspector.
    Animator animator;

    bool canAttack = true;

    void Reset()
    {
        // Tenta vincular o Animator automaticamente ao resetar o componente
        animator = GetComponent<Animator>();
    }

    void Awake()
    {
        // Garante que exista referência ao Animator sem precisar configurar no Inspector
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogWarning($"[{nameof(PlayerAttack)}] Nenhum Animator encontrado no GameObject '{name}'. As animações não irão tocar até que um Animator com um Controller esteja presente.");
    }

    void Update()
    {
        if (!canAttack) return;

        // ataque rápido (botão esquerdo)
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(PerformAttack(weakAttackTrigger, weakAttackDamage));
        }

        // ataque médio/forte (botão direito)
        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(PerformAttack(mediumAttackTrigger, mediumAttackDamage));
        }
    }

    IEnumerator PerformAttack(string triggerName, int damage)
    {
        canAttack = false;

        // Dispara a animação pelo trigger (nome fixo definido no código)
        if (animator != null && !string.IsNullOrEmpty(triggerName))
            animator.SetTrigger(triggerName);

        // escolhe parâmetros do ataque com base no trigger
        float range = weakAttackRange;
        Vector2 offset = weakAttackOffset;
        float hitDelay = weakAttackHitDelay;
        float cooldown = weakAttackCooldown;

        if (triggerName == mediumAttackTrigger)
        {
            range = mediumAttackRange;
            offset = mediumAttackOffset;
            hitDelay = mediumAttackHitDelay;
            cooldown = mediumAttackCooldown;
        }

        // espera até o frame onde o ataque "acerta" (sincronizar com animação)
        if (hitDelay > 0f)
            yield return new WaitForSeconds(hitDelay);

        // posição do centro do hit (considerando offset local)
        Vector2 center = (Vector2)transform.position + offset;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, range, enemyLayer.value);

        foreach (var col in hits)
        {
            if (col == null) continue;
            if (!string.IsNullOrEmpty(enemyTag) && !col.CompareTag(enemyTag)) continue;

            // tenta aplicar dano via componente NPCHealth (ou qualquer método TakeDamage)
            var hp = col.GetComponent<NPCHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }
            else
            {
                col.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            }
        }

        // espera o restante do cooldown
        if (cooldown > hitDelay)
            yield return new WaitForSeconds(cooldown - hitDelay);

        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        // desenha hit area do ataque fraco (verde)
        Gizmos.color = Color.green;
        Vector3 weakCenter = transform.position + (Vector3)weakAttackOffset;
        Gizmos.DrawWireSphere(weakCenter, weakAttackRange);

        // desenha hit area do ataque médio (vermelho)
        Gizmos.color = Color.red;
        Vector3 medCenter = transform.position + (Vector3)mediumAttackOffset;
        Gizmos.DrawWireSphere(medCenter, mediumAttackRange);
    }
}