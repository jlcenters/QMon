using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleSprite : MonoBehaviour
{
    [SerializeField] bool isPlayermon;
    [SerializeField] float enemonSpriteScale = 2.5f;
    [SerializeField] BattleHud hud;

    //public float animDuration;

    //Animation References
    Image img;
    Vector3 originalPos;
    Color originalColor;



    public bool IsPlayermon
    {
        get
        {
            return isPlayermon;
        }
    }

    public BattleHud Hud
    { 
        get
        {
            return hud;
        }
    }

    public Monster Mon { get; set; }



    private void Awake()
    {
        img = GetComponent<Image>();
        originalPos = img.transform.localPosition;
        originalColor = img.color;
    }

    public void Setup(Monster mon)
    {
        Mon = mon;

        //if player, scale x5; else, scale x2.5
        if (isPlayermon)
        {
            img.sprite = Mon.MonBase.BackSprite;
            transform.localScale = new Vector3(5f, 5f);
        }
        else
        {
            img.sprite = Mon.MonBase.FrontSprite;
            transform.localScale = new Vector3(enemonSpriteScale, enemonSpriteScale);
        }

        hud.SetData(Mon);

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

    public IEnumerator CatchingAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(img.DOFade(0, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y + 50f, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }

    public IEnumerator CatchingFailAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(img.DOFade(1, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(enemonSpriteScale, enemonSpriteScale, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }
}
