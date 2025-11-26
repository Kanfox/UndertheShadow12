using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ResetProgresso : MonoBehaviour
{
    public float delay = 1f; // Tempo antes de resetar e voltar ao início
    public string cenaInicial = "CenaInicial"; // Nome da cena inicial

    [Header("Som ao perder")]
    public AudioSource somDerrota; // Arraste o áudio aqui

    private bool somTocado = false;

    void Start()
    {
        // Toca o som automaticamente quando a cena de derrota inicia
        TocarSomDerrota();
    }

    public void ResetarJogo()
    {
        StartCoroutine(ResetCoroutine());
    }

    void TocarSomDerrota()
    {
        if (!somTocado && somDerrota != null)
        {
            somDerrota.Play();
            somTocado = true;
        }
    }

    private IEnumerator ResetCoroutine()
    {
        // Espera o delay
        yield return new WaitForSeconds(delay);

        // Apaga todos os dados salvos
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Volta para a cena inicial
        SceneManager.LoadScene(cenaInicial);
    }
}
