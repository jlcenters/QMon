using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleSprite : MonoBehaviour
{
    /*[SerializeField] MonsterBase monBase;
    [SerializeField] int level;*/
    [SerializeField] bool isPlayermon;
    public float animDuration;

    //Animation References
    Image img;
    Vector3 originalPos;
    Color originalColor;

    private void Awake()
    {
        img = GetComponent<Image>();
        originalPos = img.transform.localPosition;
        originalColor = img.color;
    }

    public bool IsPlayermon
    {
        get
        {
            return isPlayermon;
        }
    }
    public Monster Mon { get; set; }

    public void Setup(Monster mon)
    {
        Mon = mon;

        if (isPlayermon)
        {
            img.sprite = Mon.MonBase.BackSprite;
        }
        else
        {
            img.sprite = Mon.MonBase.FrontSprite;
        }
        img.color = originalColor;
        EnterAnimation();

    }

    public void EnterAnimation()
    {
        if (isPlayermon)
        {
            img.transform.localPosition = new Vector3(-750, originalPos.y);
        }
        else
        {
            img.transform.localPosition = new Vector3(750, originalPos.y);
        }

        img.transform.DOLocalMoveX(originalPos.x, 0.75f);
    }

    public void AttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayermon)
        {
            sequence.Append(img.transform.DOLocalMoveX(originalPos.x + 50, 0.15f));
        }
        else
        {
            sequence.Append(img.transform.DOLocalMoveX(originalPos.x - 50, 0.15f));
        }

        sequence.Append(img.transform.DOLocalMoveX(originalPos.x, 0.15f));
    }

    public void DamageAnimation()
    {
        //TODO: switch for beingDamaged bool for premade animation

        var sequence = DOTween.Sequence();
        sequence.Append(img.DOColor(Color.gray, 0.08f));
        sequence.Append(img.DOColor(originalColor, 0.08f));
        sequence.Append(img.DOColor(Color.gray, 0.08f));
        sequence.Append(img.DOColor(originalColor, 0.08f));
    }

    public void FaintAnimation()
    {
        var sequence = DOTween.Sequence();

        sequence.Append(img.transform.DOLocalMoveY(originalPos.y - 150, 0.5f));
        sequence.Join(img.DOFade(0f, 0.25f));
    }
}
