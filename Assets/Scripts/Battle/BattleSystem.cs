using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Specifies which part of the turn sequence you are in
public enum BattleState
{
    Start,
    PlayerAction,
    PlayerMove,
    EnemyMove,
    Busy
}



//class
public class BattleSystem : MonoBehaviour
{
    //player details
    [SerializeField] BattleSprite playerSprite;
    [SerializeField] BattleHud playerHud;

    //enemy details
    [SerializeField] BattleSprite enemySprite;
    [SerializeField] BattleHud enemyHud;

    //UI details
    [SerializeField] BattleDialogueBox dialogueBox;

    //state reference
    BattleState state;

    //view references
    int currentAction;
    int currentMove;

    //bool to determine whether or not player won battle
    public event Action<bool> OnBattleOver;

    //setup
    public void StartBattle()
     {
         StartCoroutine(SetUpBattle());
    }
    public IEnumerator SetUpBattle()
    {
        //display sprites and status HUDs
        playerSprite.Setup();
        playerHud.SetData(playerSprite.Mon);
        enemySprite.Setup();
        enemyHud.SetData(enemySprite.Mon);

        //assign moves to Move Selector
        dialogueBox.SetMoveNames(playerSprite.Mon.Moves);

        //begin process of displaying dialogue box
        yield return StartCoroutine(dialogueBox.TypeDialogue($"A wild {enemySprite.Mon.MonBase.MonName} appeared!"));

        //wait 1 second before changing to Player Action state
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }


    //State Methods
    void PlayerAction()
    {
        state = BattleState.PlayerAction;

        StartCoroutine(dialogueBox.TypeDialogue("What will you do?"));
        dialogueBox.EnableActionSelector(true);
    }
    void PlayerMove()
    {
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableMoveSelector(true);

        state = BattleState.PlayerMove;
        Debug.Log("set move stuffs view");
    }

    IEnumerator PreformPlayerMove()
    {
        state = BattleState.Busy;

        var move = playerSprite.Mon.Moves[currentMove];
        move.PP--;
        yield return dialogueBox.TypeDialogue($"{playerSprite.Mon.MonBase.MonName} used {move.Base.MoveName}.");
        playerSprite.AttackAnimation();
        yield return new WaitForSeconds(0.5f);
        enemySprite.DamageAnimation();


        var enemyDamage = enemySprite.Mon.TakeDamage(move, playerSprite.Mon);
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(enemyDamage);
        //if enemy sprite fainted from the attack, display faint sequence; else, enemy's turn
        if (enemyDamage.Fainted)
        {
            enemySprite.FaintAnimation();
            yield return dialogueBox.TypeDialogue($"{enemySprite.Mon.MonBase.MonName} fainted!");

            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = enemySprite.Mon.GetRandomMove();

        yield return dialogueBox.TypeDialogue($"{enemySprite.Mon.MonBase.MonName} used {move.Base.MoveName}.");
        move.PP--;
        enemySprite.AttackAnimation();
        yield return new WaitForSeconds(0.5f);
        playerSprite.DamageAnimation();


        var playerDamage = playerSprite.Mon.TakeDamage(move, enemySprite.Mon);

        yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(playerDamage);

        //if player sprite fainted from the attack, display faint sequence; else, player action
        if (playerDamage.Fainted)
        {
            playerSprite.FaintAnimation();
            yield return dialogueBox.TypeDialogue($"{playerSprite.Mon.MonBase.MonName} fainted!");

            yield return new WaitForSeconds(2f);
            OnBattleOver(false);
        }
        else
        {
            PlayerAction();
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails details)
    {
        if(details.Critical > 1f)
        {
            yield return dialogueBox.TypeDialogue("A Critical Hit!");
            yield return new WaitForSeconds(1f);
        }
        if(details.TypeEffectiveness > 1f)
        {
            yield return dialogueBox.TypeDialogue("It's super effective!");
        }
        if(details.TypeEffectiveness < 1f)
        {
            yield return dialogueBox.TypeDialogue("It's not very effective...");
        }
        yield return new WaitForSeconds(1f);
    }

    //check current state
    public void HandleUpdate()
    {
        if(state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if(state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    //Handle Selection Boxes
    void HandleActionSelection()
    {
        //determine which action the player is about to select
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentAction < 4)
            {
                currentAction++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentAction > 0)
            {
                currentAction--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(currentAction < 3)
            {
                currentAction += 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(currentAction > 1)
            {
                currentAction -= 2;
            }
        }

        //highlight selection
        dialogueBox.UpdateActionSelection(currentAction);

        //select
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(currentAction < 2)
            {
                if (currentAction == 0)
                {
                    //fight
                    PlayerMove();
                    Debug.Log("Fight!");

                }
                else
                {
                    //monsters
                    Debug.Log("swap mons");
                }
            }
            else
            {
                if (currentAction == 2)
                {
                    //bag
                    Debug.Log("check bag");

                }
                else
                {
                    //run
                    Debug.Log("Got away safely!");

                }
            }
            
        }
    }

    void HandleMoveSelection()
    {
        //determine which move the player is about to select
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerSprite.Mon.Moves.Count - 1)
            {
                currentMove++;
                Debug.Log("moved right");
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
            {
                currentMove--;
                Debug.Log("moved left");
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerSprite.Mon.Moves.Count - 2)
            {
                currentMove += 2;
                Debug.Log("moved down");
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 1)
            {
                currentMove -= 2;
                Debug.Log("moved up");
            }
        }

        //highlight selection
        dialogueBox.UpdateMoveSelection(currentMove, playerSprite.Mon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Move selected!");
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            StartCoroutine(PreformPlayerMove());
        }
    }
}
