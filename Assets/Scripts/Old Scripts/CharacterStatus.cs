using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Adding Scriptable Object to the Create Menu
[CreateAssetMenu(fileName = "PlayerStatusData", menuName = "StatusObjects/PlayerData", order = 1)]
public class CharacterStatus : ScriptableObject
{
    //player's position in overworld, all of player's pokemon,
    //player's inventory, and player's wallet
    //will be stored in this object and can be carried around between scenes
    public float[] position = new float[2];
    public List<Monster> party = new List<Monster>();
    public List<GameObject> inventory = new List<GameObject>();
    public int money;


    //TODO: reference active mon
}
