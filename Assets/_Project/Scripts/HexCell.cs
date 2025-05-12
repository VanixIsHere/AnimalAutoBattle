using UnityEngine;

public class HexCell : MonoBehaviour
{
    public int q;
    public int r;

    public void Init(int q, int r)
    {
        this.q = q;
        this.r = r;
        name = $"HexCell ({q}, {r})";

        // Create a text label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(transform);
        labelObj.transform.localPosition = new Vector3(0, 0.1f, 0);
        labelObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        
        var textMesh = labelObj.AddComponent<TextMesh>();
        textMesh.text = $"{q},{r}";
        textMesh.characterSize = 0.2f;
        textMesh.fontSize = 32;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = Color.black;

        labelObj.AddComponent<WorldLabel>();
    }
}
