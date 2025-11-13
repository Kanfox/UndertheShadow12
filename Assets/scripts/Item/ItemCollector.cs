using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemCollector : MonoBehaviour
{
    [Header("Detecção")]
    [Tooltip("Raio em unidades do mundo (1 unidade = 1 metro).")]
    public float collectRadius = 1f;

    [Tooltip("Layer(s) onde os itens colecionáveis estão.")]
    public LayerMask collectibleLayer;

    [Tooltip("Tag dos itens colecionáveis (verifique no inspetor / Tags do Unity).")]
    public string collectibleTag = "Collectible";

    [Header("Input")]
    [Tooltip("Tecla para coletar itens.")]
    public KeyCode collectKey = KeyCode.E;

    [Header("Meta")]
    [Tooltip("Quantidade de itens necessários (opcional).")]
    public int requiredCount = 3;

    [Tooltip("Quantidade já coletada (mostrada no inspetor).")]
    public int collectedCount = 0;

    // Tenta coletar o item mais próximo dentro do raio
    private void Update()
    {
        if (Input.GetKeyDown(collectKey))
        {
            TryCollectNearest();
        }
    }

    private void TryCollectNearest()
    {
        // Procura colisores dentro do raio filtrando por layer
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, collectRadius, collectibleLayer);

        if (hits == null || hits.Length == 0) return;

        // Filtra por tag e pega o mais próximo (caso haja vários)
        var valid = hits.Where(h => h != null && h.CompareTag(collectibleTag)).ToArray();
        if (valid.Length == 0) return;

        Collider2D nearest = valid
            .OrderBy(h => (h.transform.position - transform.position).sqrMagnitude)
            .FirstOrDefault();

        if (nearest == null) return;

        // Tenta obter o componente CollectibleItem para executar Collect(), se houver.
        CollectibleItem ci = nearest.GetComponent<CollectibleItem>();
        if (ci != null)
        {
            ci.Collect();
        }
        else
        {
            // Se não houver o script, apenas destrói o objeto
            Destroy(nearest.gameObject);
        }

        collectedCount++;

        // Exemplo: ação quando alcançar a meta
        if (collectedCount >= requiredCount)
        {
            Debug.Log("Meta alcançada! Itens coletados: " + collectedCount);
            // Aqui você pode chamar um GameManager, abrir porta, etc.
        }
    }

    // Desenha gizmos no editor para visualizar o alcance e itens dentro do alcance
    private void OnDrawGizmosSelected()
    {
        // Círculo do alcance
        Gizmos.color = new Color(0f, 0.7f, 1f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, collectRadius);

        // Mostra linhas para itens dentro do alcance (só no editor/running)
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, collectRadius, collectibleLayer);
        if (hits == null) return;

        Gizmos.color = Color.green;
        foreach (var h in hits)
        {
            if (h == null) continue;
            if (!h.CompareTag(collectibleTag)) continue;

            Gizmos.DrawLine(transform.position, h.transform.position);
            Gizmos.DrawSphere(h.transform.position, 0.04f);
        }
    }
}