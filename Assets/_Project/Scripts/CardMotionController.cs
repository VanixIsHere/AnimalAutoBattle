using UnityEngine;

public class CardMotionController : MonoBehaviour
{
    [Header("Hover Settings")]
    [SerializeField, Tooltip("Distance the card lifts when hovered.")]
    private float hoverLiftDistance = 1.5f;
    [SerializeField, Tooltip("Speed of hover/return lerping.")]
    private float hoverLerpSpeed = 10f;

    [Header("Drag Settings")]
    [SerializeField, Tooltip("Total forward distance the card travels when dragged upward.")]
    private float dragTravelDistance = 5.0f;
    [SerializeField, Tooltip("Maximum pixel distance for screen-based drag calculation.")]
    private float dragMaxScreenDistance = 200f;
    [SerializeField, Tooltip("Maximum rotation applied when dragged upward.")]
    private float dragTiltMaxDegrees = 40f;
    [SerializeField, Tooltip("Speed of return lerping when unhovering.")]
    private float rotationLerpSpeed = 5f;
    [SerializeField, Tooltip("Multiplicative factor applied to the hover lift when starting a drag.")]
    private float dragLiftDistance = 0.5f;
    [SerializeField, Tooltip("Speed of drag lift lerping.")]
    private float dragLiftLerpSpeed = 8f;
    [SerializeField, Tooltip("Distance the card will lower when another card is being dragged.")]
    private float loweredOffsetDistance = 1.4f;

    [Header("Wobble Settings")]
    [SerializeField, Tooltip("How much mouse movement affects wobble.")]
    private float wobbleSensitivity = 0.5f;
    [SerializeField, Tooltip("How quickly wobble slows down.")]
    private float wobbleDampening = 20f;
    [SerializeField, Tooltip("Maximum degrees for wobble.")]
    private float wobbleMaxDegrees = 23f;

    // Runtime state
    private Camera mainCamera;
    private CardState state;

    private Vector3 lastMousePos;
    private Vector3 dragStartMousePos;
    private Vector3 dragStartWorldPos;
    private Quaternion dragStartRotation;

    private Vector2 wobbleAngle = Vector2.zero;
    private Vector2 wobbleVelocity = Vector2.zero;

    private void Awake()
    {
        mainCamera = Camera.main;
        state = GetComponent<CardState>();
    }

    private void Update()
    {
        if (state.IsDragging)
        {
            UpdateDragPosition();
        }
        else if (state.IsHovering)
        {
            UpdateHoverMotion();
        }
        else
        {
            UpdateIdleMotion();
        }
    }

    private void UpdateIdleMotion()
    {
        Vector3 targetPos = state.HandAnchorPosition;

        if (state.IsLowered)
        {
            // Card's thin axis (downward from the face)
            Vector3 loweredOffset = -transform.forward * loweredOffsetDistance;
            targetPos += loweredOffset * loweredOffsetDistance;
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * hoverLerpSpeed); // returnSpeed ~5–10
        transform.rotation = Quaternion.Lerp(transform.rotation, state.HandAnchorRotation, Time.deltaTime * hoverLerpSpeed);

        /*
            transform.position = Vector3.Lerp(transform.position, state.HandAnchorPosition, Time.deltaTime * hoverLerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, state.HandAnchorRotation, Time.deltaTime * hoverLerpSpeed);
        */
    }

    private void UpdateHoverMotion()
    {
        Vector3 liftDir = (mainCamera.transform.position - state.HandAnchorPosition).normalized;
        Vector3 liftedPos = state.HandAnchorPosition + liftDir * hoverLiftDistance;

        transform.position = Vector3.Lerp(transform.position, liftedPos, Time.deltaTime * hoverLerpSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, state.HandAnchorRotation, Time.deltaTime * rotationLerpSpeed);
    }

