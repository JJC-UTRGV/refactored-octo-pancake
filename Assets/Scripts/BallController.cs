using UnityEngine;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    public Transform aimIndicator;
    public float maxForce = 10f;

    Vector3 startPosition;
    Rigidbody rb;

    Vector2 dragStart;
    bool dragging = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        aimIndicator.gameObject.SetActive(false);
        startPosition = transform.position;
    }

    void Update()
    {
        if (transform.position.y < -5f)
        {
            // ResetBall();
        }
        HandleAim();
        //CheckFall();
    }

    void HandleAim()
    {
        if (rb.linearVelocity.magnitude > 0.1f) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            dragStart = Mouse.current.position.ReadValue();
            dragging = true;
            aimIndicator.gameObject.SetActive(true);
        }

        if (dragging)
        {
            Vector2 currentMouse = Mouse.current.position.ReadValue();
            Vector2 drag = dragStart - currentMouse;

            Vector3 direction = new Vector3(drag.x, 0, drag.y).normalized;

            aimIndicator.position = transform.position + direction * 1.2f;
            aimIndicator.forward = direction;

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                Shoot(drag.magnitude);
                dragging = false;
                aimIndicator.gameObject.SetActive(false);
            }
        }
    }

    void Shoot(float dragLength)
    {
        float force = Mathf.Clamp(dragLength / 100f, 0, maxForce);

        rb.AddForce(aimIndicator.forward * force, ForceMode.Impulse);

        GameManager.Instance.AddStroke();
    }

    // void CheckFall()
    // {
    //     if (transform.position.y < -5)
    //     {
    //         GameManager.Instance.RespawnBall();
    //     }
    // }
    void ResetBall()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = startPosition;
    }
}