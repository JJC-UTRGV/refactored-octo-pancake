using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameCompleteManager : MonoBehaviour
{
    public TMP_Text hole1Text;
    public TMP_Text hole2Text;
    public TMP_Text hole3Text;
    public TMP_Text hole4Text;
    public TMP_Text hole5Text;
    public TMP_Text totalText;
    public TMP_Text totalTimeText;

    public GameObject highScoresPanel;
    public TMP_Text highScoresText;

    void Start()
    {
        ShowCurrentScores();

        if (highScoresPanel != null)
        {
            highScoresPanel.SetActive(false);
        }
    }

    void ShowCurrentScores()
    {
        if (ScoreManager.Instance == null)
        {
            Debug.LogWarning("No ScoreManager found. Start from Game, not GameComplete.");
            return;
        }

        int[] scores = ScoreManager.Instance.holeScores;
        float[] times = ScoreManager.Instance.holeTimes;

        hole1Text.text = "Hole 1: " + scores[0] + " strokes | " + FormatTime(times[0]);
        hole2Text.text = "Hole 2: " + scores[1] + " strokes | " + FormatTime(times[1]);
        hole3Text.text = "Hole 3: " + scores[2] + " strokes | " + FormatTime(times[2]);
        hole4Text.text = "Hole 4: " + scores[3] + " strokes | " + FormatTime(times[3]);
        hole5Text.text = "Hole 5: " + scores[4] + " strokes | " + FormatTime(times[4]);

        totalText.text = "Total Strokes: " + ScoreManager.Instance.totalStrokes;

        if (totalTimeText != null)
        {
            totalTimeText.text = "Total Time: " + FormatTime(ScoreManager.Instance.totalTime);
        }
    }

    public void PlayAgain()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetGame();
        }

        SceneManager.LoadScene("Game");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SeeHighScores()
    {
        if (highScoresPanel != null)
        {
            highScoresPanel.SetActive(true);
        }

        if (highScoresText != null && ScoreManager.Instance != null)
        {
            highScoresText.text =
                "High Scores\n\n" +
                "Hole 1 Best: " + FormatScore(ScoreManager.Instance.GetBestHole("Hole 1")) + "\n" +
                "Hole 2 Best: " + FormatScore(ScoreManager.Instance.GetBestHole("Hole 2")) + "\n" +
                "Hole 3 Best: " + FormatScore(ScoreManager.Instance.GetBestHole("Hole 3")) + "\n" +
                "Hole 4 Best: " + FormatScore(ScoreManager.Instance.GetBestHole("Hole 4")) + "\n" +
                "Hole 5 Best: " + FormatScore(ScoreManager.Instance.GetBestHole("Hole 5")) + "\n\n" +
                "Best Total: " + FormatScore(ScoreManager.Instance.GetBestTotal());
        }
    }

    public void CloseHighScores()
    {
        if (highScoresPanel != null)
        {
            highScoresPanel.SetActive(false);
        }
    }

    string FormatScore(int score)
    {
        if (score == -1)
        {
            return "--";
        }

        return score + " strokes";
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}