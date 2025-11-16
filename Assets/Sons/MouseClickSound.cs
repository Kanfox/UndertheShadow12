using UnityEngine;

public class MouseClickSound : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip soundM1;
    public AudioClip soundM2;
    public AudioClip soundA;
    public AudioClip soundD;
    public AudioClip soundLeft;
    public AudioClip soundRight;

    // Cooldown individual
    public float cdM1 = 1f;
    public float cdM2 = 0.5f;
    public float cdA = 0.7f;
    public float cdD = 0.7f;
    public float cdLeft = 0.7f;
    public float cdRight = 0.7f;

    private float lastM1;
    private float lastM2;
    private float lastA;
    private float lastD;
    private float lastLeft;
    private float lastRight;

    private bool isPlaying = false;

    void Update()
    {
        if (isPlaying) return;

        if (Input.GetMouseButton(0) && Time.time - lastM1 >= cdM1)
            Play(soundM1, ref lastM1, cdM1);

        else if (Input.GetMouseButton(1) && Time.time - lastM2 >= cdM2)
            Play(soundM2, ref lastM2, cdM2);

        else if (Input.GetKey(KeyCode.A) && Time.time - lastA >= cdA)
            Play(soundA, ref lastA, cdA);

        else if (Input.GetKey(KeyCode.D) && Time.time - lastD >= cdD)
            Play(soundD, ref lastD, cdD);

        else if (Input.GetKey(KeyCode.LeftArrow) && Time.time - lastLeft >= cdLeft)
            Play(soundLeft, ref lastLeft, cdLeft);

        else if (Input.GetKey(KeyCode.RightArrow) && Time.time - lastRight >= cdRight)
            Play(soundRight, ref lastRight, cdRight);
    }

    void Play(AudioClip clip, ref float lastTime, float cooldown)
    {
        if (clip == null) return;

        audioSource.PlayOneShot(clip);
        lastTime = Time.time;

        isPlaying = true;
        Invoke(nameof(ResetPlaying), clip.length); // trava até o som acabar
    }

    void ResetPlaying()
    {
        isPlaying = false;
    }
}
