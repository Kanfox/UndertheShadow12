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

    // NOVO: Texto que aparece quando estiver na área de pegar o item
    public TextMeshProUGUI pickupPrompt;

    private void Start()
    {
        if (pickupPrompt != null)
            pickupPrompt.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }

    void Update()
    {
        // Mostrar/ocultar o prompt dependendo se há um item na área
        if (pickupPrompt != null)
        {
            Collider2D nearby = Physics2D.OverlapCircle(transform.position, pickupRadius, itemLayer);
            bool showPrompt = false;
            if (nearby != null)
            {
                if (nearby.CompareTag(itemTag) || nearby.CompareTag(specialItemTag))
                    showPrompt = true;
            }

            // Se o painel especial estiver aberto, não mostrar o prompt
            if (painelEspecial != null && painelEspecial.activeSelf)
                showPrompt = false;

            pickupPrompt.gameObject.SetActive(showPrompt);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            // Se o painel especial já estiver aberto, permitir fechar independentemente da distância
            if (painelEspecial != null && painelEspecial.activeSelf)
            {
                AlternarPainelEspecial();
                return;
            }

            RaycastHit2D hit = Physics2D.CircleCast(transform.position, pickupRadius, Vector2.zero, 0f, itemLayer);
            if (hit.collider != null)
            {
                var item = hit.collider.gameObject;

                // --- Item comum ---
                if (item.CompareTag(itemTag))
                {
                    Destroy(item);

                    // esconder o prompt ao coletar o item
                    if (pickupPrompt != null)
                        pickupPrompt.gameObject.SetActive(false);

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

            // Se o painel foi ativado, garantir que o prompt de pegar seja desativado imediatamente
            if (painelEspecial.activeSelf && pickupPrompt != null)
                pickupPrompt.gameObject.SetActive(false);

            // Se o painel foi desativado, reaparecer o prompt apenas para o item especial
            // (aparecerá aqui apenas se o jogador estiver dentro da área do item especial;
            // ao sair da área, o Update() continuará a esconder o prompt normalmente)
            if (!painelEspecial.activeSelf && pickupPrompt != null)
            {
                Collider2D nearby = Physics2D.OverlapCircle(transform.position, pickupRadius, itemLayer);
                if (nearby != null && nearby.CompareTag(specialItemTag))
                {
                    pickupPrompt.gameObject.SetActive(true);
                }
            }
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