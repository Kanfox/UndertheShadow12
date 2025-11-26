using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectAndSwitchScene : MonoBehaviour
{
    public float rangeHorizontal = 2f;       // Alcance horizontal do gizmo e detecção
    public float rangeVertical = 1f;         // Alcance vertical do gizmo e detecção
    public string collectibleTag = "Collectible"; // Tag do objeto coletável
    public string nextSceneName = "NextScene";    // Nome da cena para trocar
    private GameObject collectedObject;
    private bool hasCollected = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !hasCollected)
        {
            Vector2 center = transform.position;
            Vector2 size = new Vector2(rangeHorizontal, rangeVertical);

            Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f);

            foreach (var hit in hits)
            {
                if (hit.CompareTag(collectibleTag))
                {
                    collectedObject = hit.gameObject;
                    CollectObject();
                    break;
                }
            }
        }
    }

    void CollectObject()
    {
        hasCollected = true;
        collectedObject.SetActive(false);
        Invoke("SwitchScene", 3f);
    }

    void SwitchScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    // Desenha o gizmo como um box ao redor do player para ajuste
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(rangeHorizontal, rangeVertical, 0.1f));
    }
}