using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    [Header("Hole")]
    public Transform holeCenter;

    [Header("Score")]
    public string holeName = "Hole1";

    [Header("Transition")]
    [SerializeField] private bool isFinalHole = false;
    [SerializeField] private string nextSceneName = "Level2";
    [SerializeField] private LevelTransitionOverlay transitionOverlay;

    private bool completed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (completed) return;
        if (!other.CompareTag("Ball")) return;

        completed = true;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        other.transform.position = holeCenter.position;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SaveHole(holeName);
        }

        Debug.Log("Hole Complete!");

        if (isFinalHole && transitionOverlay != null)
        {
            transitionOverlay.PlayAndLoad("Hole Complete!", nextSceneName);
        }
    }
}