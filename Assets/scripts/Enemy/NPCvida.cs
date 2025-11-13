using System.Collections;
using UnityEngine;

public class NPCHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 100;
    [HideInInspector] public int currentHealth;

    [Header("Animação")]
    public Animator animator;
    public string hitTrigger = "Hit";
    public string deathTrigger = "Die";

    [Header("Comportamento ao morrer")]
    public MonoBehaviour[] disableOnDeath;
    public bool disableRigidbodyOnDeath = true;
    public bool disableColliderOnDeath = true;
    public Collider2D[] extraCollidersToDisable;

    [Header("Desaparecer após morte")]
    public float destroyDelay = 3f;

    [Header("Opções de correção de 'afundamento'")]
    [Tooltip("Se true: trava Y do Transform no LateUpdate para impedir que a animação mova o NPC.")]
    public bool lockYInLateUpdate = true;
    [Tooltip("Se preferir instanciar um prefab de 'cadáver' (não afunda), marque true e atribua o prefab.")]
    public bool useDeathPrefab = false;
    public GameObject deathPrefab; // prefab que contém a animação de morte sem curvas de posição (opcional)
    [Tooltip("Se usar deathPrefab, se true o objeto original será destruído imediatamente; se false será desativado e deathPrefab ficará no lugar.")]
    public bool destroyOriginalWhenSpawnPrefab = true;

    // estado interno
    Rigidbody2D rb;
    Collider2D mainCollider;
    bool isDead = false;

    // valor Y a ser mantido
    float deathFixedY;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<Collider2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        // evita root motion e força modo Normal (assim garantimos que LateUpdate sobreporá)
        if (animator != null)
        {
            animator.applyRootMotion = false;
            animator.updateMode = AnimatorUpdateMode.Normal;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (animator != null && !string.IsNullOrEmpty(hitTrigger))
            animator.SetTrigger(hitTrigger);

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            // desativa collider imediatamente
            if (disableColliderOnDeath && mainCollider != null)
                mainCollider.enabled = false;

            if (extraCollidersToDisable != null)
            {
                foreach (var c in extraCollidersToDisable)
                    if (c != null) c.enabled = false;
            }

            // entra na rotina de morte
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // registra Y atual para manter a posição do chão
        deathFixedY = transform.position.y;

        // se optar por spawnar um prefab de cadáver, instancie e finalize aqui
        if (useDeathPrefab && deathPrefab != null)
        {
            // instanciar no mesmo local (mantém a aparência, mas o prefab deve ter animação sem posição)
            Instantiate(deathPrefab, transform.position, transform.rotation);

            // desativa/destrói o original de acordo com a configuração
            if (destroyOriginalWhenSpawnPrefab)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);

            return; // nada mais para fazer no objeto original
        }

        // toca animação de morte no Animator do próprio objeto (se existir)
        if (animator != null && !string.IsNullOrEmpty(deathTrigger))
            animator.SetTrigger(deathTrigger);

        // para física
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            if (disableRigidbodyOnDeath)
                rb.simulated = false;
        }

        // desativa scripts listados
        if (disableOnDeath != null)
        {
            foreach (var comp in disableOnDeath)
                if (comp != null) comp.enabled = false;
        }

        // desativa especificamente por nome (EnemyBehavior) sem precisar do tipo
        DisableComponentByName("EnemyBehavior");

        // envia mensagens para garantir que outros scripts parem
        BroadcastMessage("DisableAttack", SendMessageOptions.DontRequireReceiver);
        BroadcastMessage("StopChasing", SendMessageOptions.DontRequireReceiver);
        BroadcastMessage("OnDeath", SendMessageOptions.DontRequireReceiver);

        // inicia a destruição após delay
        StartCoroutine(DestroyAfterDelayCoroutine());
    }

    // trava Y depois que a animação foi aplicada (LateUpdate sobrepõe a animação)
    void LateUpdate()
    {
        if (isDead && lockYInLateUpdate)
        {
            Vector3 p = transform.position;
            p.y = deathFixedY;
            transform.position = p;
        }
    }

    void DisableComponentByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return;

        // self
        Component c = GetComponent(name);
        if (c is Behaviour b0) b0.enabled = false;

        // children
        var childComps = GetComponentsInChildren<Component>(true);
        foreach (var comp in childComps)
        {
            if (comp == null) continue;
            if (comp.GetType().Name == name && comp is Behaviour bb) bb.enabled = false;
        }

        // parents
        var parentComps = GetComponentsInParent<Component>(true);
        foreach (var comp in parentComps)
        {
            if (comp == null) continue;
            if (comp.GetType().Name == name && comp is Behaviour bb) bb.enabled = false;
        }
    }

    IEnumerator DestroyAfterDelayCoroutine()
    {
        yield return new WaitForSeconds(Mathf.Max(0f, destroyDelay));
        Destroy(gameObject);
    }

    [ContextMenu("Force Die")]
    public void ForceDie()
    {
        TakeDamage(int.MaxValue);
    }
}