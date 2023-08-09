using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSprite : MonoBehaviour
{
    [SerializeField] MonsterBase monBase;
    [SerializeField] int level;
    [SerializeField] bool isPlayermon;

    public Monster Mon { get; set; }

    public void Setup()
    {
        Mon = new Monster(monBase, level);

        if (isPlayermon)
        {
            GetComponent<Image>().sprite = Mon.MonBase.BackSprite;
        }
        else
        {
            GetComponent<Image>().sprite = Mon.MonBase.FrontSprite;
        }
    }


}
