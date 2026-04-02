using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelTransitionOverlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text messageText;

    [Header("Timing")]
    [SerializeField] private float messageDelay = 0.6f;
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float holdAfterFade = 1.0f;

    private bool busy = false;

    void Awake()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        if (messageText != null)
            messageText.text = "";
    }

    public void PlayAndLoad(string message, string nextSceneName)
    {
        if (busy) return;

        Debug.Log("PlayAndLoad called");
        StartCoroutine(PlayAndLoadRoutine(message, nextSceneName));
    }

    private IEnumerator PlayAndLoadRoutine(string message, string nextSceneName)
    {
        busy = true;

        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        if (messageText != null)
            messageText.text = message;

        yield return new WaitForSeconds(messageDelay);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / fadeDuration);

            if (canvasGroup != null)
                canvasGroup.alpha = alpha;

            yield return null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(holdAfterFade);

        SceneManager.LoadSceneAsync(nextSceneName);
    }
}