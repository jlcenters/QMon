using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Monster> wildMons;


    //TODO: find based on rarity instead of completely random
    public Monster GetRandomWildMon()
    {
        var wildMon = wildMons[Random.Range(0, wildMons.Count)];
        wildMon.Init();
        return wildMon;
    }


}
