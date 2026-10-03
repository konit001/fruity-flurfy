using UnityEngine;

public class PlayerMagnet : MonoBehaviour
{
    [SerializeField] private float magnetRadius = 3f;
    [SerializeField] private float magnetSpeed = 6f;
    [SerializeField] private float pickupDistance = 0.5f;

    void Update()
    {
        float Radius = magnetRadius * magnetRadius;

        foreach (var item in DroppedItem.All)
        {
            if ((item.transform.position - transform.position).sqrMagnitude <= Radius)
                item.Attract(transform, magnetSpeed, pickupDistance);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}
