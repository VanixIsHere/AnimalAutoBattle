using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public List<UnitData> unitPool; // List of all possible units
    public Transform handUIParent;  // Parent for card visuals
    public GameObject cardPrefab;
    public int gold = 10;
    public int handSize = 5;
    private List<UnitData> currentHand = new();

    public void GenerateHand()
    {
        ClearHand();

        for (int i = 0; i < handSize; i++)
        {
            UnitData unit = unitPool[Random.Range(0, unitPool.Count)];
            currentHand.Add(unit);
            InstantiateCard(unit);
        }
    }

    public void Reroll()
    {
        if (gold >= 2)
        {
            gold -= 2;
            GenerateHand();
        }
    }

    void InstantiateCard(UnitData data)
    {
        GameObject card = Instantiate(cardPrefab, handUIParent);
        card.GetComponent<CardUI>().Init(data);
    }

    void ClearHand()
    {
        foreach (Transform child in handUIParent)
            Destroy(child.gameObject);
        currentHand.Clear();
    }
}
