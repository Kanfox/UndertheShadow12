using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnAndPortalHandler : MonoBehaviour
{
    [Header("Portal Settings")]
    public GameObject portalPrefab;
    public Transform portalSpawnPoint;
    public string sceneDestino = "Cena cidade"; // Altere aqui para o nome correto da cena
    [Tooltip("Delay (segundos) entre o clone do player ser detectado e o spawn do portal.")]
    public float delayAfterCloneFound = 0f;

    [Header("Player detection")]
    [Tooltip("Tag do GameObject do player (padronize com a tag do seu player).")]
    public string playerTag = "Player";
    [Tooltip("Se verdadeiro, detecta apenas GameObjects com '(Clone)' no nome.")]
    public bool requireNameContainsClone = true;
    [Tooltip("Intervalo (segundos) entre checagens para o clone do player.")]
    public float pollFrequency = 0.15f;
    [Tooltip("Tempo máximo (segundos) para aguardar o clone antes de parar (0 = aguardar indefinidamente).")]
    public float maxWaitTime = 0f;

    private GameObject spawnedPortal;
    private Coroutine waitCoroutine;

    void Start()
    {
        // Começa a rotina que espera o clone do player e só então instancia o portal.
        waitCoroutine = StartCoroutine(WaitForPlayerCloneAndSpawnPortal());
    }

    // Rotina que fica checando se o clone do player já existe na cena.
    IEnumerator WaitForPlayerCloneAndSpawnPortal()
    {
        float startTime = Time.time;

        while (true)
        {
            // Se maxWaitTime > 0 e estourou, aborta a espera
            if (maxWaitTime > 0f && Time.time - startTime > maxWaitTime)
            {
                Debug.LogWarning($"PlayerSpawnAndPortalHandler: tempo máximo ({maxWaitTime}s) de espera pelo clone do player excedido. Portal não será spawnado automaticamente.");
                yield break;
            }

            GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
            bool found = false;

            foreach (var p in players)
            {
                if (p == null) continue;

                if (requireNameContainsClone)
                {
                    if (p.name.Contains("(Clone)"))
                    {
                        found = true;
                        break;
                    }
                }
                else
                {
                    // aceita qualquer com a tag
                    found = true;
                    break;
                }
            }

            if (found)
            {
                // aguarda o delay configurado (se houver) e spawna o portal
                if (delayAfterCloneFound > 0f)
                    yield return new WaitForSeconds(delayAfterCloneFound);

                SpawnPortal();
                yield break;
            }

            yield return new WaitForSeconds(pollFrequency);
        }
    }

    // Método público para permitir um fluxo dirigido por evento: se outro script souber quando o clone foi criado,
    // pode chamar esse método para forçar o spawn do portal.
    public void OnPlayerCloneSpawned()
    {
        // cancela a rotina de polling se estiver rodando
        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }

        // inicia spawn com delay se necessário
        StartCoroutine(SpawnPortalAfterDelay());
    }

    IEnumerator SpawnPortalAfterDelay()
    {
        if (delayAfterCloneFound > 0f)
            yield return new WaitForSeconds(delayAfterCloneFound);

        SpawnPortal();
    }

    void SpawnPortal()
    {
        if (portalPrefab == null || portalSpawnPoint == null)
        {
            Debug.LogWarning("PlayerSpawnAndPortalHandler: PortalPrefab ou PortalSpawnPoint não configurado(s)!");
            return;
        }

        if (spawnedPortal != null)
        {
            Debug.Log("PlayerSpawnAndPortalHandler: portal já foi spawnado anteriormente.");
            return;
        }

        spawnedPortal = Instantiate(portalPrefab, portalSpawnPoint.position, Quaternion.identity, null);
        Debug.Log("PlayerSpawnAndPortalHandler: portal spawnado.");

        // Se o prefab já tiver PortalTrigger, apenas atualiza o nome da cena.
        PortalTrigger trigger = spawnedPortal.GetComponent<PortalTrigger>();
        if (trigger == null)
        {
            // adiciona o script caso não exista
            trigger = spawnedPortal.AddComponent<PortalTrigger>();
        }
        trigger.nomeDaCenaDestino = sceneDestino;
    }

    // (Opcional) para remoção do portal programaticamente
    public void DestroyPortal()
    {
        if (spawnedPortal != null)
        {
            Destroy(spawnedPortal);
            spawnedPortal = null;
            Debug.Log("PlayerSpawnAndPortalHandler: portal destruído.");
        }
    }
}

// Script auxiliar interno para teleporte (mantive praticamente igual ao seu original)
public class PortalTrigger : MonoBehaviour
{
    public string nomeDaCenaDestino;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (string.IsNullOrEmpty(nomeDaCenaDestino))
            {
                Debug.LogWarning("PortalTrigger: nomeDaCenaDestino não está configurado!");
                return;
            }

            SceneManager.LoadScene(nomeDaCenaDestino);
        }
    }
}