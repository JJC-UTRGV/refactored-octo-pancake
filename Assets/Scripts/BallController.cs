using UnityEngine;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer aimLine;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Collider ballCollider;

    [Header("Shot Settings")]
    [SerializeField] private float maxForce = 10f;
    [SerializeField] private float dragToForce = 100f;
    [SerializeField] private float minDragDistance = 10f;

    [Header("Aim Line")]
    [SerializeField] private float minLineLength = 0.8f;
    [SerializeField] private float maxLineLength = 2.5f;
    [SerializeField] private float lineStartWidth = 0.08f;
    [SerializeField] private float lineEndWidth = 0.04f;
    [SerializeField] private float indicatorGap = 0.05f;
    [SerializeField] private float verticalOffset = 0.03f;

    [Header("State Checks")]
    [SerializeField] private float stopThreshold = 0.1f;
    [SerializeField] private float fallHeight = -5f;

    private Vector3 startPosition;
    private Rigidbody rb;

    private Vector2 dragStart;
    private bool dragging = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;

        if (ballCollider == null)
            ballCollider = GetComponent<Collider>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (aimLine != null)
        {
            aimLine.positionCount = 2;
            aimLine.useWorldSpace = true;
            aimLine.startWidth = lineStartWidth;
            aimLine.endWidth = lineEndWidth;
            aimLine.enabled = false;
        }
    }

    void Update()
    {
        CheckFall();
        HandleAim();
    }

    void HandleAim()
    {
        if (Mouse.current == null || aimLine == null || cameraTransform == null)
            return;

        if (BallIsMoving())
        {
            CancelAim();
            return;
        }

        if (Mouse.current.rightButton.isPressed)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            dragStart = Mouse.current.position.ReadValue();
            dragging = true;
            aimLine.enabled = true;
        }

        if (!dragging)
            return;

        Vector2 currentMouse = Mouse.current.position.ReadValue();
        Vector2 drag = dragStart - currentMouse;
        float dragLength = drag.magnitude;

        if (dragLength > 0.01f)
        {
            Vector3 direction = GetCameraRelativeDirection(drag);

            if (direction.sqrMagnitude > 0.0001f)
            {
                float force = Mathf.Clamp(dragLength / dragToForce, 0f, maxForce);
                float powerPercent = maxForce > 0f ? force / maxForce : 0f;

                UpdateAimLine(direction, powerPercent);
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (dragLength >= minDragDistance)
            {
                Vector3 shotDirection = GetCameraRelativeDirection(drag);
                Shoot(shotDirection, dragLength);
            }

            CancelAim();
        }
    }

    Vector3 GetCameraRelativeDirection(Vector2 drag)
    {
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 direction = (camRight * drag.x) + (camForward * drag.y);
        return direction.normalized;
    }

    Vector3 GetBallCenter()
    {
        if (ballCollider != null)
            return ballCollider.bounds.center;

        return transform.position;
    }

    float GetBallRadius()
    {
        if (ballCollider != null)
        {
            Vector3 extents = ballCollider.bounds.extents;
            return Mathf.Max(extents.x, extents.z);
        }

        return 0.5f;
    }

    void UpdateAimLine(Vector3 direction, float powerPercent)
    {
        float lineLength = Mathf.Lerp(minLineLength, maxLineLength, powerPercent);
        float ballRadius = GetBallRadius();

        Vector3 center = GetBallCenter() + Vector3.up * verticalOffset;
        Vector3 start = center + direction * (ballRadius + indicatorGap);
        Vector3 end = start + direction * lineLength;

        aimLine.startWidth = lineStartWidth;
        aimLine.endWidth = lineEndWidth;

        aimLine.SetPosition(0, start);
        aimLine.SetPosition(1, end);
    }

    void Shoot(Vector3 direction, float dragLength)
    {
        float force = Mathf.Clamp(dragLength / dragToForce, 0f, maxForce);
        rb.AddForce(direction * force, ForceMode.Impulse);

        GameManager.Instance.AddStroke();
    }

    bool BallIsMoving()
    {
        return rb.linearVelocity.magnitude > stopThreshold;
    }

    void CancelAim()
    {
        dragging = false;

        if (aimLine != null)
            aimLine.enabled = false;
    }

    void CheckFall()
    {
        if (transform.position.y < fallHeight)
        {
            ResetBall();
        }
    }

    void ResetBall()
    {
        CancelAim();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPosition;
    }
}