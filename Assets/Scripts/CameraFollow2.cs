using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow2 : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow")]
    public float distance = 8f;
    public float height = 4f;
    public float smoothSpeed = 5f;
    public float lookHeight = 1f;

    [Header("Rotation")]
    public float rotateSpeed = 120f;
    public float pitch = 25f;
    public float minPitch = 10f;
    public float maxPitch = 60f;

    [Header("Aim Control")]
    public bool rotateWhileAimingOnly = true;
    public bool isAiming = false;

    private float yaw;

    void Start()
    {
        if (target == null) return;

        Vector3 dir = (target.position - transform.position).normalized;
        yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleRotation();
        FollowTarget();
    }

    void HandleRotation()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.isPressed) return;

        if (!Mouse.current.rightButton.isPressed) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        yaw += mouseDelta.x * rotateSpeed * Time.deltaTime;
        pitch -= mouseDelta.y * rotateSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void FollowTarget()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 rotatedOffset = rotation * new Vector3(0f, 0f, -distance);
        rotatedOffset.y += height;

        Vector3 desiredPosition = target.position + rotatedOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
        transform.LookAt(target.position + Vector3.up * lookHeight);
    }

    public Vector3 GetAimDirection()
    {
        Vector3 dir = transform.forward;
        dir.y = 0f;
        return dir.normalized;
    }

    public void SetAiming(bool aiming)
    {
        isAiming = aiming;
    }
}
