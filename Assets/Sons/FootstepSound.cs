using UnityEngine;

public class SomDePassos : MonoBehaviour
{
    public AudioSource somPassos; // arraste o AudioSource aqui no Inspector

    void Update()
    {
        bool apertando = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);

        if (apertando && !somPassos.isPlaying)
        {
            somPassos.Play();
        }
        else if (!apertando && somPassos.isPlaying)
        {
            somPassos.Stop();
        }
    }
}
