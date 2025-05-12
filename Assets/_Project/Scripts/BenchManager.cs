using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class BenchManager : MonoBehaviour
{
    public int benchSlotCount = 9;
    public float slotSpacing = 1.5f;
    float yOffsetFromGrid = -5.5f;
    public GameObject slotVisualPrefab;
    public GameObject unitInstancePrefab; // Placeholder prefab for unit

    private Transform[] benchSlots;
    private UnitData[] occupiedSlots;

    public Transform gridCenter;

    void Start()
    {
        GameObject centerObj = GameObject.Find("GridCenter");
        if (centerObj == null)
        {
            Debug.LogError("GridCenter not found! Bench will not be positioned correctly");
            return;
        }

        gridCenter = centerObj.transform;
        GenerateBenchSlots();
    } 

    void GenerateBenchSlots()
    {
        benchSlots = new Transform[benchSlotCount];
        occupiedSlots = new UnitData[benchSlotCount];

        float totalWidth = (benchSlotCount - 1) * slotSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < benchSlotCount; i++)
        {
            Vector3 localOffset = new Vector3(startX + i * slotSpacing, 0f, yOffsetFromGrid);
            Vector3 worldPos = gridCenter.position + localOffset;

            GameObject slot = Instantiate(slotVisualPrefab, worldPos, Quaternion.identity, transform);
            slot.name = $"BenchSlot{i}";
            benchSlots[i] = slot.transform;
        }
    }

    public bool TryAddToBench(UnitData unit)
    {
        for (int i = 0; i < benchSlots.Length; i++)
        {
            if (occupiedSlots[i] == null)
            {
                occupiedSlots[i] = unit;
                SpawnUnit(unit, benchSlots[i]);
                return true;
            }
        }

        Debug.Log("Bench is full!");
        return false;
    }

    void SpawnUnit(UnitData unit, Transform slot)
    {
        GameObject instance = Instantiate(unit.unitPrefab, slot.position, Quaternion.identity, slot);
        instance.GetComponent<UnitInstance>()?.Init(unit);
    }
}
