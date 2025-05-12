using System;
using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    public GameObject hexPrefab;
    public int width = 6;
    public int height = 5;
    public float hexSize = 1f;
    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        float xOffset = Mathf.Sqrt(3f) * hexSize;
        float yOffset = 1.5f * hexSize;

        for (int r = 0; r < height; r++)
        {
            int rOffset = Mathf.FloorToInt(r / 2f);
            for (int q = -rOffset; q < width - rOffset; q++)
            {
                Vector3 pos = new Vector3(
                    xOffset * (q + r * 0.5f),
                    0,
                    yOffset * r
                );

                GameObject hex = Instantiate(hexPrefab, pos, Quaternion.identity, transform);
                hex.GetComponent<HexCell>().Init(q, r);
            }
        }
        
        CreateGridCenterAndPositionCamera(xOffset, yOffset);
    }

    void CreateGridCenterAndPositionCamera(float xOffset, float yOffset)
    {
        // Calculate grid center
        float centerX = xOffset * (width - 1) / 2f;
        float centerZ = yOffset * (height - 1) / 2f;

        Vector3 center = new Vector3(centerX, 0, centerZ);

        // Create the center marker
        GameObject gridCenter = new GameObject("GridCenter");
        gridCenter.transform.position = center;

        // Attach a gizmo for debug
        gridCenter.AddComponent<GridCenterGizmo>();

        // Reposition the camera
        Camera.main.GetComponent<CameraPositioner>().PositionCamera(width, height, hexSize);
        Camera.main.GetComponent<CameraPositioner>().target = gridCenter.transform;

        var cam = Camera.main;
        if (cam != null)
        {
            // Position the camera if the script is present
            var camPositioner = cam.GetComponent<CameraPositioner>();
            if (camPositioner != null)
            {
                camPositioner.target = gridCenter.transform;
                camPositioner.PositionCamera(width, height, hexSize);
            }
            else
            {
                Debug.LogWarning("Camera Positioner script missing on Main Camera.");
            }
        }
        else
        {
            Debug.LogError("Main Camera not found in scene!");
        }
    }
}
