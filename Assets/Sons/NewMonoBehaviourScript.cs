using UnityEngine;

public class HeroKnightSound : MonoBehaviour
{
    [Header("Audios")]
    public AudioClip somPasso;
    public AudioClip somPulo;
    public AudioClip somAtaque;

    [Header("Configurações")]
    public float intervaloPasso = 0.35f;
    private float timerPasso;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
    }

    void Update()
    {
        // --- Som de passos ---
        bool andando = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)
                       || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);

        if (andando)
        {
            timerPasso -= Time.deltaTime;
            if (timerPasso <= 0)
            {
                TocarSom(somPasso);
                timerPasso = intervaloPasso;
            }
        }
        else
        {
            timerPasso = 0;
        }

        // --- Som de pulo ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TocarSom(somPulo);
        }

        // --- Som de ataque ---
        if (Input.GetMouseButtonDown(0)) // M1
        {
            TocarSom(somAtaque);
        }
    }

    void TocarSom(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
