using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int currentHoleStrokes = 0;
    public int totalStrokes = 0;

    public float currentHoleTime = 0f;
    public float totalTime = 0f;

    public int[] holeScores = new int[5];
    public float[] holeTimes = new float[5];

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        currentHoleTime += Time.deltaTime;
        totalTime += Time.deltaTime;
    }

    public void AddStroke()
    {
        currentHoleStrokes++;
        totalStrokes++;
    }

    public void ResetHole()
    {
        currentHoleStrokes = 0;
        currentHoleTime = 0f;
    }

    public void SaveHole(string holeName)
    {
        string key = holeName + "_Best";

        if (!PlayerPrefs.HasKey(key) || currentHoleStrokes < PlayerPrefs.GetInt(key))
        {
            PlayerPrefs.SetInt(key, currentHoleStrokes);
        }

        if (holeName == "Hole 1")
        {
            holeScores[0] = currentHoleStrokes;
            holeTimes[0] = currentHoleTime;
        }
        if (holeName == "Hole 2")
        {
            holeScores[1] = currentHoleStrokes;
            holeTimes[1] = currentHoleTime;
        }
        if (holeName == "Hole 3")
        {
            holeScores[2] = currentHoleStrokes;
            holeTimes[2] = currentHoleTime;
        }
        if (holeName == "Hole 4")
        {
            holeScores[3] = currentHoleStrokes;
            holeTimes[3] = currentHoleTime;
        }
        if (holeName == "Hole 5")
        {
            holeScores[4] = currentHoleStrokes;
            holeTimes[4] = currentHoleTime;
        }

        PlayerPrefs.Save();
    }

    public int GetBestHole(string holeName)
    {
        return PlayerPrefs.GetInt(holeName + "_Best", -1);
    }

    public void SaveTotal()
    {
        string key = "BestTotal";

        if (!PlayerPrefs.HasKey(key) || totalStrokes < PlayerPrefs.GetInt(key))
        {
            PlayerPrefs.SetInt(key, totalStrokes);
        }

        PlayerPrefs.Save();
    }

    public int GetBestTotal()
    {
        return PlayerPrefs.GetInt("BestTotal", -1);
    }

    public void ResetGame()
    {
        currentHoleStrokes = 0;
        totalStrokes = 0;
        currentHoleTime = 0f;
        totalTime = 0f;

        for (int i = 0; i < holeScores.Length; i++)
        {
            holeScores[i] = 0;
            holeTimes[i] = 0f;
        }
    }
}