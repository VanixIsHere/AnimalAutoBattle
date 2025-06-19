using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IMenuItem
{
    string Label { get; }
    string StyleClass { get; }
    void OnClick(VisualElement parentLayer, VisualElement nextLayer, int tier);
}

public interface ISettingItem
{
    VisualElement Build();
}

public class Submenu : IMenuItem
{
    public string Label { get; }
    public string StyleClass { get; }

    public List<IMenuItem> Children { get; }

    public Submenu(string label, params IMenuItem[] children)
        : this(label, null, children) { }

    public Submenu(string label, string styleClass, params IMenuItem[] children)
    {
        Label = label;
        StyleClass = styleClass;
        Children = new List<IMenuItem>(children);
    }

    public void OnClick(VisualElement parentLayer, VisualElement nextLayer, int tier)
    {
        nextLayer.Clear();

        foreach (var child in Children)
        {
            var btn = new Button(() =>
            {
                var childNext = UIUtils.CreateOrGetLayerColumn(nextLayer, tier + 1);
                child.OnClick(nextLayer, childNext, tier + 1);
            })
            { text = child.Label };

            btn.AddToClassList("menu-button");
            btn.AddToClassList($"tier{tier}-button");

            if (!string.IsNullOrEmpty(child.StyleClass))
                btn.AddToClassList(child.StyleClass);

            nextLayer.Add(btn);
        }
    }
}

public class LeafMenuItem : IMenuItem
{
    public string Label { get; }
    public string StyleClass { get; }

    private readonly System.Action onClickAction;

    public LeafMenuItem(string label, System.Action onClick, string styleClass = null)
    {
        Label = label;
        onClickAction = onClick;
        StyleClass = styleClass;
    }

    public void OnClick(VisualElement parentLayer, VisualElement nextLayer, int tier)
    {
        nextLayer.Clear();
        onClickAction?.Invoke();
    }
}

public class GroupContainerMenuItem : IMenuItem
{
    public string Label { get; }
    public string StyleClass { get; }

    private readonly List<ISettingItem> settings;

    public GroupContainerMenuItem(string label, string styleClass = null, params ISettingItem[] settings)
    {
        Label = label;
        this.settings = new List<ISettingItem>(settings);
        StyleClass = styleClass;
    }

    public void OnClick(VisualElement parentLayer, VisualElement nextLayer, int tier)
    {
        nextLayer.Clear();

        var scroll = new ScrollView();
        nextLayer.Add(scroll);

        foreach (var setting in settings)
        {
            scroll.Add(setting.Build());
        }
    }
}

public class ToggleSetting : ISettingItem
{
    private readonly string label;
    private readonly bool initialValue;
    private readonly System.Action<bool> onChanged;

    public ToggleSetting(string label, bool initialValue, System.Action<bool> onChanged)
    {
        this.label = label;
        this.initialValue = initialValue;
        this.onChanged = onChanged;
    }

    public VisualElement Build()
    {
        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.justifyContent = Justify.SpaceBetween;

        var lbl = new Label(label);
        var toggle = new Toggle { value = initialValue };

        toggle.RegisterValueChangedCallback(evt => onChanged?.Invoke(evt.newValue));

        container.Add(lbl);
        container.Add(toggle);
        return container;
    }
}

public class DropdownSetting : ISettingItem
{
    private readonly string label;
    private readonly List<string> options;
    private readonly string initialValue;
    private readonly System.Action<string> onChanged;

    public DropdownSetting(string label, List<string> options, string initialValue, System.Action<string> onChanged)
    {
        this.label = label;
        this.options = options;
        this.initialValue = initialValue;
        this.onChanged = onChanged;
    }

    public VisualElement Build()
    {
        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.justifyContent = Justify.SpaceBetween;

        var lbl = new Label(label);
        var dropdown = new DropdownField(options, initialValue);
        dropdown.RegisterValueChangedCallback(evt => onChanged?.Invoke(evt.newValue));

        container.Add(lbl);
        container.Add(dropdown);
        return container;
    }
}

public class SliderSetting : ISettingItem
{
    private readonly string label;
    private readonly float min;
    private readonly float max;
    private readonly float initialValue;
    private readonly System.Action<float> onChanged;

    public SliderSetting(string label, float min, float max, float initialValue, System.Action<float> onChanged)
    {
        this.label = label;
        this.min = min;
        this.max = max;
        this.initialValue = initialValue;
        this.onChanged = onChanged;
    }

    public VisualElement Build()
    {
        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.justifyContent = Justify.SpaceBetween;

        var lbl = new Label(label);
        var slider = new Slider(min, max) { value = initialValue };
        slider.RegisterValueChangedCallback(evt => onChanged?.Invoke(evt.newValue));

        container.Add(lbl);
        container.Add(slider);
        return container;
    }
}

public static class UIUtils
{
    public static VisualElement CreateOrGetLayerColumn(VisualElement currentLayer, int tier)
    {
        var parent = currentLayer.parent;
        int currentIndex = parent.IndexOf(currentLayer);
        int targetIndex = currentIndex + 1;

        // SAFETY: Check if targetIndex makes sense
        if (targetIndex < 0 || targetIndex > parent.childCount)
        {
            Debug.LogError($"Invalid target index: {targetIndex}. Current index: {currentIndex}. Parent has {parent.childCount} children.");
            targetIndex = parent.childCount; // fallback to append at end
        }

        // Remove layers at or beyond targetIndex
        while (parent.childCount > targetIndex)
        {
            parent.RemoveAt(parent.childCount - 1);
        }

        // Create new layer
        var layer = new VisualElement();
        layer.AddToClassList("menu-column");
        layer.AddToClassList($"tier{tier}-layer");
        parent.Add(layer);

        // Animate
        layer.schedule.Execute(() => {
            layer.AddToClassList("active");
        }).ExecuteLater(10);

        return layer;
    }
}
