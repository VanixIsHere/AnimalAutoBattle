using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "AnimalAutoBattle/Unit")]
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
}
