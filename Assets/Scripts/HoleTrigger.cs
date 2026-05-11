using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    [Header("Hole")]
    public Transform holeCenter;

    [Header("Score")]
    public string holeName = "Hole 1";

    [Header("Transition")]
    [SerializeField] private bool isFinalHole = false;
    [SerializeField] private string nextSceneName = "Hole 2";
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

        int strokesThisHole = 0;

        if (ScoreManager.Instance != null)
        {
            strokesThisHole = ScoreManager.Instance.currentHoleStrokes;

            ScoreManager.Instance.SaveHole(holeName);

            if (isFinalHole)
            {
                ScoreManager.Instance.SaveTotal();
            }
            else
            {
                ScoreManager.Instance.ResetHole();
            }
        }

        Debug.Log(holeName + " Complete!");

        if (transitionOverlay != null)
        {
            transitionOverlay.PlayAndLoad(
                holeName + " Complete!\nStrokes: " + strokesThisHole,
                nextSceneName
            );
        }
    }
}