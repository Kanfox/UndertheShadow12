using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Tooltip("coloca o nome da cena que tu quer onde esteja o portal aqui noah")]
    public string cenaDestino;
   
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou no portal é o jogador
        if (other.CompareTag("Player"))
        {
            // Carrega a cena de destino
            SceneManager.LoadScene(cenaDestino);
        }
    }
}
