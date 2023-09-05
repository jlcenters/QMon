using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum FlowerType
{
    None,
    Fire,
    FireHard,
    Water,
    WaterHard,
    Grass,
    GrassHard,
    Normal,
    Ghost,
    GhostHard,
    Special,
    Boss
}



public class MapArea : MonoBehaviour
{
    [SerializeField] List<Monster> wildMons;

    public FlowerType flowerType;



    public List<Monster> WildMons { get { return wildMons; } set { wildMons = value; } }
    public int EncounterIndex { get; private set; }


    //TODO: find based on rarity instead of completely random
    public Monster GetRandomWildMon()
    {
        //store list index in case of boss
        EncounterIndex = Random.Range(0, wildMons.Count);

        var wildMon = wildMons[EncounterIndex];
        wildMon.Init();
        return wildMon;
    }



    //if player walks into flower grid bounds, update its reference
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().flowerType = flowerType;

            //if potentially encountering a boss, 
            if (flowerType == FlowerType.Boss)
            {

            }
        }
    }
}
