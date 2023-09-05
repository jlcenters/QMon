using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class BattleHud : MonoBehaviour
{
    
    [SerializeField] TMP_Text nameTxt;
    [SerializeField] TMP_Text lvTxt;
    [SerializeField] TMP_Text hpTxt;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject xpBar;

    Monster monster;

    public void SetData(Monster mon)
    {
        monster = mon;

        nameTxt.text = mon.MonBase.MonName;
        SetLevel();
        hpBar.SetHP((float)mon.Hp / mon.MaxHp);
        SetXP();
        //TODO: decrement
        hpTxt.text = mon.Hp + "/" + mon.MaxHp;
    }

    public void SetXP()
    {
        //if xp bar not available, do not set xp
        if(xpBar == null)
        {
            return;
        }

        float normal = GetXPNormalized();
        xpBar.transform.localScale = new Vector3(normal, 1, 1);
    }

    public void SetLevel()
    {
        lvTxt.text = $"lv {monster.Level}";
    }

    public void SetHP()
    {
        hpBar.SetHP((float)monster.Hp / monster.MaxHp);
        hpTxt.text = monster.Hp + "/" + monster.MaxHp;
    }



    //Update Methods
    //false by default
    public IEnumerator UpdateXP(bool reset=false)
    {
        //if xp bar not available, do not set xp
        if (xpBar == null)
        {
            yield break;
        }

        //set local position of xp bar if leveled up
        if (reset)
        {
            xpBar.transform.localScale = new Vector3(0, 1, 1);
        }

        float normal = GetXPNormalized();

        yield return xpBar.transform.DOScaleX(normal, 1.5f).WaitForCompletion();
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHPSmooth((float)monster.Hp / monster.MaxHp);
        hpTxt.text = monster.Hp + "/" + monster.MaxHp;
    }



    float GetXPNormalized()
    {
        int curLvlXp = monster.MonBase.GetXpForLevel(monster.Level);
        int nextLvlXp = monster.MonBase.GetXpForLevel(monster.Level + 1);

        float normXp = (float)(monster.Xp - curLvlXp) / (nextLvlXp - curLvlXp);
        return Mathf.Clamp01(normXp);
    }
}
