using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ModalManager : MonoBehaviour
{
    public VisualTreeAsset modalTemplate;
    private VisualElement root;

    void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("ROOT");
    }

    public void ShowConfirm(string primaryLabel, string secondaryLabel, Action onConfirm, Action onCancel = null, string confirmText = "Confirm", string cancelText = "Cancel")
    {
        var modal = modalTemplate.CloneTree();

        modal.Q<Label>("PrimaryText").text = primaryLabel;
        modal.Q<Label>("SecondaryText").text = secondaryLabel;
        var confirmButton = modal.Q<Button>("Confirm");
        confirmButton.text = confirmText;
        confirmButton.clicked += () => {
            root.Remove(modal);
            onConfirm?.Invoke();
        };
        var cancelButton = modal.Q<Button>("Cancel");
        cancelButton.text = cancelText;
        cancelButton.clicked += () => {
            root.Remove(modal);
            onCancel?.Invoke();
        };

        root.Add(modal);
    }
}
