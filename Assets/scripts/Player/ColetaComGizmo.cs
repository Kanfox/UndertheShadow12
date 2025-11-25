using UnityEngine;
using TMPro;
using System.Collections;

public class ColetaComGizmo : MonoBehaviour
{
    [Header("Configuração do Gizmo (Player)")]
    public float coletaRange = 2f;
    public Vector3 offsetGizmo = Vector3.zero;
    public string tagParaColetar = "Coletavel";
    public KeyCode teclaColeta = KeyCode.E;

    [Header("Configuração do Texto (TextMeshPro)")]
    public TMP_Text textoColetaUI; // arraste o TMP Text já presente no Canvas!
    public string mensagemColeta = "Objeto coletado!";
    public float tempoVisivel = 2f;

    void OnDrawGizmosSelected()
    {
        if (CompareTag("Player"))
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position + offsetGizmo, coletaRange);
        }
    }

    void Update()
    {
        if (CompareTag("Player") && Input.GetKeyDown(teclaColeta))
        {
            GameObject[] coletaveis = GameObject.FindGameObjectsWithTag(tagParaColetar);

            bool coletouAlgo = false;
            foreach (GameObject obj in coletaveis)
            {
                Vector3 centroGizmo = transform.position + offsetGizmo;
                float distancia = Vector3.Distance(obj.transform.position, centroGizmo);

                if (distancia <= coletaRange)
                {
                    Destroy(obj);
                    coletouAlgo = true;
                }
            }
            if (coletouAlgo)
            {
                MostraTextoColetadoUI();
            }
        }
    }

    void MostraTextoColetadoUI()
    {
        if (textoColetaUI != null)
        {
            textoColetaUI.text = mensagemColeta;
            textoColetaUI.gameObject.SetActive(true);
            StartCoroutine(EscondeTextoColeta());
        }
    }

    IEnumerator EscondeTextoColeta()
    {
        yield return new WaitForSeconds(tempoVisivel);
        if (textoColetaUI != null)
        {
            textoColetaUI.gameObject.SetActive(false);
        }
    }
}