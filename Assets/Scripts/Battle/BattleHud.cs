using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleHud : MonoBehaviour
{
    
    [SerializeField] TMP_Text nameTxt;
    [SerializeField] TMP_Text lvTxt;
    [SerializeField] TMP_Text hpTxt;
    [SerializeField] HPBar hpBar;
    public int currHp;

    public void SetData(Monster mon)
    {
        nameTxt.text = mon.MonBase.MonName;
        lvTxt.text = "lv " + mon.Level;
        currHp = mon.Hp;
        hpBar.SetHP((float)currHp / (float)mon.MaxHp);
        //TODO: decrement
        hpTxt.text = currHp + "/" + mon.Hp;
        Debug.Log("lv " + mon.Level + " current HP: " + currHp + "; normalized for HP bar: " + (float)currHp / (float)mon.MaxHp);



    }

}
