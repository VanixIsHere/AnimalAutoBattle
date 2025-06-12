using UnityEngine;

public enum CursorState
{
    Normal,
    HoverGrab,
    Grab
}

[System.Serializable]
public class CursorPreset
{
    public Texture2D texture;
    public Vector2 hotspot;
}

public class HandleCursor : MonoBehaviour
{
    public CursorPreset normal;
    public CursorPreset hoverGrab;
    public CursorPreset grab;
    public CursorMode cursorMode = CursorMode.Auto;

    public CursorState currentState { get; private set; } = CursorState.Normal;

    public void SetState(CursorState newState)
    {
        if (currentState == newState)
            return;

        CursorPreset preset = new CursorPreset
        {
            texture = null,
            hotspot = Vector2.zero,
        };

        switch (newState)
        {
            case CursorState.HoverGrab:
                preset = hoverGrab;
                break;
            case CursorState.Grab:
                preset = grab;
                break;
        }

        Cursor.SetCursor(preset.texture, preset.hotspot, cursorMode);
        currentState = newState;
    }
}
