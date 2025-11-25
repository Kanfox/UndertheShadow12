using UnityEngine;

public class PlayerDefend : MonoBehaviour
{
    public Animator animator;
    public float quickDefendDuration = 1f;
    private bool isDefending = false;
    private float defendTimer = 0f;

    void Update()
    {
        // Segurar botão F (defesa continua)
        if (Input.GetKey(KeyCode.F))
        {
            isDefending = true;
            defendTimer = 0f; // zera o timer do modo rápido
        }

        // Tirou o dedo do botão F (para defesa contínua)
        if (Input.GetKeyUp(KeyCode.F))
        {
            isDefending = false;
        }

        // Apertou rapidamente F (defesa rápida)
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!Input.GetKey(KeyCode.F))
            {
                defendTimer = quickDefendDuration;
                isDefending = true;
            }
        }

        // Timer da defesa rápida
        if (defendTimer > 0f)
        {
            defendTimer -= Time.deltaTime;
            if (defendTimer <= 0f)
            {
                isDefending = false;
            }
        }

        // Atualiza animação
        animator.SetBool("Defending", isDefending);
    }

    // Exemplo de bloqueio: chame isso quando receber ataque
    public bool IsDefending()
    {
        return isDefending;
    }
}