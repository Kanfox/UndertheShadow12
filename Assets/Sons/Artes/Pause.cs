using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu; // arraste o painel do menu aqui
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;  // pausa o jogo
        isPaused = true;

        // 🔇 Pausa todos os sons do jogo
        AudioListener.pause = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;  // volta ao normal
        isPaused = false;

        // 🔊 Volta os sons do jogo
        AudioListener.pause = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // 🔥 Reiniciar cena
    public void RestartGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false; // volta o som antes de recarregar
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
