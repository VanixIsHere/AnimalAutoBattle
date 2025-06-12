using UnityEngine;

public class CameraPositioner : MonoBehaviour
{
    public Transform target; // GridCenter is assigned at runtime
    [Range(45f, 60f)]
    public float minAngle = 45f;
    [Range(45f, 60f)]
    public float maxAngle = 60f;
    public float padding = 1.2f; // Extra zoom-out buffer
    [Range(0.1f, 1f)]
    public float horizontalCoverage = 0.7f; // Portion of the view width the grid should occupy

    [Range(0f, 0.5f)]
    public float topOfGridPercent = 0.15f; // distance from top of screen
    [Range(0.5f, 1f)]
    public float benchBottomPercent = 0.75f; // distance from top of screen

    static float ViewportY(float z, float angleRad, float distance, float yOffset, float tanHalfFov)
    {
        float A = Mathf.Sin(angleRad);
        float B = Mathf.Cos(angleRad);
        return 0.5f + 0.5f * ((A * z - B * yOffset) / ((distance + A * yOffset + B * z) * tanHalfFov));
    }

    static float SolveYOffset(float zCenter, float desiredV, float angleRad, float distance, float tanHalfFov)
    {
        float A = Mathf.Sin(angleRad);
        float B = Mathf.Cos(angleRad);
        float k = 2f * (desiredV - 0.5f);
        float denom = k * tanHalfFov * A + B;
        if (Mathf.Abs(denom) < 0.0001f)
            return 0f;
        return (A * zCenter - k * tanHalfFov * (distance + B * zCenter)) / denom;
    }

    public void PositionCamera(int width, int height, float hexSize)
    {
        if (target == null)
        {
            Debug.LogWarning("Camera target not assigned — skipping camera positioning.");
            return;
        }
        var cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("Main camera not found — skipping camera positioning.");
            return;
        }

        float worldWidth = (width + 0.5f) * Mathf.Sqrt(3f) * hexSize;
        float halfGridHeight = (height - 1) * 1.5f * hexSize * 0.5f;

        // Try to use the bench manager for more accurate vertical extent
        BenchManager bench = FindFirstObjectByType<BenchManager>();
        float benchOffset = -(halfGridHeight + 2.5f * hexSize);
        if (bench != null)
        {
            benchOffset = bench.CurrentYOffset;
        }

        float verticalFovRad = cam.fieldOfView * Mathf.Deg2Rad;
        float horizontalFovRad = 2f * Mathf.Atan(Mathf.Tan(verticalFovRad * 0.5f) * cam.aspect);
        float tanHalfFov = Mathf.Tan(verticalFovRad * 0.5f);

        float desiredTopV = 1f - Mathf.Clamp01(topOfGridPercent);
        float desiredBottomV = 1f - Mathf.Clamp01(benchBottomPercent);

        float bestScore = float.MaxValue;
        float bestAngle = minAngle;
        float bestDistance = 10f;
        float bestYOffset = 0f;

        float topV = Mathf.Clamp01(topOfGridPercent);
        float bottomV = Mathf.Clamp01(benchBottomPercent);
        float vCoverage = bottomV - topV;
        if (vCoverage < 0.05f)
            vCoverage = 0.05f;

        for (float a = minAngle; a <= maxAngle; a += 0.5f)
        {
            float aRad = a * Mathf.Deg2Rad;

            float requiredVisibleWidth = (worldWidth * padding) / Mathf.Clamp(horizontalCoverage, 0.01f, 0.99f);
            float distFromWidth = (requiredVisibleWidth * 0.5f) / Mathf.Tan(horizontalFovRad * 0.5f);

            float kTop = 2f * (desiredTopV - 0.5f);
            float kBot = 2f * (desiredBottomV - 0.5f);
            float A = Mathf.Sin(aRad);
            float B = Mathf.Cos(aRad);

            float CTop = A * halfGridHeight - kTop * tanHalfFov * B * halfGridHeight;
            float DTop = B + kTop * tanHalfFov * A;
            float CBot = A * benchOffset - kBot * tanHalfFov * B * benchOffset;
            float DBot = B + kBot * tanHalfFov * A;

            float denom = kTop * DBot - kBot * DTop;
            if (Mathf.Abs(denom) < 0.0001f)
                continue;

            float yOff = (kTop * CBot - kBot * CTop) / denom;
            float distVert = (CTop - DTop * yOff) / (kTop * tanHalfFov);
            if (distVert <= 0f)
                continue;

            float distance = distVert;
            if (distFromWidth > distance)
            {
                distance = distFromWidth;
                yOff = SolveYOffset(halfGridHeight, desiredTopV, aRad, distance, tanHalfFov);
            }

            float bottomY = ViewportY(benchOffset, aRad, distance, yOff, tanHalfFov);
            float score = Mathf.Abs(bottomY - desiredBottomV);

            if (score < bestScore)
            {
                bestScore = score;
                bestAngle = a;
                bestDistance = distance;
                bestYOffset = yOff;
            }
        }

        Quaternion rot = Quaternion.Euler(bestAngle, 0f, 0f);
        Vector3 pivot = target.position + new Vector3(0f, bestYOffset, 0f);
        Vector3 offset = rot * new Vector3(0f, 0f, -bestDistance);
        cam.transform.SetPositionAndRotation(pivot + offset, rot);
    }
}