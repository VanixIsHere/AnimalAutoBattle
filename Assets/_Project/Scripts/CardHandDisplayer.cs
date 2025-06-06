using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardHandDisplayer : MonoBehaviour
{
    public Camera playerCamera;
    [SerializeField] private Camera cardCameraPrefab;
    private List<GameObject> cardsInHand = new();

    [SerializeField] private float distanceFromCamera = 2.5f;
    [SerializeField] private float verticalOffsetFromCamera = -0.5f; // negative = down
    [SerializeField] private float spacing = 1.8f; // Space between cards
    [SerializeField] private float arcRadius = 1.5f;           // how curved the hand is (0 = flat line)
    [SerializeField] private float arcVerticalCurve = 0.5f;    // how high the curve lifts outer cards
    [SerializeField] private float arcVerticalDrop = 0.5f; // how much more vertical space cards get
    [SerializeField] private float arcTiltDegrees = 15f;       // how much cards tilt forward/back
    [SerializeField] private float arcTwistDegrees = 10f; // how much each card rotates around its thin edge (Z)
    [SerializeField] private float arcRollDegrees = 10f; // How much each card rotates around its front vertical edge (Y)

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

            // Apply fan rotation
            float middleIndex = (cardsInHand.Count - 1) / 2f;
            float normalizedIndex = (cardsInHand.Count == 1) ? 0f : (i - middleIndex) / middleIndex; // -1 to 1
            if (cardsInHand.Count == 1)
            {
                normalizedIndex = 0f;
            }

            float offsetX = (i * spacing) - (totalWidth * 0.5f);
            Vector3 rightOffset = playerCamera.transform.right * offsetX;
            Vector3 targetPos = centerPoint + rightOffset;

            Quaternion targetRot = Quaternion.LookRotation(-playerCamera.transform.forward, Vector3.up);
            targetRot *= Quaternion.Euler(90f, 0f, 0f);
            targetRot *= Quaternion.Euler(0f, 180f, 0f);

            // Tilt fan (X rotation)
            float tilt = (1f - Mathf.Abs(normalizedIndex)) * arcTiltDegrees;
            targetRot *= Quaternion.Euler(tilt, 0f, 0f);

            // Twist fan (Y rotation) - like a steering wheel
            float twist = normalizedIndex * arcTwistDegrees;
            targetRot *= Quaternion.Euler(0f, twist, 0f);

            // 🆕 Roll fan (Z rotation) - like spinning the card's face
            float roll = normalizedIndex * arcRollDegrees;
            targetRot *= Quaternion.Euler(0f, 0f, roll);

            // Vertical offset
            float verticalArcOffset = (1f - Mathf.Abs(normalizedIndex)) * arcVerticalCurve;
            targetPos += playerCamera.transform.up * verticalArcOffset;

            // Forward arc bow
            float forwardArcOffset = (1f - Mathf.Abs(normalizedIndex)) * arcRadius;
            targetPos += playerCamera.transform.forward * forwardArcOffset;

            // Individual Card Drop
            float dropOffset = Mathf.Pow(normalizedIndex, 2) * arcVerticalDrop;
            targetPos += -playerCamera.transform.up * dropOffset;

            if (state != null)
            {
                state.HandAnchorPosition = targetPos;
                state.HandAnchorRotation = targetRot;
            }
        }
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
