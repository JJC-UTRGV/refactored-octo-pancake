using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    public TMP_Text timerText;

    void Update()
    {
        if (ScoreManager.Instance == null || timerText == null)
        {
            return;
        }

        timerText.text = "Time: " + FormatTime(ScoreManager.Instance.currentHoleTime);
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}