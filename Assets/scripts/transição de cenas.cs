using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup; // Referência ao Canvas Group
    public float fadeDuration = 1f;     // Duração do fade

    private void Start()
    {
        // Quando a cena começar, faz o fade in (da tela preta para visível)
        StartCoroutine(FadeIn());
    }

    // Método público para trocar de cena com fade
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(FadeAndSwitchScenes(sceneName));
    }

    // Coroutine para fazer o fade in
    private IEnumerator FadeIn()
    {
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = 1 - (time / fadeDuration); // de 1 para 0
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false; // libera cliques
    }

    // Coroutine para fazer fade out e trocar de cena
    private IEnumerator FadeAndSwitchScenes(string sceneName)
    {
        fadeCanvasGroup.blocksRaycasts = true; // bloqueia cliques
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = time / fadeDuration; // de 0 para 1
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;

        // Carrega nova cena
        SceneManager.LoadScene(sceneName);
    }
}