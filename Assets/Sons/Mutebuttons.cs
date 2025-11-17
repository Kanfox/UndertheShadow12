using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    public Button botaoMute;
    public Image imagemDoBotao;

    public Sprite imagemSomLigado;   // imagem quando o som está normal
    public Sprite imagemSomDesligado; // imagem quando está mutado

    private bool estaMutado = false;

    private void Start()
    {
        botaoMute.onClick.AddListener(ApertouBotao);
        AtualizarImagem();
    }

    private void ApertouBotao()
    {
        estaMutado = !estaMutado;

        AudioListener.pause = estaMutado;

        AtualizarImagem();
    }

    private void AtualizarImagem()
    {
        if (estaMutado)
        {
            imagemDoBotao.sprite = imagemSomDesligado;
        }
        else
        {
            imagemDoBotao.sprite = imagemSomLigado;
        }
    }
}
