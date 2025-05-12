using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitRegistry", menuName = "AnimalAutoBattle/UnitRegistry")]
public class UnitRegistry : ScriptableObject
{
    public List<UnitData> units;
}