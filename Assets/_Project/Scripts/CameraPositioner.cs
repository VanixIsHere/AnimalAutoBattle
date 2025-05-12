using UnityEngine;

public class CameraPositioner : MonoBehaviour
{
    public Transform target; // GridCenter is assigned at runtime
    public float angle = 45f; // Tilt angle downwards
    public float padding = 1.2f; // Extra zoom-out buffer

    public void PositionCamera(int width, int height, float hexSize)
    {
        if (target == null)
        {
            Debug.LogWarning("Camera target not assigned — skipping camera positioning.");
            return;
        }
        float worldWidth = (width + 0.5f) * Mathf.Sqrt(3f) * hexSize;
        float worldHeight = (height + 1) * 1.5f * hexSize;
        float maxExtent = Mathf.Max(worldWidth, worldHeight) * 0.5f * padding;

        float distance = maxExtent / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);

        Vector3 offset = Quaternion.Euler(angle, 0f, 0f) * new Vector3(0, 0, -distance);
        Camera.main.transform.position = target.position + offset;
        Camera.main.transform.LookAt(target);
    }
}