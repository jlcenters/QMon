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

    Monster monster;

    public void SetData(Monster mon)
    {
        monster = mon;

        nameTxt.text = mon.MonBase.MonName;
        lvTxt.text = "lv " + mon.Level;
        hpBar.SetHP((float)mon.Hp / (float)mon.MaxHp);
        //TODO: decrement
        hpTxt.text = mon.Hp + "/" + mon.MaxHp;
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHPSmooth((float)monster.Hp / monster.MaxHp);
        hpTxt.text = monster.Hp + "/" + monster.MaxHp;
    }
}
