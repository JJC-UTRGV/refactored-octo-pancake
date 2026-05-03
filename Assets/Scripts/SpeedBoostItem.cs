using UnityEngine;

public class SpeedBoostItem : MonoBehaviour
{
    [Header("Boost Settings")]
    [SerializeField] private float boostSpeed = 10f;
    [SerializeField] private float maxSpeed = 14f;

    [Header("Direction Settings")]
    [SerializeField] private Transform directionSource;

    [Header("Item Settings")]
    [SerializeField] private bool destroyAfterUse = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball"))
            return;

        Rigidbody rb = other.attachedRigidbody;

        if (rb == null)
            return;
        
        Transform source = directionSource != null ? directionSource : transform;

        Vector3 boostDirection = source.up;
        
        boostDirection.y = 0f;
        boostDirection.Normalize();
        
        Vector3 newVelocity = boostDirection * boostSpeed;
        
        newVelocity.y = rb.linearVelocity.y;

        if (newVelocity.magnitude > maxSpeed)
        {
            newVelocity = newVelocity.normalized * maxSpeed;
        }

        rb.linearVelocity = newVelocity;

        if (destroyAfterUse)
        {
            Destroy(gameObject);
        }
    }
}