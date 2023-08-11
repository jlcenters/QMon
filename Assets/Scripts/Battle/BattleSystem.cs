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


    //setup
    private void Start()
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
        state = BattleState.PlayerMove;

        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableMoveSelector(true);
    }


    //check current state
    private void Update()
    {
        if(state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        if(state == BattleState.PlayerMove)
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
    }
}
