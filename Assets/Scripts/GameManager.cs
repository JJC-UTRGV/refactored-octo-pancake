using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Transform ball;
    public Vector3 spawnPoint;

    public TextMeshProUGUI strokeText;

    int strokes = 0;

    void Awake()
    {
        Instance = this;
        spawnPoint = ball.position;
        UpdateUI();
    }

    public void AddStroke()
    {
        strokes++;
        UpdateUI();
    }

    void UpdateUI()
    {
        strokeText.text = "Strokes: " + strokes;
    }

    public void RespawnBall()
    {
        ball.position = spawnPoint;
        ball.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }
}