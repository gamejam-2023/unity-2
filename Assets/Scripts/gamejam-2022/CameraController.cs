using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 8f;

    private Vector3 offset;

    void Start()
    {
        if (!target) return;
        offset = transform.position - target.position; // keeps your current camera angle/distance
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desired = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desired, followSpeed * Time.deltaTime);

        // Optional: if you want the camera to always face the player
        // transform.LookAt(target);
    }
}
