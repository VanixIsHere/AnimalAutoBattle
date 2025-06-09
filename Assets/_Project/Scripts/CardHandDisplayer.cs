using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardHandDisplayer : MonoBehaviour
{
    [Header("Camera References")]
    public Camera playerCamera;
    [SerializeField] private Camera cardCameraPrefab;

    private readonly List<GameObject> cardsInHand = new();

    [Header("Layout Settings")]
    [SerializeField] private float distanceFromCamera = 2.5f;
    [SerializeField] private float verticalOffsetFromCamera = -0.5f; // negative = down
    [SerializeField] public float spawnVerticalOffsetFromCamera = 12f;
    [SerializeField] private float spacing = 1.8f; // Space between cards
    [SerializeField] private int layoutHandSize = 5;

    [Header("Arc Settings")]
    [SerializeField] private float arcRadius = 1.5f;           // how curved the hand is (0 = flat line)
    [SerializeField] private float arcVerticalCurve = 0.5f;    // how high the curve lifts outer cards
    [SerializeField] private float arcVerticalDrop = 0.5f;     // how much more vertical space cards get
    [SerializeField] private float arcTiltDegrees = 15f;       // how much cards tilt forward/back
    [SerializeField] private float arcTwistDegrees = 10f;      // how much each card rotates around its thin edge (Z)
    [SerializeField] private float arcRollDegrees = 10f;       // How much each card rotates around its front vertical edge (Y)

    public void SetLayoutHandSize(int size)
    {
        layoutHandSize = Mathf.Max(1, size);
    }

    void Update()
    {
        LayoutCards();
        UpdateCardLowering();
    }

    public void SetCards(List<GameObject> cards)
    {
        cardsInHand = cards;
        LayoutCards();
    }

    float GetHandSizeScale()
    {
        if (layoutHandSize <= 1)
            return 1f;
        return (cardsInHand.Count - 1f) / (layoutHandSize - 1f);
    }

    public void LayoutCards()
    {
        if (playerCamera == null || cardsInHand.Count == 0) return;

        float totalWidth = (cardsInHand.Count - 1) * spacing;

        // Ray direction and base point
        Vector3 rayOrigin = playerCamera.transform.position;
        Vector3 rayDirection = playerCamera.transform.forward;
        Vector3 cameraDown = -playerCamera.transform.up; // This is "screen-space down"

        // Distance outward from camera
        Vector3 centerPoint = rayOrigin + rayDirection * distanceFromCamera + cameraDown * verticalOffsetFromCamera;

        float sizeScale = GetHandSizeScale();

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            var card = cardsInHand[i];
            var state = card.GetComponent<CardState>();
            state.SetBaseRenderLayer($"Card{i}");

            if (state != null && (state.IsDragging || state.IsHovering))
            {
                // Debug.Log($"Card {i + 1} skipping");
                continue;
            }

            LayoutSingleCard(state, i, totalWidth, centerPoint, sizeScale);
        }
    }

    void LayoutSingleCard(CardState state, int index, float totalWidth, Vector3 centerPoint, float sizeScale)
    {
        float middleIndex = (cardsInHand.Count - 1) / 2f;
        float normalizedIndex = (cardsInHand.Count == 1) ? 0f : (index - middleIndex) / middleIndex;

        float offsetX = (index * spacing) - (totalWidth * 0.5f);
        Vector3 rightOffset = playerCamera.transform.right * offsetX;
        Vector3 targetPos = centerPoint + rightOffset;

        Quaternion targetRot = GetBaseRotation();
        ApplyArcModifiers(ref targetPos, ref targetRot, normalizedIndex, sizeScale);

        if (state != null)
        {
            state.HandAnchorPosition = targetPos;
            state.HandAnchorRotation = targetRot;
        }
    }

    Quaternion GetBaseRotation()
    {
        Quaternion targetRot = Quaternion.LookRotation(-playerCamera.transform.forward, Vector3.up);
        targetRot *= Quaternion.Euler(90f, 0f, 0f);
        targetRot *= Quaternion.Euler(0f, 180f, 0f);
        return targetRot;
    }

    void ApplyArcModifiers(ref Vector3 position, ref Quaternion rotation, float normalizedIndex, float sizeScale)
    {
        float tilt = (1f - Mathf.Abs(normalizedIndex)) * arcTiltDegrees * sizeScale;
        rotation *= Quaternion.Euler(tilt, 0f, 0f);

        float twist = normalizedIndex * arcTwistDegrees * sizeScale;
        rotation *= Quaternion.Euler(0f, twist, 0f);

        float roll = normalizedIndex * arcRollDegrees * sizeScale;
        rotation *= Quaternion.Euler(0f, 0f, roll);

        float verticalArcOffset = (1f - Mathf.Abs(normalizedIndex)) * arcVerticalCurve * sizeScale;
        position += playerCamera.transform.up * verticalArcOffset;

        float forwardArcOffset = (1f - Mathf.Abs(normalizedIndex)) * arcRadius * sizeScale;
        position += playerCamera.transform.forward * forwardArcOffset;

        float dropOffset = Mathf.Pow(normalizedIndex, 2) * arcVerticalDrop * Mathf.Pow(sizeScale, 2);
        position += -playerCamera.transform.up * dropOffset;
    }

    private void UpdateCardLowering()
    {
        bool isAnyCardDragging = cardsInHand.Any(card => card.GetComponent<CardState>().IsDragging);

        foreach (var card in cardsInHand)
        {
            var state = card.GetComponent<CardState>();
            if (state == null) continue;

            state.IsLowered = isAnyCardDragging && !state.IsDragging;
        }
    }
}
