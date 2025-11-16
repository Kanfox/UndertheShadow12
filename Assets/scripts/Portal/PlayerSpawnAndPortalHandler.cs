using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnAndPortalHandler : MonoBehaviour
{
    [Header("Portal Settings")]
    public GameObject portalPrefab;
    public Transform portalSpawnPoint;
    public string sceneDestino = "Cena cidade"; // Altere aqui para o nome correto da cena

    private GameObject spawnedPortal;

    void Start()
    {
        // Quando este player for spawnado, espera 3 segundos e instancia o portal
        Invoke(nameof(SpawnPortal), 3f);
    }

    void SpawnPortal()
    {
        if (portalPrefab != null && portalSpawnPoint != null)
        {
            spawnedPortal = Instantiate(portalPrefab, portalSpawnPoint.position, Quaternion.identity);

            // Adiciona automaticamente o script de detecção ao portal recém-criado
            PortalTrigger trigger = spawnedPortal.AddComponent<PortalTrigger>();
            trigger.nomeDaCenaDestino = sceneDestino;
        }
        else
        {
            Debug.LogWarning("PortalPrefab ou PortalSpawnPoint não configurado!");
        }
    }

    // Script auxiliar interno para teleporte
    public class PortalTrigger : MonoBehaviour
    {
        public string nomeDaCenaDestino;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                SceneManager.LoadScene(nomeDaCenaDestino);
            }
        }
    }
}