    public void BeginDrag()
    {
        state.IsDragging = true;
        dragStartMousePos = Input.mousePosition;
        lastMousePos = dragStartMousePos;
        dragStartRotation = transform.rotation;

        // Step 1: Calculate hover position (already lifted)
        Vector3 liftDir = (mainCamera.transform.position - state.HandAnchorPosition).normalized;
        Vector3 hoverPos = state.HandAnchorPosition + liftDir * hoverLiftDistance;
        // Step 2: Add drag lift *on top* of hover
        dragStartWorldPos = hoverPos + liftDir * dragLiftDistance;

        // Reset wobble
        wobbleAngle = Vector2.zero;
        wobbleVelocity = Vector2.zero;
    }

    public void EndDrag()
    {
        state.IsDragging = false;
        wobbleVelocity = Vector2.zero;
        wobbleAngle = Vector2.zero;
        transform.rotation = state.HandAnchorRotation;
    }

    private void UpdateDragPosition()
    {
        Vector3 currentMousePos = Input.mousePosition;

        Vector2 mouseDelta = (Vector2)(currentMousePos - lastMousePos);
        lastMousePos = currentMousePos;

        UpdateWobble(mouseDelta);

        float verticalProgress = GetVerticalDragProgress(currentMousePos);
        float horizontalProgress = GetHorizontalDragProgress(currentMousePos);

        ApplyDragTransform(verticalProgress, horizontalProgress);
        // Optional:
        // if (percentDragged > 0.8f) ShowPlayPreview();
    }

    private void UpdateWobble(Vector2 mouseDelta)
    {
        wobbleVelocity += new Vector2(mouseDelta.y, -mouseDelta.x) * wobbleSensitivity;
        wobbleVelocity = Vector2.Lerp(wobbleVelocity, Vector2.zero, Time.deltaTime * wobbleDampening);

        wobbleAngle = Vector2.Lerp(wobbleAngle, wobbleAngle + wobbleVelocity, Time.deltaTime * 30f);
        wobbleAngle = Vector2.ClampMagnitude(wobbleAngle, wobbleMaxDegrees);
    }

    private float GetVerticalDragProgress(Vector3 currentMousePos)
    {
        float verticalDelta = currentMousePos.y - dragStartMousePos.y;
        return Mathf.Clamp01(verticalDelta / dragMaxScreenDistance);
    }

    private float GetHorizontalDragProgress(Vector3 currentMousePos)
    {
        float horizontalDelta = currentMousePos.x - dragStartMousePos.x;
        return Mathf.Clamp(horizontalDelta / dragMaxScreenDistance, -1f, 1f);
    }

    private void ApplyDragTransform(float forwardProgress, float horizontalProgress)
    {
        Vector3 forwardDir = mainCamera.transform.forward;
        Vector3 sideDir = mainCamera.transform.right;

        Vector3 forwardOffset = forwardDir * (dragTravelDistance * forwardProgress);
        Vector3 sideOffset = sideDir * (dragTravelDistance * 0.5f * horizontalProgress);

        Vector3 desiredPos = dragStartWorldPos + forwardOffset + sideOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * dragLiftLerpSpeed);

        float tiltAngle = forwardProgress * dragTiltMaxDegrees;

        // --- 🔧 Twist Correction (Y rotation only) ---
        float twistCorrectionFactor = Mathf.Clamp01(forwardProgress);
        float originalY = dragStartRotation.eulerAngles.y;
        float correctedY = Mathf.LerpAngle(originalY, mainCamera.transform.rotation.eulerAngles.y, twistCorrectionFactor);

        Quaternion targetRot = Quaternion.Euler(
            dragStartRotation.eulerAngles.x + tiltAngle + wobbleAngle.x,
            correctedY,
            dragStartRotation.eulerAngles.z + wobbleAngle.y
        );

        transform.rotation = targetRot;
    }
    
    private void OnMouseEnter()
    {
        state.IsHovering = true;
    }

    private void OnMouseExit()
    {
        state.IsHovering = false;
    }

    void OnMouseDown()
    {
        BeginDrag();
        state.IsDragging = true;
    }

    void OnMouseUp()
    {
        EndDrag();
        state.IsDragging = false;
        // TODO: add drop logic or snap back to arc
    }
}
