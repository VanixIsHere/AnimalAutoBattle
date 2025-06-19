using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    public VisualTreeAsset InGameMenuContainer;
    private bool isPaused = false;

    // Runtime state
    private Camera mainCamera;
    private HandleCursor cursor;
    private UIBlockerManager uiBlocker;
    private ModalManager modal;
    private UIDocument rootDoc;

    void Awake()
    {
        rootDoc = GetComponent<UIDocument>();
        if (rootDoc != null)
            rootDoc.rootVisualElement.style.display = DisplayStyle.None;
        mainCamera = Camera.main;
    }

    void Start()
    {
        cursor = mainCamera.GetComponent<HandleCursor>();
        uiBlocker = GetComponent<UIBlockerManager>();
        modal = GetComponent<ModalManager>();

        var menuContainer = InGameMenuContainer.CloneTree();
        var root = rootDoc.rootVisualElement.Q<VisualElement>("ROOT");

        var nextLayer = menuContainer.Q<VisualElement>("nextLayer");
        var primaryLayer = menuContainer.Q<VisualElement>("primary-layer");
        root.Add(menuContainer);

        var rootMenu = new List<IMenuItem>
        {
            new LeafMenuItem("Resume", ResumeGame),
            new Submenu("Settings", "tier1-button",
                new GroupContainerMenuItem("Audio", "",
                    new SliderSetting("Master Volume", 0f, 100f, 50f, v => Debug.Log("Master Volume: " + v)),
                    new SliderSetting("Music Volume", 0f, 100f, 70f, v => Debug.Log("Music Volume: " + v))
                ),
                new GroupContainerMenuItem("Video", "",
                    new DropdownSetting("Resolution", new List<string> { "1920x1080", "1280x720" }, "1920x1080", val => Debug.Log("Res: " + val)),
                    new ToggleSetting("Fullscreen", true, b => Debug.Log("Fullscreen: " + b))
                ),
                new GroupContainerMenuItem("Gameplay", "",
                    new ToggleSetting("<filler option gameplay>", true, b => Debug.Log("Useless: " + b))
                ),
                new GroupContainerMenuItem("Controls", "",
                    new ToggleSetting("<filler option controls>", true, b => Debug.Log("Useless: " + b))
                ),
                new GroupContainerMenuItem("Misc", "",
                    new ToggleSetting("<filler option misc>", true, b => Debug.Log("Useless: " + b))
                )
            ),
            new Submenu("Debug",
                new LeafMenuItem("Enter matchmaking", HandleAttemptMatchmaking)
            ),
            new LeafMenuItem("Quit Game", HandleQuit)
        };

        foreach (var item in rootMenu)
        {
            var btn = new Button(() =>
            {
                var nextLayer = UIUtils.CreateOrGetLayerColumn(primaryLayer, 2);
                item.OnClick(primaryLayer, nextLayer, 2);
            })
            { text = item.Label };

            btn.AddToClassList("menu-button");
            btn.AddToClassList("tier1-button");

            if (!string.IsNullOrEmpty(item.StyleClass))
                btn.AddToClassList(item.StyleClass);

            primaryLayer.Add(btn);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (rootDoc != null)
        {
            var root = rootDoc.rootVisualElement;
            root.style.display = isPaused ? DisplayStyle.Flex : DisplayStyle.None;

            if (isPaused)
            {
                // float scale = Screen.height / 1080f;
                // FontScaler.ScaleFonts(root, scale);
            }
        }

        bool shouldPauseTime = GameManager.Instance != null && !GameManager.Instance.IsGameActive; // Don't freeze gameplay if in an active session
        Time.timeScale = shouldPauseTime && isPaused ? 0f : 1f;  // Pause or resume game time

        uiBlocker?.SetBlocking(isPaused);
        cursor.SetState(CursorState.Normal);
    }

    // Optional resume button hook
    public void ResumeGame()
    {
        isPaused = false;

        if (rootDoc != null)
            rootDoc.rootVisualElement.style.display = DisplayStyle.None;

        Time.timeScale = 1f;

        uiBlocker?.SetBlocking(isPaused);
        cursor.SetState(CursorState.Normal);
    }

    private void ExitLeafNode()
    {

    }

    private void OpenAudioSettings()
    {

    }

    private void OpenVideoSettings()
    {

    }

    private void HandleAttemptMatchmaking()
    {

    }

    private void HandleQuit()
    {
        modal.ShowConfirm(
            "Are you sure you want to leave?",
            "Any unsaved changes will be lost.",
            Application.Quit,
            () => Debug.Log("Cancelled Quit"),
            "Yes",
            "No"
        );
    }
}
