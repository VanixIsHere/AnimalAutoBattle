using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using System.Linq.Expressions;

public class DeckManager : MonoBehaviour
{
    public List<UnitData> allUnits;
    public GameObject cardPrefab;
    public Transform handParent; // 3D empty parent in world space
    public int handSize = 5;

    private List<UnitData> currentHand = new();

    public UnitPoolManager poolManager;

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

        GenerateHand();
    }

    public void GenerateHand()
    {
        ClearHand();

        for (int i = 0; i < handSize; i++)
        {
            var unit = allUnits[Random.Range(0, allUnits.Count)];
            currentHand.Add(unit);
            CreateCard(unit, i);
        }
    }

    void CreateCard(UnitData unit, int handSlot)
    {
        float cardSeparationOffset = 1.5f;
        float transformOffset = handSlot * cardSeparationOffset;
        Vector3 spawnPosition = new Vector3(transformOffset, 10f, 0f);
        GameObject card = Instantiate(cardPrefab, spawnPosition, Quaternion.identity, handParent);
        Debug.LogWarning($"Drew card: {unit.unitName}");

        Card3DView cardView = card.GetComponent<Card3DView>();
        if (cardView != null)
        {
            cardView.Init(unit);
        }
        else {
            Debug.LogError("DeckManager: Spawned card prefab is missing Card3DView component.");
        }
    }

    void ClearHand()
    {
        foreach (Transform child in handParent)
        {
            Destroy(child.gameObject);
        }
        currentHand.Clear();
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
