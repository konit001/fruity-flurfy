using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public static PlayerInteract Instance { get; private set; }

    [Header("Detect Radius")]
    public Transform detectCenter;
    public float radius;
    public LayerMask targetLayer;
    public bool isDetect;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (userInput.instance != null && userInput.instance.interact)
        {
            CheckPointObject();
        }
    }

    private bool TryGetCloser<T>(out T result) where T : Component
    {
        result = null;
        if (detectCenter == null) return false;
        float closestSqrDistance = float.MaxValue;

        Collider[] hitColliders = Physics.OverlapSphere(detectCenter.position, radius, targetLayer);
        foreach (Collider hitCollider in hitColliders)
        {
            if (!hitCollider.TryGetComponent(out T component))
                continue;

            float distance = (component.transform.position - detectCenter.position).sqrMagnitude;
            if (distance < closestSqrDistance)
            {
                closestSqrDistance = distance;
                result = component;
            }
        }

        return result != null;
    }
    public void CheckPointObject()
    {
        if (TryGetCloser(out CheckPoint checkPoint))
            checkPoint.save();
    }

    public bool IsDetect()
    {
        if (detectCenter == null) return isDetect = false;
        return isDetect = Physics.CheckSphere(detectCenter.position, radius, targetLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (detectCenter == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectCenter.position, radius);
    }
}
