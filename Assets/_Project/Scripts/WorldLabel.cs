using UnityEngine;


[RequireComponent(typeof(TextMesh))]
public class WorldLabel : MonoBehaviour
{
    public void SetText(string text)
    {
        GetComponent<TextMesh>().text = text;
    }
}