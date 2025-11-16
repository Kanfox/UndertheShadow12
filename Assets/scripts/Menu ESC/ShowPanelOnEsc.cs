using UnityEngine;

public class ShowPanelOnEscOrM : MonoBehaviour
{
    public GameObject panel; // Arraste seu painel aqui pelo Inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M))
        {
            panel.SetActive(!panel.activeSelf); // Ativa/desativa o painel
        }
    }
}