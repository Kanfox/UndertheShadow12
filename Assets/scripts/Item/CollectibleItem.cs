using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Tooltip("ID ou nome do item (opcional)")]
    public string itemId = "";

    [Tooltip("Som opcional ao coletar")]
    public AudioClip collectSound;

    [Tooltip("Partículas opcionais ao coletar")]
    public GameObject collectVFXPrefab;

    // Configuração dos Range Gizmos (até 3 ranges)
    [Header("Configuração dos Range Gizmos (até 3 ranges)")]
    public float rangeGizmo1 = 0.06f;
    public Vector3 offsetGizmo1 = Vector3.zero;
    public Color colorGizmo1 = Color.yellow;

    public float rangeGizmo2 = 0.06f;
    public Vector3 offsetGizmo2 = Vector3.zero;
    public Color colorGizmo2 = Color.green;

    public float rangeGizmo3 = 0.06f;
    public Vector3 offsetGizmo3 = Vector3.zero;
    public Color colorGizmo3 = Color.red;

    // Chamado quando o item é coletado pelo player
    public void Collect()
    {
        // Tocar som (se hover)
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // Instanciar efeitos visuais (se hover)
        if (collectVFXPrefab != null)
        {
            Instantiate(collectVFXPrefab, transform.position, Quaternion.identity);
        }

        // Aqui você pode adicionar lógica extra (notificar Inventory, GameManager, etc.)
        // Ex.: GameManager.Instance.OnItemCollected(itemId);

        // Destrói o objeto (some do mapa)
        Destroy(gameObject);
    }

    // Desenho simples no editor para destacar os itens (ranges esféricos)
    private void OnDrawGizmos()
    {
        // Gizmo 1
        Gizmos.color = colorGizmo1;
        Gizmos.DrawSphere(transform.position + offsetGizmo1, rangeGizmo1);

        // Gizmo 2
        Gizmos.color = colorGizmo2;
        Gizmos.DrawSphere(transform.position + offsetGizmo2, rangeGizmo2);

        // Gizmo 3
        Gizmos.color = colorGizmo3;
        Gizmos.DrawSphere(transform.position + offsetGizmo3, rangeGizmo3);
    }
}