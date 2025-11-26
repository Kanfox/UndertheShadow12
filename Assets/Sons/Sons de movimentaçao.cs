using UnityEngine;

public class SomMovimentacao : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip SoundA;
    public AudioClip SoundD;
    public AudioClip SoundLeft;
    public AudioClip SoundRight;
    public AudioClip SoundSpace;
    public AudioClip SoundF;

    public AudioClip SoundM1;
    public AudioClip SoundM2;

    public float intervaloA = 0.7f;
    public float intervaloD = 0.7f;
    public float intervaloLeft = 0.7f;
    public float intervaloRight = 0.7f;
    public float intervaloSpace = 0.3f;
    public float intervaloF = 0.4f;

    public float intervaloM1 = 0.2f;
    public float intervaloM2 = 0.2f;

    public float intervaloBloqueio = 1f;

    private float ultimoA = 0f;
    private float ultimoD = 0f;
    private float ultimoLeft = 0f;
    private float ultimoRight = 0f;
    private float ultimoSpace = 0f;
    private float ultimoF = 0f;

    private float ultimoM1 = 0f;
    private float ultimoM2 = 0f;

    private float cooldownM1 = 0f;
    private float cooldownM2 = 0f;

    private float bloqueioPassos = 0f;

    private KeyCode teclaAtiva = KeyCode.None;

    // 🔴 NOVO: trava todos os sons quando o player morre
    public bool playerMorto = false;

    void Update()
    {
        // 🔒 BLOQUEIA TODOS OS SONS SE O PLAYER MORREU
        if (playerMorto) return;

        // Atualiza cooldown cruzado
        if (cooldownM1 > 0f) cooldownM1 -= Time.deltaTime;
        if (cooldownM2 > 0f) cooldownM2 -= Time.deltaTime;

        // Libera tecla ativa quando soltar
        if (teclaAtiva != KeyCode.None && Input.GetKeyUp(teclaAtiva))
            teclaAtiva = KeyCode.None;

        // -------- F (bloqueia tudo) ----------
        if (Input.GetKey(KeyCode.F))
        {
            teclaAtiva = KeyCode.F;

            if (Time.time - ultimoF >= intervaloF)
            {
                audioSource.PlayOneShot(SoundF);
                ultimoF = Time.time;
            }

            return;
        }

        // ---------- M1 ----------
        if (Input.GetKeyDown(KeyCode.Mouse0)
            && cooldownM2 <= 0f
            && Time.time - ultimoM1 >= intervaloM1)
        {
            audioSource.PlayOneShot(SoundM1);
            ultimoM1 = Time.time;

            cooldownM1 = intervaloM1;
            cooldownM2 = intervaloM1;

            return;
        }

        // ---------- M2 ----------
        if (Input.GetKeyDown(KeyCode.Mouse1)
            && cooldownM1 <= 0f
            && Time.time - ultimoM2 >= intervaloM2)
        {
            audioSource.PlayOneShot(SoundM2);
            ultimoM2 = Time.time;

            cooldownM2 = intervaloM2;
            cooldownM1 = intervaloM2;

            return;
        }

        // ---------- SPACE / W ----------
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
            && Time.time - ultimoSpace >= intervaloSpace)
        {
            audioSource.PlayOneShot(SoundSpace);
            ultimoSpace = Time.time;

            bloqueioPassos = intervaloBloqueio;
            teclaAtiva = KeyCode.None;

            return;
        }

        // Bloqueio pós espaço
        if (bloqueioPassos > 0f)
        {
            bloqueioPassos -= Time.deltaTime;
            return;
        }

        // -------- PASSOS --------
        if (teclaAtiva != KeyCode.None)
        {
            TocarSom(teclaAtiva);
            return;
        }

        if (Input.GetKey(KeyCode.A)) teclaAtiva = KeyCode.A;
        else if (Input.GetKey(KeyCode.D)) teclaAtiva = KeyCode.D;
        else if (Input.GetKey(KeyCode.LeftArrow)) teclaAtiva = KeyCode.LeftArrow;
        else if (Input.GetKey(KeyCode.RightArrow)) teclaAtiva = KeyCode.RightArrow;

        if (teclaAtiva != KeyCode.None)
            TocarSom(teclaAtiva);
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
