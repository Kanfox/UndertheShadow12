using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider volumeSlider;  // Arraste o Slider aqui no inspetor
    public AudioSource audioSource; // Arraste o AudioSource que quer controlar

    void Start()
    {
        // Define o valor inicial do slider para o volume atual
        if (audioSource != null)
            volumeSlider.value = audioSource.volume;

        // Adiciona o evento para chamar a função quando o slider for alterado
        volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    // Essa função é chamada sempre que o valor do slider mudar
    void OnSliderValueChanged(float value)
    {
        if (audioSource != null)
            audioSource.volume = value;
    }
}
