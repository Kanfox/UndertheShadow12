using UnityEngine;

public class ScrollCameraCredits : MonoBehaviour
{
    public float speed = 0.5f;  // velocidade de descida
    public float endY = -10f;   // posição final da câmera no Y

    void Update()
    {
        // Move apenas a câmera para baixo
        if (transform.position.y > endY)
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
        }
    }
}
