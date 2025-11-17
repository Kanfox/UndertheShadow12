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

    public float intervaloBloqueio = 1f;

    private float ultimoA = 0f;
    private float ultimoD = 0f;
    private float ultimoLeft = 0f;
    private float ultimoRight = 0f;
    private float ultimoSpace = 0f;

    private float bloqueioPassos = 0f;

    private KeyCode teclaAtiva = KeyCode.None;

    void Update()
    {
        // Libera tecla ativa quando soltar
        if (teclaAtiva != KeyCode.None && Input.GetKeyUp(teclaAtiva))
        {
            teclaAtiva = KeyCode.None;
        }

        // --- SOM DO SPACE ou W (instantâneos, não viram tecla ativa) ---
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
            && Time.time - ultimoSpace >= intervaloSpace)
        {
            audioSource.PlayOneShot(SoundSpace);
            ultimoSpace = Time.time;

            bloqueioPassos = intervaloBloqueio;

            // IMPORTANTE: LIBERA QUALQUER TECLA ATIVA
            teclaAtiva = KeyCode.None;

            return; // finaliza, deixa o bloqueio agir
        }

        // Se está bloqueado, não toca nada
        if (bloqueioPassos > 0f)
        {
            bloqueioPassos -= Time.deltaTime;
            return;
        }

        // Se há uma tecla ativa, só ela toca
        if (teclaAtiva != KeyCode.None)
        {
            TocarSom(teclaAtiva);
            return;
        }

        // Seleção da nova tecla ativa
        if (Input.GetKey(KeyCode.A)) teclaAtiva = KeyCode.A;
        else if (Input.GetKey(KeyCode.D)) teclaAtiva = KeyCode.D;
        else if (Input.GetKey(KeyCode.LeftArrow)) teclaAtiva = KeyCode.LeftArrow;
        else if (Input.GetKey(KeyCode.RightArrow)) teclaAtiva = KeyCode.RightArrow;

        // toca a nova tecla ativa
        if (teclaAtiva != KeyCode.None)
        {
            TocarSom(teclaAtiva);
        }
    }

    void TocarSom(KeyCode key)
    {
        if (key == KeyCode.A && Time.time - ultimoA >= intervaloA)
        {
            audioSource.PlayOneShot(SoundA);
            ultimoA = Time.time;
        }
        else if (key == KeyCode.D && Time.time - ultimoD >= intervaloD)
        {
            audioSource.PlayOneShot(SoundD);
            ultimoD = Time.time;
        }
        else if (key == KeyCode.LeftArrow && Time.time - ultimoLeft >= intervaloLeft)
        {
            audioSource.PlayOneShot(SoundLeft);
            ultimoLeft = Time.time;
        }
        else if (key == KeyCode.RightArrow && Time.time - ultimoRight >= intervaloRight)
        {
            audioSource.PlayOneShot(SoundRight);
            ultimoRight = Time.time;
        }
    }
}
