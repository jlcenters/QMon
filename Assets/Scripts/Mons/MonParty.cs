using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonParty : MonoBehaviour
{
    [SerializeField] List<Monster> monsters;



    //Property
    public List<Monster> Monsters
    {
        get
        {
            return monsters;
        }
    }



    private void Start()
    {
        foreach(Monster mon in monsters)
        {
            mon.Init();
        }
    }



    public Monster GetHealthyMon()
    {
        //returns same type where x.hp > 0; will only return the first or default element in list; will return null if all are fainted
        return monsters.Where(x => x.Hp > 0).FirstOrDefault();
    }



    public List<Monster> GetHealthyMons()
    {
        List<Monster> healthyMons = new List<Monster>();
        //returns same type where x.hp > 0
        foreach(Monster mon in monsters)
        {
            if(mon.Hp > 0)
            {
                healthyMons.Add(mon);
            }
        }
        return healthyMons;
    }



    public void AddMon(Monster newMon)
    {
        if(monsters.Count < 6)
        {
            monsters.Add(newMon);
        }
        else
        {
            //transfer to pc
        }
    }



    public void RemoveMon(Monster selectedMon)
    {
        monsters.Remove(selectedMon);
    }



    public void SwapMons(Monster firstSwap, int firstIndex, Monster secondSwap, int secondIndex)
    {
        List<Monster> newMons = new List<Monster>();

        for (int i = 0; i < monsters.Count; i++)
        {
            if (i == firstIndex || i == secondIndex)
            {
                if (i == firstIndex)
                {
                    newMons.Add(secondSwap);
                }
                else
                {
                    newMons.Add(firstSwap);
                }
            }
            else
            {
                newMons.Add(monsters[i]);
            }
        }

        monsters = newMons;
    }
}
