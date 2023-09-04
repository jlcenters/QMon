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


    //TODO: find based on rarity instead of completely random
    public Monster GetRandomWildMon()
    {
        var wildMon = wildMons[Random.Range(0, wildMons.Count)];
        wildMon.Init();
        return wildMon;
    }



    //if player walks into flower grid bounds, update its reference
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().flowerType = flowerType;
        }
    }
}
