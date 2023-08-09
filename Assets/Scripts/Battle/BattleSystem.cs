using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    //player details
    [SerializeField] BattleSprite playerSprite;
    [SerializeField] BattleHud playerHud;
    //enemy details
    [SerializeField] BattleSprite enemySprite;
    [SerializeField] BattleHud enemyHud;

    [SerializeField] BattleDialogueBox dialogueBox;

    private void Start()
     {
         SetUpBattle();
     }

    public void SetUpBattle()
    {
        playerSprite.Setup();
        playerHud.SetData(playerSprite.Mon);
        enemySprite.Setup();
        enemyHud.SetData(enemySprite.Mon);
        dialogueBox.SetDialogue($"A wild {enemySprite.Mon.MonBase.MonName} appeared!");
    }
}
