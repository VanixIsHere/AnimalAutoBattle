using System;
using UnityEngine;
using AnimalAutoBattle.Units;

[CreateAssetMenu(fileName = "UnitData", menuName = "Alpha Instinct/Unit")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public Texture cardArtwork;
    public GameObject unitPrefab;

    [Header("Stats")]
    public int baseHealth;
    public int baseAttack;
    public float baseSpeed;
    public int baseRange;
    public int cost;
    [Range(1, 5)]
    public int rarity = 1;

    [Header("Class/Role")]
    public UnitRole role; // enum (e.g. Tank, Flanker, Healer)
    public UnitOrigin origin; // enum or country if you want synergy


    // 💡 Derived dynamically — do not try to assign during field init!
    public UnitClass UnitClass => role.GetClass();
}
