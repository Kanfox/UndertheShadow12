using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnAndPortalHandler : MonoBehaviour
{
    [Header("Portal Settings")]
    public GameObject portalPrefab;
    public Transform portalSpawnPoint;
    public string sceneDestino = "Cena cidade";
    public float delayAfterCloneFound = 0f;

    [Header("Player detection")]
    public string playerTag = "Player";
    public bool requireNameContainsClone = true;
    public float pollFrequency = 0.15f;
    public float maxWaitTime = 0f;

    private GameObject spawnedPortal;
    private Coroutine waitCoroutine;

    // -----------------------------
    // 🔊 NOVO: Som de spawn do portal
    // -----------------------------
    [Header("Portal Sound")]
    public AudioClip portalSpawnSound;
    private AudioSource audioSource;
    // -----------------------------

    void Start()
    {
        // 🔊 NOVO: criar ou pegar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        // 🔊 NOVO FIM

        waitCoroutine = StartCoroutine(WaitForPlayerCloneAndSpawnPortal());
    }

    IEnumerator WaitForPlayerCloneAndSpawnPortal()
    {
        float startTime = Time.time;

        while (true)
        {
            if (maxWaitTime > 0f && Time.time - startTime > maxWaitTime)
            {
                Debug.LogWarning($"PlayerSpawnAndPortalHandler: tempo máximo ({maxWaitTime}s) de espera...");
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
                    found = true;
                    break;
                }
            }

            if (found)
            {
                if (delayAfterCloneFound > 0f)
                    yield return new WaitForSeconds(delayAfterCloneFound);

                SpawnPortal();
                yield break;
            }

            yield return new WaitForSeconds(pollFrequency);
        }
    }

    public void OnPlayerCloneSpawned()
    {
        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }

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
            Debug.LogWarning("PortalPrefab ou PortalSpawnPoint não configurado(s)!");
            return;
        }

        if (spawnedPortal != null)
        {
            Debug.Log("Portal já foi spawnado anteriormente.");
            return;
        }

        spawnedPortal = Instantiate(portalPrefab, portalSpawnPoint.position, Quaternion.identity, null);
        Debug.Log("Portal spawnado!");

        // -----------------------------
        // 🔊 NOVO: tocar som ao spawnar
        // -----------------------------
        if (portalSpawnSound != null)
        {
            audioSource.PlayOneShot(portalSpawnSound);
        }
        // -----------------------------

        PortalTrigger trigger = spawnedPortal.GetComponent<PortalTrigger>();
        if (trigger == null)
        {
            trigger = spawnedPortal.AddComponent<PortalTrigger>();
        }
        trigger.nomeDaCenaDestino = sceneDestino;
    }

    public void DestroyPortal()
    {
        if (spawnedPortal != null)
        {
            Destroy(spawnedPortal);
            spawnedPortal = null;
            Debug.Log("Portal destruído.");
        }
    }
}

public class PortalTrigger : MonoBehaviour
{
    public string nomeDaCenaDestino;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (string.IsNullOrEmpty(nomeDaCenaDestino))
            {
                Debug.LogWarning("PortalTrigger: nome da cena não configurado!");
                return;
            }

            SceneManager.LoadScene(nomeDaCenaDestino);
        }
    }
}
