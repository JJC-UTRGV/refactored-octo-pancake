using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int currentHoleStrokes = 0;
    public int totalStrokes = 0;

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

    public void AddStroke()
    {
        currentHoleStrokes++;
        totalStrokes++;
    }

    public void ResetHole()
    {
        currentHoleStrokes = 0;
    }

    // 🔹 Save best score for a hole
    public void SaveHole(string holeName)
    {
        string key = holeName + "_Best";

        if (!PlayerPrefs.HasKey(key) || currentHoleStrokes < PlayerPrefs.GetInt(key))
        {
            PlayerPrefs.SetInt(key, currentHoleStrokes);
        }

        PlayerPrefs.Save();
    }

    // 🔹 Get best score
    public int GetBestHole(string holeName)
    {
        return PlayerPrefs.GetInt(holeName + "_Best", -1);
    }

    // 🔹 Save total best score
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
}