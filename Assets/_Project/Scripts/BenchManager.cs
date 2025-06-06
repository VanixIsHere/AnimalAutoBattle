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
    private UnitInstance[] occupiedInstances;

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
        occupiedInstances = new UnitInstance[benchSlotCount];

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
                occupiedInstances[i] = SpawnUnit(unit, benchSlots[i]);
                return true;
            }
        }

        Debug.Log("Bench is full!");
        return false;
    }

    public bool CanAdd(UnitData unit)
    {
        // Check for empty slot
        for (int i = 0; i < occupiedSlots.Length; i++)
        {
            if (occupiedSlots[i] == null)
            {
                return true;
            }
        }

        // Bench full - check for potential merge (two level 1 units of same type)
        int count = 0;
        for (int i = 0; i < occupiedSlots.Length; i++)
        {
            if (occupiedSlots[i] == unit && occupiedInstances[i] != null && occupiedInstances[i].level == 1)
            {
                count++;
                if (count >= 2)
                {
                    return true;
                }
            }
        }

        return false;
    }

    UnitInstance SpawnUnit(UnitData unit, Transform slot)
    {
        GameObject instanceObj = Instantiate(unit.unitPrefab, slot.position, Quaternion.identity, slot);
        UnitInstance inst = instanceObj.GetComponent<UnitInstance>();
        if (inst != null)
        {
            inst.Init(unit);
        }
        return inst;
    }
}
