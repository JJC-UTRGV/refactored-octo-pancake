using TMPro;
using UnityEngine;

public class HighScoresUI : MonoBehaviour
{
    [Header("Score Text")]
    [SerializeField] private TMP_Text hole1ScoreText;
    [SerializeField] private TMP_Text hole2ScoreText;

    private void OnEnable()
    {
        RefreshScores();
    }

    public void RefreshScores()
    {
        int hole1Best = ScoreManager.Instance != null ? ScoreManager.Instance.GetBestHole("Hole1") : -1;
        int hole2Best = ScoreManager.Instance != null ? ScoreManager.Instance.GetBestHole("Hole2") : -1;

        hole1ScoreText.text = hole1Best == -1 ? "Best: --" : "Best: " + hole1Best;
        hole2ScoreText.text = hole2Best == -1 ? "Best: --" : "Best: " + hole2Best;

        Debug.Log("Hole1_Best = " + hole1Best);
        Debug.Log("Hole2_Best = " + hole2Best);
    }
}