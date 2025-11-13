using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioSource clickSound;
    public Button button;

    void Start()
    {
        // Garante que o som toque quando o botão for clicado
        button.onClick.AddListener(PlaySound);
    }

    void PlaySound()
    {
        clickSound.Play();
    }
}