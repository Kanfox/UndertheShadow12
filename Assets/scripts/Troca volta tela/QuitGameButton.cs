using UnityEngine;

/// <summary>
/// Coloque este componente em um GameObject (ou no próprio botão).
/// Depois, no componente Button (UI), adicione o GameObject e selecione
/// QuitGameButton.QuitApplication() no evento OnClick().
/// 
/// Em build chama Application.Quit(); no editor para testes fecha o Play Mode.
/// </summary>
public class QuitGameButton : MonoBehaviour
{
    /// <summary>
    /// Chame este método no OnClick do botão.
    /// </summary>
    public void QuitApplication()
    {
        // Se estamos no editor, para o Play Mode (útil para teste).
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Em build fecha o aplicativo.
        Application.Quit();
#endif
    }
}