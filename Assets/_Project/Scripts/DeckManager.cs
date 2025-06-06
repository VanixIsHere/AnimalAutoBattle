using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class DeckManager : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private Camera cardCameraPrefab;
    public List<UnitData> allUnits;
    public GameObject cardPrefab;
    public Transform handParent; // 3D empty parent in world space
    public int handSize = 5;

    private List<UnitData> currentHand = new();
    private List<GameObject> currentCardObjects = new();

    public UnitPoolManager poolManager;

    private CardHandDisplayer cardHandDisplayer;

    private void Awake()
    {
        mainCamera = Camera.main;
        cardHandDisplayer = GetComponent<CardHandDisplayer>();
    }

    void Start()
    {
        if (poolManager == null || poolManager.unitRegistry == null)
        {
            Debug.LogError("DeckManager: Missing UnitPoolManager or UnitRegistry.");
            return;
        }

        allUnits = new List<UnitData>(poolManager.unitRegistry.units);

        if (allUnits.Count == 0)
        {
            Debug.LogError("DeckManager: No units found in registry.");
            return;
        }

        InstantiateCardCameras();

        GenerateHand();
    }

    private void InstantiateCardCameras()
    {
        UniversalAdditionalCameraData urpMain = mainCamera.GetUniversalAdditionalCameraData();

        // Generate Card Cameras based on the total hand size, for proper render stacks
        for (int i = 0; i < handSize; i++)
        {
            Camera newCardCam = Instantiate(cardCameraPrefab);
            newCardCam.name = $"CardCamera{i}";
            newCardCam.transform.SetPositionAndRotation(mainCamera.transform.position, mainCamera.transform.rotation);
            newCardCam.transform.SetParent(mainCamera.transform);

            int layerDesignatedForCamera = LayerMask.NameToLayer($"Card{i}");
            newCardCam.cullingMask = 1 << layerDesignatedForCamera;
            newCardCam.depth = mainCamera.depth + i + 1;

            urpMain.cameraStack.Add(newCardCam);
            UniversalAdditionalCameraData urpCardCam = newCardCam.GetUniversalAdditionalCameraData();
            urpCardCam.renderType = CameraRenderType.Overlay;
        }

        // Generate one extra card camera for 'focused' cards (hovered/dragged)
        Camera focusCam = Instantiate(cardCameraPrefab);
        focusCam.name = "CardCameraFocused";
        focusCam.transform.SetPositionAndRotation(mainCamera.transform.position, mainCamera.transform.rotation);
        focusCam.transform.SetParent(mainCamera.transform);

        int focusLayer = LayerMask.NameToLayer("CardFocused");
        focusCam.cullingMask = 1 << focusLayer;
        focusCam.depth = mainCamera.depth + handSize + 1 + 100;
        
        urpMain.cameraStack.Add(focusCam);
        UniversalAdditionalCameraData urpFocusCam = focusCam.GetUniversalAdditionalCameraData();
        urpFocusCam.renderType = CameraRenderType.Overlay;
    }

    public bool IsCardBeingDragged(GameObject exceptCard = null)
    {
        for (int i = 0; i < currentCardObjects.Count; i++)
        {
            if (exceptCard != null && exceptCard == currentCardObjects[i])
            {
                continue;
            }
            var state = currentCardObjects[i].GetComponent<CardState>();
            if (state && state.IsDragging)
            {
                return true;
            }
        }
        return false;
    }

    public void GenerateHand()
    {
        ClearHand();

        for (int i = 0; i < handSize; i++)
        {
            var unit = allUnits[Random.Range(0, allUnits.Count)];
            currentHand.Add(unit);
            GameObject card = CreateCard(unit, i);
            currentCardObjects.Add(card);
        }

        cardHandDisplayer.SetCards(currentCardObjects);
    }

    GameObject CreateCard(UnitData unit, int handSlot)
    {
        float cardSeparationOffset = 1.5f;
        float transformOffset = handSlot * cardSeparationOffset;
        Vector3 spawnPosition = new Vector3(transformOffset, 10f, 0f);
        GameObject card = Instantiate(cardPrefab, spawnPosition, Quaternion.identity);
        card.GetComponent<CardMotionController>().SetDeckManager(this);
        card.layer = LayerMask.NameToLayer($"Card{handSlot}");

        LayerUtils.SetLayerRecursive(card, card.layer);

        Card3DView cardView = card.GetComponent<Card3DView>();
        if (cardView != null)
        {
            cardView.Init(unit);
        }
        else
        {
            Debug.LogError("DeckManager: Spawned card prefab is missing Card3DView component.");
        }
        return card;
    }

    void ClearHand()
    {
        foreach (GameObject card in currentCardObjects)
        {
            Destroy(card.gameObject);
        }
        currentHand.Clear();
        currentCardObjects.Clear();
    }

    public void Reroll()
    {
        if (GameManager.Instance.gold >= 2)
        {
            GameManager.Instance.gold -= 2;
            GenerateHand();
            GameManager.Instance.UpdateUI();
        }
    }
}
