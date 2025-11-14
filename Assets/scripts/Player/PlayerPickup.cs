using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRadius = 2f;
    public string itemTag = "Item";
    public LayerMask itemLayer;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider2D[] itensNaArea = Physics2D.OverlapCircleAll(transform.position, pickupRadius, itemLayer);
            Debug.Log("Detectados " + itensNaArea.Length + " itens na área!");

            foreach (Collider2D item in itensNaArea)
            {
                Debug.Log("Nome detectado: " + item.name + " | Tag: " + item.tag + " | Layer:" + LayerMask.LayerToName(item.gameObject.layer));

                if (item.CompareTag(itemTag))
                {
                    Destroy(item.gameObject);
                    Debug.Log("Item coletado: " + item.name);
                }
            }
        }
    }
}