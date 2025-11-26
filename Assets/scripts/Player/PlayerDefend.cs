using UnityEngine;

public class PlayerDefend : MonoBehaviour
{
    [Header("Animator / Defesa")]
    public Animator animator;
    public string defendAnimatorBoolName = "Defending";
    public string defendStateName = "Defend"; // coloque o nome exato do State no Animator
    public int defendLayerIndex = 0;
    public float crossfadeDuration = 0f;

    [Header("Tempos")]
    public float quickDefendDuration = 1f;

    [Header("Scripts a desativar (nome da classe)")]
    public string attackComponentClassName = "PlayerAttack";
    public string controllerComponentClassName = "PlayerController";

    [Header("Rigidbody (opcional)")]
    public Rigidbody2D rb2d;
    public Rigidbody rb3d;

    [Header("Parâmetros de movimento a resetar (ajuste se necessário)")]
    public string[] movementFloatParams = new string[] { "Speed", "Horizontal", "Vertical" };
    public string[] movementBoolParams = new string[] { "isRunning", "IsMoving" };
    public string[] movementIntParams = new string[] { "State" };

    [Header("Debug")]
    public bool debugLogs = false;

    private bool isDefending = false;
    private float defendTimer = 0f;
    private bool lastIsDefending = false;

    private MonoBehaviour attackComponent;
    private MonoBehaviour controllerComponent;
    private bool attackWasEnabled = false;
    private bool controllerWasEnabled = false;
    private bool previousApplyRootMotion = false;

