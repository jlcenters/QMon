using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartyScreen : MonoBehaviour
{
   [SerializeField] TMP_Text messageTxt;
   [SerializeField] PartyMemberUI[] members;


    public void Init()
    {
        members = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Monster> mons)
    {
        for(int i = 0; i < members.Length; i++)
        {
            if(i < mons.Count)
            {
                members[i].SetData(mons[i]);
            }
            else
            {
                members[i].gameObject.SetActive(false);
            }
        }

        messageTxt.text = "Choose a Monster.";
    }
}
