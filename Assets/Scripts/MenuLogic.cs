using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLogic : MonoBehaviour
{
    public GameObject scorePanel;

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OpenScores()
    {
        scorePanel.SetActive(true);
    }

    public void CloseScores()
    {
        scorePanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}