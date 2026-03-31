using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    public Transform holeCenter;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            other.transform.position = holeCenter.position;

            Debug.Log("Completed!");
        }
    }
}