using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRadius = 2f;
    public string itemTag = "Item";
    public LayerMask itemLayer;

    public GameObject novoPersonagemPrefab; // Prefab do novo personagem a ser spawnado
    public Transform spawnPoint;            // Ponto arrastável para spawn no inspetor

    private int coletados = 0;
    private bool novoPersonagemSpawnado = false;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }

    void Update()
    {
        if (novoPersonagemSpawnado)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider2D[] itemsNaArea = Physics2D.OverlapCircleAll(transform.position, pickupRadius, itemLayer);
            Debug.Log("Detectados " + itemsNaArea.Length + " itens na área!");

            foreach (Collider2D item in itemsNaArea)
            {
                Debug.Log("Nome detectado: " + item.name + " | Tag: " + item.tag + " | Layer:" + LayerMask.LayerToName(item.gameObject.layer));
                if (item.CompareTag(itemTag))
                {
                    Destroy(item.gameObject);
                    coletados++;
                    Debug.Log("Item coletado: " + item.name);

                    if (coletados >= 3)
                    {
                        SpawnNovoPersonagem();
                        break;
                    }
                }
            }
        }
    }

    void SpawnNovoPersonagem()
    {
        novoPersonagemSpawnado = true;

        // Usa o ponto de spawn do inspetor; se não existir, cria ao lado do personagem antigo
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : (transform.position + Vector3.right * 2f);
        GameObject novo = Instantiate(novoPersonagemPrefab, spawnPos, Quaternion.identity);

        // Desativa qualquer movimento do personagem antigo

        // 1. Desativa o script de movimentação Padre Moviment
        MonoBehaviour padreMov = GetComponent("Padre Moviment") as MonoBehaviour;
        if (padreMov != null)
            padreMov.enabled = false;

        // 2. Set Rigidbody2D para Static e zera velocidades
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static;
        }

        // 3. Reseta a rotação deste objeto para zero
        transform.rotation = Quaternion.identity;

        // 4. Se a câmera for filha do personagem antigo, desparenta
        if (Camera.main != null && Camera.main.transform.parent == this.transform)
            Camera.main.transform.parent = null;

        // 5. Desabilita todos os outros scripts possíveis exceto este
        foreach (MonoBehaviour script in GetComponents<MonoBehaviour>())
        {
            if (script != this && script != padreMov)
                script.enabled = false;
        }

        // 6. Este script também se desativa
        this.enabled = false;

        // --- Se precisar transferir controle/câmera, faça aqui. Exemplo:
        // Camera.main.GetComponent<CameraFollow>().target = novo.transform;
        // novo.GetComponent<NOVOSCRIPT_DE_MOVIMENTO>().enabled = true;
    }
}