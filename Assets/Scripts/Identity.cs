using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Identitity")]
public class Identity : ScriptableObject
{
    public Sprite Face;
    public string unitName;
    public enum TypeOfUnit { Paladin, Warrior, Barbarian, Thief };
    public TypeOfUnit unitBodyType;
}