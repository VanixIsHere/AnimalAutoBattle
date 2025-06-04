using UnityEngine;

public static class LayerUtils
{
    /// <summary>
    /// Recursively sets the layer for this GameObject and all its children.
    /// </summary>
    public static void SetLayerRecursive(GameObject obj, int layer)
    {
        if (obj == null) return;

        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }
}