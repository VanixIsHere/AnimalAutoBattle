using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using GameSettings;

public class PauseMenuController : MonoBehaviour
{
    public GameSettingsManager GameSettingsManager;
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
        GroupContainerMenuItem.SetModalManager(modal);

        var menuContainer = InGameMenuContainer.CloneTree();
        var root = rootDoc.rootVisualElement.Q<VisualElement>("ROOT");

        var elementContent = menuContainer.Q<VisualElement>("ui-element-content");
        root.Add(menuContainer);

        var rootMenu = new List<IMenuItem>
        {
            new LeafMenuItem("Resume", ResumeGame),
            new Submenu("Settings", "tier1-button",
                new GroupContainerMenuItem("Audio", "",
                    new SliderSetting("Master Volume", 0f, 100f, GameSettingsManager.MasterVolume*100,
                        v =>
                        {
                            Debug.Log("Master Volume: " + v + " " + v/100);
                            GameSettingsManager.SetMasterVolume(v/100);
                        }),
                    new SliderSetting("Music Volume", 0f, 100f, GameSettingsManager.MusicVolume*100,
                        v =>
                        {
                            Debug.Log("Music Volume: " + v + " " + v/100);
                            GameSettingsManager.SetMusicVolume(v/100);
                        }),
                    new SliderSetting("SFX Volume", 0f, 100f, GameSettingsManager.SFXVolume*100,
                        v =>
                        {
                            Debug.Log("SFX Volume: " + v + " " + v/100);
                            GameSettingsManager.SetSFXVolume(v/100);
                        }),
                    new SliderSetting("Voice Volume", 0f, 100f, GameSettingsManager.VoiceVolume*100,
                        v =>
                        {
                            Debug.Log("Voice Volume: " + v + " " + v/100);
                            GameSettingsManager.SetVoiceVolume(v/100);
                        })
                ),
                new GroupContainerMenuItem("Video", "",
                    new DropdownSetting(
                        "Resolution",
                        ResolutionSettingExtensions.GetResolutionList(),
                        ResolutionSettingExtensions.ToResolutionString(GameSettingsManager.ScreenResolution),
                        val => {
                            Debug.Log("ScreenResolution: " + val);
                            GameSettingsManager.SetScreenResolution(ResolutionSettingExtensions.ToResolutionSetting(val));
                        }),
                    new DropdownSetting(
                        "Screen Mode",
                        ScreenModeSettingExtensions.GetScreenModeList(),
                        ScreenModeSettingExtensions.ToScreenModeString(GameSettingsManager.ScreenMode),
                        val => {
                            Debug.Log("ScreenMode: " + val);
                            GameSettingsManager.SetScreenMode(ScreenModeSettingExtensions.ToScreenModeSetting(val));
                        })
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
            new Submenu("test1",
                new Submenu("test2",
                    new LeafMenuItem("Enter matchmaking", HandleAttemptMatchmaking)
                ),
                new Submenu("test3",
                    new Submenu("test5",
                        new Submenu("test6",
                            new Submenu("test7",
                                new LeafMenuItem("Enter matchmaking", HandleAttemptMatchmaking)
                            )
                        )
                    )
                ),
                new Submenu("test4",
                    new LeafMenuItem("Enter matchmaking", HandleAttemptMatchmaking)
                )
            ),
            new LeafMenuItem("Quit Game", HandleQuit)
        };

        foreach (var item in rootMenu)
        {
            var btn = new Button(() =>
            {
                var nextLayer = UIUtils.CreateOrGetLayerColumn(elementContent, 2);
                item.OnClick(elementContent, nextLayer, 2);
            })
            { text = item.Label };

            btn.AddToClassList("menu-button");
            btn.AddToClassList("tier1-button");

            if (!string.IsNullOrEmpty(item.StyleClass))
                btn.AddToClassList(item.StyleClass);

            elementContent.Add(btn);
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
