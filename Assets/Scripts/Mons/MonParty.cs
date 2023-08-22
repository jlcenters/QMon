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
}
