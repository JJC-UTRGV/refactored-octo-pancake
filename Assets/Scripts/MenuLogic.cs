using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLogic : MonoBehaviour
{
    [SerializeField]
    public GameObject settingsPanel;

    void Start()
    {
        if (settingsPanel == null)
            return;

        SettingsPanelController settingsController = settingsPanel.GetComponent<SettingsPanelController>();

        if (settingsController == null)
            settingsController = settingsPanel.AddComponent<SettingsPanelController>();

        settingsController.Initialize(this);
        settingsPanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
