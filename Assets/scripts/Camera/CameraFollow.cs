using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Cameraseguir : MonoBehaviour
{
    [Header("Detecção")]
    public string playerTag = "Player";
    public float checkInterval = 0.18f;

    [Header("Seguir")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10f);
    public float smoothTime = 0.12f;

    // CORREÇÃO: velocity é Vector3 e está no escopo da classe
    private Vector3 velocity = Vector3.zero;

    private Coroutine detectCoroutine;
    private HashSet<int> knownPlayerIDs = new HashSet<int>();

    void OnEnable()
    {
        detectCoroutine = StartCoroutine(DetectRoutine());
    }

    void OnDisable()
    {
        if (detectCoroutine != null) StopCoroutine(detectCoroutine);
    }

    void Start()
    {
        if (target != null)
            RememberPlayers();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }

    IEnumerator DetectRoutine()
    {
        while (true)
        {
            DetectAndAssignTargetIfNeeded();
            yield return new WaitForSeconds(checkInterval);
        }
    }

    void DetectAndAssignTargetIfNeeded()
    {
        if (target != null)
        {
            if (target.gameObject.activeInHierarchy) return;
            target = null;
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);

        HashSet<int> currentIDs = new HashSet<int>();
        foreach (var p in players)
        {
            if (p == null) continue;
            currentIDs.Add(p.GetInstanceID());
        }

        if (players.Length > 0)
        {
            Transform chosen = null;

            foreach (var p in players)
            {
                if (p == null || !p.activeInHierarchy) continue;
                if (HasEnabledPlayerScript(p))
                {
                    chosen = p.transform;
                    break;
                }
            }

            if (chosen == null)
            {
                foreach (var p in players)
                {
                    if (p == null) continue;
                    if (p.activeInHierarchy)
                    {
                        chosen = p.transform;
                        break;
                    }
                }
            }

            if (chosen == null && players.Length > 0)
                chosen = players[0].transform;

            if (chosen != null && chosen != target)
            {
                SetTarget(chosen, snapToTarget: false);
            }
        }

        knownPlayerIDs = currentIDs;
    }

    bool HasEnabledPlayerScript(GameObject go)
    {
        var monos = go.GetComponents<MonoBehaviour>();
        foreach (var m in monos)
        {
            if (m == null) continue;
            if (m == this) continue;
            if (m.enabled) return true;
        }

        if (go.GetComponent<Rigidbody2D>() != null) return true;

        return false;
    }

    public void SetTarget(Transform newTarget, bool snapToTarget = false)
    {
        if (newTarget == null) return;

        target = newTarget;

        // opcional: manter a distância atual
        offset = transform.position - target.position;

        knownPlayerIDs.Add(newTarget.GetInstanceID());

        if (snapToTarget)
        {
            transform.position = target.position + offset;
            // zera velocity (Vector3)
            velocity = Vector3.zero;
        }
    }

    public void NotifySpawned(Transform spawnedPlayer, bool snapToTarget = false)
    {
        SetTarget(spawnedPlayer, snapToTarget);
    }

    void RememberPlayers()
    {
        var players = GameObject.FindGameObjectsWithTag(playerTag);
        knownPlayerIDs.Clear();
        foreach (var p in players) if (p != null) knownPlayerIDs.Add(p.GetInstanceID());
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(target.position, 0.3f);
        }
    }
#endif
}