using UnityEngine;

public class SomMovimentacao : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip SoundA;
    public AudioClip SoundD;
    public AudioClip SoundLeft;
    public AudioClip SoundRight;
    public AudioClip SoundSpace;

    public float intervaloA = 0.7f;
    public float intervaloD = 0.7f;
    public float intervaloLeft = 0.7f;
    public float intervaloRight = 0.7f;
    public float intervaloSpace = 0.3f;

    public float intervaloBloqueio = 1f; // <--- tempo que o SPACE bloqueia os outros sons

    private float ultimoA = 0f;
    private float ultimoD = 0f;
    private float ultimoLeft = 0f;
    private float ultimoRight = 0f;
    private float ultimoSpace = 0f;

    private float bloqueioPassos = 0f;

    void Update()
    {
        // ----- SOM DO SPACE -----
        if (Input.GetKeyDown(KeyCode.Space) && Time.time - ultimoSpace >= intervaloSpace)
        {
            audioSource.PlayOneShot(SoundSpace);
            ultimoSpace = Time.time;

            // aplica o bloqueio com valor configurado no inspector
            bloqueioPassos = intervaloBloqueio;
        }

        // diminui o tempo do bloqueio
        if (bloqueioPassos > 0f)
        {
            bloqueioPassos -= Time.deltaTime;
            return; // impede qualquer som de movimento
        }

        // --- SOM A ---
        if (Input.GetKey(KeyCode.A))
        {
            if (Time.time - ultimoA >= intervaloA)
            {
                audioSource.PlayOneShot(SoundA);
                ultimoA = Time.time;
            }
        }

        // --- SOM D ---
        if (Input.GetKey(KeyCode.D))
        {
            if (Time.time - ultimoD >= intervaloD)
            {
                audioSource.PlayOneShot(SoundD);
                ultimoD = Time.time;
            }
        }

        // --- SOM LEFT ---
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (Time.time - ultimoLeft >= intervaloLeft)
            {
                audioSource.PlayOneShot(SoundLeft);
                ultimoLeft = Time.time;
            }
        }

        // --- SOM RIGHT ---
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (Time.time - ultimoRight >= intervaloRight)
            {
                audioSource.PlayOneShot(SoundRight);
                ultimoRight = Time.time;
            }
        }
    }
}
