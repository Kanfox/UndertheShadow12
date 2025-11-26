using UnityEngine;

/// <summary>
/// Ativa/Desativa GameObjects de texto (p.ex. TextMeshProUGUI) quando o mouse passa sobre objetos com tags mapeadas.
/// - Para 2D usa Physics2D.OverlapPoint (mais confiável que Raycast com direção zero).
/// - Para 3D usa Physics.Raycast.
/// - Os textObj podem começar desativados na cena; o script os ativa/desativa.
/// </summary>
public class MouseHoverActivateText : MonoBehaviour
{
    [System.Serializable]
    public struct TagToTextObject
    {
        public string tag;         // tag do objeto alvo
        public GameObject textObj; // GameObject do texto (pode estar desativado)
    }

    [Tooltip("Mapeamento Tag -> GameObject de texto")]
    public TagToTextObject[] mappings = new TagToTextObject[3];

    [Tooltip("Usar raycast 2D (Physics2D) ou 3D (Physics)")]
    public bool use2D = true;

    [Tooltip("Câmera usada para raycast (se nulo usa Camera.main)")]
    public Camera raycastCamera;

    [Tooltip("Se true exibe logs no Console para depuração")]
    public bool debugLogs = false;

    private GameObject currentActiveText = null;
    private string currentHoveredTag = null;

    void Start()
    {
        // garante que todos os textos comecem desativados
        foreach (var m in mappings)
        {
            if (m.textObj != null)
                m.textObj.SetActive(false);
        }
        if (raycastCamera == null)
            raycastCamera = Camera.main;
    }

    void Update()
    {
        GameObject hitObj = GetObjectUnderMouse();
        string hitTag = hitObj != null ? hitObj.tag : null;

        if (debugLogs)
        {
            if (hitObj != null) Debug.Log("[MouseHover] Hit: " + hitObj.name + " tag=" + hitObj.tag);
            else Debug.Log("[MouseHover] Hit: null");
        }

        // Se mudou a tag/objeto sob o mouse, atualiza textos
        if (hitTag != currentHoveredTag)
        {
            // desativa texto anterior
            if (currentActiveText != null)
            {
                currentActiveText.SetActive(false);
                currentActiveText = null;
            }
            currentHoveredTag = hitTag;

            // ativa novo texto se houver mapeamento para a tag detectada
            if (!string.IsNullOrEmpty(currentHoveredTag))
            {
                foreach (var m in mappings)
                {
                    if (!string.IsNullOrEmpty(m.tag) && m.tag == currentHoveredTag && m.textObj != null)
                    {
                        m.textObj.SetActive(true);
                        currentActiveText = m.textObj;
                        break;
                    }
                }
            }
        }
    }

    private GameObject GetObjectUnderMouse()
    {
        if (raycastCamera == null)
        {
            if (debugLogs) Debug.LogWarning("[MouseHover] raycastCamera é null e Camera.main também é null.");
            return null;
        }

        if (use2D)
        {
            // converte para world point; OverlapPoint usa apenas x,y
            Vector3 wp = raycastCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 point2D = new Vector2(wp.x, wp.y);

            // OverlapPoint detecta Collider2D que contém o ponto
            Collider2D col = Physics2D.OverlapPoint(point2D);
            if (col != null)
                return col.gameObject;
            return null;
        }
        else
        {
            Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                return hit.collider.gameObject;
            return null;
        }
    }

    // opcional: desativa todos os textos manualmente
    public void HideAllTexts()
    {
        foreach (var m in mappings)
            if (m.textObj != null)
                m.textObj.SetActive(false);
        currentActiveText = null;
        currentHoveredTag = null;
    }
}