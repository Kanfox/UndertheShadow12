using UnityEngine;

using UnityEngine;

public class MouseClickSound : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip soundM1;   // Botão esquerdo do mouse
    public AudioClip soundM2;   // Botão direito do mouse

    public AudioClip soundA;    // Tecla A
    public AudioClip soundD;    // Tecla D
    public AudioClip soundLeft; // Seta esquerda
    public AudioClip soundRight;// Seta direita

    public float cooldown = 0.7f;
    private float nextTimeToPlay = 0f;

    void Update()
    {
        if (Time.time >= nextTimeToPlay)
        {
            // Mouse
            if (Input.GetMouseButtonDown(0))
            {
                audioSource.PlayOneShot(soundM1);
                nextTimeToPlay = Time.time + cooldown;
            }

            if (Input.GetMouseButtonDown(1))
            {
                audioSource.PlayOneShot(soundM2);
                nextTimeToPlay = Time.time + cooldown;
            }

            // Teclas A e D
            if (Input.GetKeyDown(KeyCode.A))
            {
                audioSource.PlayOneShot(soundA);
                nextTimeToPlay = Time.time + cooldown;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                audioSource.PlayOneShot(soundD);
                nextTimeToPlay = Time.time + cooldown;
            }

            // Seta esquerda e direita
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                audioSource.PlayOneShot(soundLeft);
                nextTimeToPlay = Time.time + cooldown;
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                audioSource.PlayOneShot(soundRight);
                nextTimeToPlay = Time.time + cooldown;
            }
        }
    }
}
