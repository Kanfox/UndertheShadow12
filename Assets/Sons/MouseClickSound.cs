using UnityEngine;

public class MouseClickSounds : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip soundM1;
    public AudioClip soundM2;

    public AudioClip soundA;
    public AudioClip soundD;

    public AudioClip soundLeft;
    public AudioClip soundRight;

    // Cooldowns individuais
    public float cdM1 = 0.2f;
    public float cdM2 = 0.2f;
    public float cdA = 0.7f;
    public float cdD = 0.7f;
    public float cdLeft = 0.7f;
    public float cdRight = 0.7f;

    // 🔒 Cooldown global bloqueando tudo
    public float globalBlockTime = 1f;
    private float unblockTime = 0f;

    private float lastM1;
    private float lastM2;
    private float lastA;
    private float lastD;
    private float lastLeft;
    private float lastRight;

    void Update()
    {
        bool blocked = Time.time < unblockTime;  // está dentro do bloqueio?

        // 🖱️ M1
        if (Input.GetMouseButtonDown(0))
        {
            if (!blocked && Time.time - lastM1 >= cdM1)
            {
                Play(soundM1);
                lastM1 = Time.time;
                unblockTime = Time.time + globalBlockTime; // inicia bloqueio
            }
            return; // impede que outras teclas executem no mesmo frame
        }

        // 🖱️ M2
        if (Input.GetMouseButtonDown(1))
        {
            if (!blocked && Time.time - lastM2 >= cdM2)
            {
                Play(soundM2);
                lastM2 = Time.time;
                unblockTime = Time.time + globalBlockTime; // inicia bloqueio
            }
            return;
        }

        // ❌ se estiver bloqueado, nenhum som abaixo toca
        if (blocked) return;

        // ⌨️ tecla A
        if (Input.GetKeyDown(KeyCode.A) && Time.time - lastA >= cdA)
        {
            Play(soundA);
            lastA = Time.time;
        }

        // ⌨️ tecla D
        else if (Input.GetKeyDown(KeyCode.D) && Time.time - lastD >= cdD)
        {
            Play(soundD);
            lastD = Time.time;
        }

        // ⬅️ seta esquerda
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && Time.time - lastLeft >= cdLeft)
        {
            Play(soundLeft);
            lastLeft = Time.time;
        }

        // ➡️ seta direita
        else if (Input.GetKeyDown(KeyCode.RightArrow) && Time.time - lastRight >= cdRight)
        {
            Play(soundRight);
            lastRight = Time.time;
        }
    }

    void Play(AudioClip clip)
    {
        if (clip == null || audioSource == null)
            return;

        audioSource.PlayOneShot(clip);
    }
}
