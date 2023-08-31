using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum FlowerType
{
    None,
    Fire,
    Water,
    Grass,
    Normal,
    Ghost,
    Special
}



public class MapArea : MonoBehaviour
{
    [SerializeField] List<Monster> wildMons;
    [SerializeField] BoxCollider2D bc;

    public FlowerType flowerType;


    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
    }

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
