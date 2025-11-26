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

    [Header("Som ao morrer")]
    public AudioSource audioSource;
    public AudioClip deathSound;

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
    public GameObject deathPrefab;
    [Tooltip("Se usar deathPrefab, se true o objeto original será destruído imediatamente; se false será desativado.")]
    public bool destroyOriginalWhenSpawnPrefab = true;

    Rigidbody2D rb;
    Collider2D mainCollider;
    bool isDead = false;

    float deathFixedY;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<Collider2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

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

            if (disableColliderOnDeath && mainCollider != null)
                mainCollider.enabled = false;

            if (extraCollidersToDisable != null)
            {
                foreach (var c in extraCollidersToDisable)
                    if (c != null) c.enabled = false;
            }

            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // 🔊 SOM DE MORTE
        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        deathFixedY = transform.position.y;

        if (useDeathPrefab && deathPrefab != null)
        {
            Instantiate(deathPrefab, transform.position, transform.rotation);

            if (destroyOriginalWhenSpawnPrefab)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);

            return;
        }

        if (animator != null && !string.IsNullOrEmpty(deathTrigger))
            animator.SetTrigger(deathTrigger);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            if (disableRigidbodyOnDeath)
                rb.simulated = false;
        }

        if (disableOnDeath != null)
        {
            foreach (var comp in disableOnDeath)
                if (comp != null) comp.enabled = false;
        }

        DisableComponentByName("EnemyBehavior");

        BroadcastMessage("DisableAttack", SendMessageOptions.DontRequireReceiver);
        BroadcastMessage("StopChasing", SendMessageOptions.DontRequireReceiver);
        BroadcastMessage("OnDeath", SendMessageOptions.DontRequireReceiver);

        StartCoroutine(DestroyAfterDelayCoroutine());
    }

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

        Component c = GetComponent(name);
        if (c is Behaviour b0) b0.enabled = false;

        var childComps = GetComponentsInChildren<Component>(true);
        foreach (var comp in childComps)
        {
            if (comp == null) continue;
            if (comp.GetType().Name == name && comp is Behaviour bb) bb.enabled = false;
        }

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