    void Start()
    {
        attackComponent = FindComponentByClassNameAnywhere(attackComponentClassName);
        controllerComponent = FindComponentByClassNameAnywhere(controllerComponentClassName);

        if (attackComponent == null && debugLogs)
            Debug.Log($"[PlayerDefend] Não encontrou componente '{attackComponentClassName}' em {gameObject.name}");
        if (controllerComponent == null && debugLogs)
            Debug.Log($"[PlayerDefend] Não encontrou componente '{controllerComponentClassName}' em {gameObject.name}");

        if (rb2d == null) rb2d = GetComponent<Rigidbody2D>();
        if (rb3d == null) rb3d = GetComponent<Rigidbody>();

        if (animator != null)
            previousApplyRootMotion = animator.applyRootMotion;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            isDefending = true;
            defendTimer = 0f;
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            isDefending = false;

            // --------------- ACRÉSCIMO: Força saída da defesa imediatamente ---------------
            ForceExitDefendAnimationOnKeyUp();
            // ---------------------------------------------------------------------------
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!Input.GetKey(KeyCode.F))
            {
                defendTimer = quickDefendDuration;
                isDefending = true;
            }
        }

        if (defendTimer > 0f)
        {
            defendTimer -= Time.deltaTime;
            if (defendTimer <= 0f)
                isDefending = false;
        }

        if (isDefending && !lastIsDefending)
            EnterDefendState();
        else if (!isDefending && lastIsDefending)
            ExitDefendState();
        lastIsDefending = isDefending;

        if (animator != null && !string.IsNullOrEmpty(defendAnimatorBoolName))
            animator.SetBool(defendAnimatorBoolName, isDefending);
    }

    private void EnterDefendState()
    {
        if (debugLogs) Debug.Log("[PlayerDefend] EnterDefendState");

        if (attackComponent != null) attackWasEnabled = attackComponent.enabled;
        if (controllerComponent != null) controllerWasEnabled = controllerComponent.enabled;

        if (attackComponent != null) attackComponent.enabled = false;
        if (controllerComponent != null) controllerComponent.enabled = false;

        if (rb2d != null) rb2d.linearVelocity = Vector2.zero;
        if (rb3d != null) rb3d.linearVelocity = Vector3.zero;

        if (animator != null)
        {
            previousApplyRootMotion = animator.applyRootMotion;
            animator.applyRootMotion = false;

            foreach (var p in movementFloatParams)
            {
                if (!string.IsNullOrEmpty(p)) animator.SetFloat(p, 0f);
            }
            foreach (var p in movementBoolParams)
            {
                if (!string.IsNullOrEmpty(p)) animator.SetBool(p, false);
            }
            foreach (var p in movementIntParams)
            {
                if (!string.IsNullOrEmpty(p)) animator.SetInteger(p, 0);
            }

            // tenta tocar o state diretamente, mas primeiro checa se o state existe
            bool played = TryPlayDefendStateSafely();
            if (!played)
            {
                // fallback: só usar o bool e confiar nas transições do Animator
                if (debugLogs) Debug.LogWarning($"[PlayerDefend] State '{defendStateName}' não encontrado na layer {defendLayerIndex}. Usando SetBool como fallback. Verifique o nome do state no Animator.");
            }
        }
    }

    private void ExitDefendState()
    {
        if (debugLogs) Debug.Log("[PlayerDefend] ExitDefendState");

        if (attackComponent != null) attackComponent.enabled = attackWasEnabled;
        if (controllerComponent != null) controllerComponent.enabled = controllerWasEnabled;

        if (animator != null)
            animator.applyRootMotion = previousApplyRootMotion;
    }

    // tenta algumas variações do nome e verifica com HasState antes de chamar Play/CrossFade
    private bool TryPlayDefendStateSafely()
    {
        if (animator == null || string.IsNullOrEmpty(defendStateName))
            return false;

        // tenta dois candidatos: o nome direto e "LayerName.StateName"
        string layerName = "";
        try
        {
            layerName = animator.GetLayerName(defendLayerIndex);
        }
        catch { layerName = ""; }

        string[] candidates = new string[] { defendStateName, (string.IsNullOrEmpty(layerName) ? "" : layerName + "." + defendStateName) };

        foreach (var candidate in candidates)
        {
            if (string.IsNullOrEmpty(candidate)) continue;
            int hash = Animator.StringToHash(candidate);
            if (animator.HasState(defendLayerIndex, hash))
            {
                if (crossfadeDuration > 0f)
                    animator.CrossFade(hash, crossfadeDuration, defendLayerIndex);
                else
                    animator.Play(hash, defendLayerIndex, 0f);

                animator.Update(0f); // aplica no mesmo frame
                if (debugLogs) Debug.Log($"[PlayerDefend] Played state '{candidate}' (layer {defendLayerIndex}).");
                return true;
            }
        }

        // se nenhum dos candidatos foi encontrado, tenta também procurar somente pelo nome hash simples
        int simpleHash = Animator.StringToHash(defendStateName);
        if (animator.HasState(defendLayerIndex, simpleHash))
        {
            animator.Play(simpleHash, defendLayerIndex, 0f);
            animator.Update(0f);
            if (debugLogs) Debug.Log($"[PlayerDefend] Played state by simple hash '{defendStateName}'.");
            return true;
        }

        return false;
    }

    private MonoBehaviour FindComponentByClassNameAnywhere(string className)
    {
        if (string.IsNullOrEmpty(className)) return null;
        var monos = GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var mb in monos)
        {
            if (mb == null) continue;
            if (mb.GetType().Name == className)
                return mb;
        }
        return null;
    }

    public bool IsDefending()
    {
        return isDefending;
    }

    // --------------- ACRÉSCIMO: Força o Animator a sair do estado de defesa imediatamente ---------------
    private void ForceExitDefendAnimationOnKeyUp()
    {
        if (animator != null && !string.IsNullOrEmpty(defendAnimatorBoolName))
        {
            animator.SetBool(defendAnimatorBoolName, false); // reforço extra

            // Troque "Idle" pelo nome do state de espera/parado do seu Animator!
            string idleStateName = "Idle";
            int hash = Animator.StringToHash(idleStateName);

            if (animator.HasState(defendLayerIndex, hash))
            {
                animator.Play(hash, defendLayerIndex, 0f);
                animator.Update(0f);
                if (debugLogs) Debug.Log("[PlayerDefend EXTRA] Forçado saída para '" + idleStateName + "' ao soltar F.");
            }
        }
    }
    // ----------------------------------------------------------------------------------------------
}