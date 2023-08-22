using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartyScreen : MonoBehaviour
{
   [SerializeField] TMP_Text messageTxt;
   [SerializeField] PartyMemberUI[] members;

   List<Monster> monsters;

    public void Init()
    {
        members = GetComponentsInChildren<PartyMemberUI>(true);
    }

    public void SetPartyData(List<Monster> mons)
    {
        monsters = mons;

        for(int i = 0; i < members.Length; i++)
        {
            if(i < mons.Count)
            {
                members[i].gameObject.SetActive(true);
                members[i].SetData(mons[i]);
            }
            else
            {
                members[i].gameObject.SetActive(false);
            }
        }

        messageTxt.text = "Choose a Monster.";
    }

    public void UpdateMonsterSelection(int selectedMon)
    {
        //highlight selection
        for(int i = 0; i < monsters.Count; i++)
        {
            if(i == selectedMon)
            {
                members[i].SetSelected(true);
            }
            else
            {
                members[i].SetSelected(false);
            }
        }
    }

    public void SetMessageText(string message)
    {
        messageTxt.text = message;
    }
}
