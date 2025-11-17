using UnityEngine;
using TMPro;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRadius = 0.5f;
    public LayerMask itemLayer;
    public string itemTag = "Item";
    public GameObject mensagemPainelPrefab;
    public TextMeshProUGUI mensagemTextPrefab;

    // ITEM ESPECIAL
    public string specialItemTag = "ItemEspecial";
    public GameObject painelEspecial; // Arraste o painel já pronto da cena aqui!

    // OUTRO PERSONAGEM
    public GameObject outroPersonagemPrefab;

    // SPAWN POINT PARA NOVO PERSONAGEM
    public Transform spawnPointNovoPersonagem;

    private bool personagemSpawnado = false;
    private int itensColetados = 0;
    public int itensNecessarios = 3;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, pickupRadius, Vector2.zero, 0f, itemLayer);
            if (hit.collider != null)
            {
                var item = hit.collider.gameObject;

                // --- Item comum ---
                if (item.CompareTag(itemTag))
                {
                    Destroy(item);
                    SpawnMensagemPainel();
                    itensColetados++;
                    if (!personagemSpawnado && itensColetados >= itensNecessarios)
                    {
                        SpawnOutroPersonagem();
                        personagemSpawnado = true;
                    }
                    return;
                }

                // --- Item especial: alterna o painel ---
                if (item.CompareTag(specialItemTag))
                {
                    AlternarPainelEspecial();
                    return;
                }
            }
        }
    }

    void SpawnMensagemPainel()
    {
        if (mensagemPainelPrefab != null && mensagemTextPrefab != null)
        {
            GameObject painel = Instantiate(mensagemPainelPrefab);
            TextMeshProUGUI texto = Instantiate(mensagemTextPrefab, painel.transform);
            texto.text = "Item coletado!";
        }
    }

    void AlternarPainelEspecial()
    {
        if (painelEspecial != null)
        {
            painelEspecial.SetActive(!painelEspecial.activeSelf);
        }
        else
        {
            Debug.LogWarning("Painel Especial não está atribuído no inspetor!");
        }
    }

    void SpawnOutroPersonagem()
    {
        if (outroPersonagemPrefab != null)
        {
            Vector3 pos = transform.position + Vector3.right * 2f;
            if (spawnPointNovoPersonagem != null)
            {
                pos = spawnPointNovoPersonagem.position;
            }

            Instantiate(outroPersonagemPrefab, pos, Quaternion.identity);

            // Desabilita o movimento do personagem antigo
            var movimento = GetComponent<PadreMoviment>();
            if (movimento != null)
                movimento.enabled = false;

            // Zera imediatamente a velocidade física do personagem antigo
            var rb = GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }
    }
}