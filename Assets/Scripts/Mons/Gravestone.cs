using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Gravestone : MonoBehaviour, Interactable
{
    //public static Gravestone Instance;
    [SerializeField] GameController gameController;

    public bool isDefeated;

    public event Action<GameObject> OnSpawn;


    public void Interact()
    {
        //begin battle
        gameController.gravestone = GetComponent<Gravestone>();
        OnSpawn += gameController.StartGhostBattle;
        OnSpawn(gameObject);
    }
}
