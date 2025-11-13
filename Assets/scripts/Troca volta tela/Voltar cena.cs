using UnityEngine;
using UnityEngine.SceneManagement;

public class TrocarCena : MonoBehaviour
{
    public string nomeDaCena;

    public void IrParaCena()
    {
        // Salva a cena atual antes de trocar
        PlayerPrefs.SetString("UltimaCena", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        SceneManager.LoadScene(nomeDaCena);
    }
}