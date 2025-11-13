using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;           // Personagem a ser seguido
    public float offsetX = 0f;         // Deslocamento horizontal
    public float offsetZ = -10f;       // Profundidade fixa (padrão 2D)
    public float smoothSpeed = 0.125f; // Suavização da câmera

    public float minX = 0f;            // Limite mínimo no eixo X
    public float maxX = 50f;           // Limite máximo no eixo X

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (player == null) return;

        // Posição alvo apenas no eixo X
        Vector3 targetPosition = new Vector3(
            Mathf.Clamp(player.position.x + offsetX, minX, maxX), // restringe movimento
            transform.position.y, // mantém Y fixo
            offsetZ
        );

        // Movimento suave da câmera
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothSpeed
        );
    }
}