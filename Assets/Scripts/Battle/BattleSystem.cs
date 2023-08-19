using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Specifies which part of the turn sequence you are in
public enum BattleState
{
    Start,
    ActionSelection,
    PartyScreen,
    MoveSelection,
    RunningTurn,
    Busy,
    BattleOver
}

//Checking which Action you have selected in the Action Menu
public enum BattleAction
{
    Move,
    SwitchPokemon,
    UseItem,
    Run
}



//Class
public class BattleSystem : MonoBehaviour
{
    //sprite details
    [SerializeField] BattleSprite playerSprite;

    [SerializeField] BattleSprite enemySprite;

    //UI details
    [SerializeField] BattleDialogueBox dialogueBox;

    //state references (Previous State is nullable)
    BattleState state;
    BattleState? previousState;

    //view references
    int currentAction;
    int currentMove;
    int currentPartyMember;

    //monster select reference
    [SerializeField] PartyScreen partyScreen;

    //event to determine whether or not player won battle
    public event Action<bool> OnBattleOver;

    MonParty playerParty;
    Monster wildMon;



    //Setup
    public void StartBattle(MonParty party, Monster encounter)
     {
        playerParty = party;
        wildMon = encounter;
        StartCoroutine(SetUpBattle());
    }

    public IEnumerator SetUpBattle()
    {
        //display sprites and status HUDs
        playerSprite.Setup(playerParty.GetHealthyMon());
        enemySprite.Setup(wildMon);

        //set up party screen for if player wants to swap out their mons
        partyScreen.Init();

        //assign moves to Move Selector
        dialogueBox.SetMoveNames(playerSprite.Mon.Moves);

        //begin process of displaying dialogue box
        yield return StartCoroutine(dialogueBox.TypeDialogue($"A wild {enemySprite.Mon.MonBase.MonName} appeared!"));

        //wait 1 second before changing to Player Action state
        yield return new WaitForSeconds(1f);
        ActionSelection();
    }



    //State Methods
    void ActionSelection()
    {
        state = BattleState.ActionSelection;

        dialogueBox.SetDialogue("What will you do?");
        dialogueBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
/**/        Debug.Log("opening party screen");
        state = BattleState.PartyScreen;

        partyScreen.SetPartyData(playerParty.Monsters);
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableMoveSelector(true);

        state = BattleState.MoveSelection;
    }

    void BattleOver(bool isWin)
    {
        state = BattleState.BattleOver;

        //resets stat boosts of all mons in player party
        playerParty.Monsters.ForEach(m => m.OnBattleOver());
        OnBattleOver(isWin);
    }



    //Action State Methods
    IEnumerator RunTurns(BattleAction action)
    {
        state = BattleState.RunningTurn;

        //if player chooses to attack, preform moves in order of speed; else, player preforms action first always
        if (action == BattleAction.Move)
        {
            //grab player and enemy choices
            playerSprite.Mon.CurrentMove = playerSprite.Mon.Moves[currentMove];
            GetEnemyAction();

            //check priority of moves
            int playerPriority = playerSprite.Mon.CurrentMove.Base.Priority;
            int enemyPriority = playerSprite.Mon.CurrentMove.Base.Priority;
            bool playerFirst = true;

            //if enemy move has a higher priority (lower number), enemy goes first; else, if their moves share the same priority, highest speed goes first
            //PRIORITY KEY: 0-flee, 1-switch, 2-items, 3-attack
            if(enemyPriority < playerPriority)
            {
                playerFirst = false;
            }
            else if(enemyPriority == playerPriority)
            {
                playerFirst = playerSprite.Mon.MonBase.Speed >= enemySprite.Mon.MonBase.Speed;
            }

            //if the playermon's speed is greater than or equal to the enemy's, player moves first; else, enemy does
            var firstSprite = (playerFirst) ? playerSprite : enemySprite;
            var secondSprite = (playerFirst) ? enemySprite : playerSprite;

            //if secondSprite faints, use this as a reference
            var secondMon = secondSprite.Mon;

            //preform first move; if target party faints, end battle
            yield return RunMove(firstSprite, secondSprite, firstSprite.Mon.CurrentMove);
            yield return RunAfterTurn();

            if (secondMon.Hp > 0)
            {
                //preform second move; if target party faints, end turn
                yield return RunMove(secondSprite, firstSprite, secondSprite.Mon.CurrentMove);
                yield return RunAfterTurn();
            }

        }
        else
        {
            //if action chosen is to switch current mon out, do so
            if(action == BattleAction.SwitchPokemon)
            {
                var selectedMon = playerParty.Monsters[currentPartyMember];
                state = BattleState.Busy;
                yield return SwitchMonster(selectedMon);
            }

            //enemy's turn
            GetEnemyAction();
            yield return RunMove(enemySprite, playerSprite, enemySprite.Mon.CurrentMove);
            if (state == BattleState.BattleOver)
            {
                yield break;
            }
        }
        
        //if battle is not over, begin new turn
        if(state != BattleState.BattleOver)
        {
            ActionSelection();
        }
    }

    void GetEnemyAction()
    {
        enemySprite.Mon.CurrentMove = enemySprite.Mon.GetRandomMove();
    }

    IEnumerator SwitchMonster(Monster newMon)
    {
        bool didPlayerFaint = playerSprite.Mon.Hp <= 0;
        //if the mons swapped out are both healthy, run this dialogue and animation
        if (!didPlayerFaint)
        {
            yield return dialogueBox.TypeDialogue($"Come back, {playerSprite.Mon.MonBase.MonName}!");
            playerSprite.FaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerSprite.Setup(newMon);
        playerSprite.Hud.SetData(newMon);
        dialogueBox.SetMoveNames(newMon.Moves);
        StartCoroutine(dialogueBox.TypeDialogue($"Go, {newMon.MonBase.MonName}!"));
        yield return new WaitForSeconds(1f);

        //if the mon was swapped because of a fainting, check who attacks first; else, enemy attacks next
        yield return null;
        if (didPlayerFaint)
        {
            state = BattleState.RunningTurn;
        }
        else
        {
            yield return null;
        }
    }



    //Move State Methods
    IEnumerator RunMove(BattleSprite srcSprite, BattleSprite targetSprite, Move move)
    {
        //Run dialogue and animations, and reduce PP
        yield return dialogueBox.TypeDialogue($"{srcSprite.Mon.MonBase.MonName} used {move.Base.MoveName}.");
        move.PP--;
        srcSprite.AttackAnimation();
        yield return new WaitForSeconds(0.5f);
        targetSprite.DamageAnimation();

        //if move is a status effect, run effects ienumerator; else, attack
        if(move.Base.Category == MoveCategory.Status)
        {
            yield return RunMoveEffects(move, srcSprite.Mon, targetSprite.Mon);
        }
        else
        {
            var targetDamage = targetSprite.Mon.TakeDamage(move, srcSprite.Mon);

            yield return targetSprite.Hud.UpdateHP();
            yield return ShowDamageDetails(targetDamage);
        }


        //if hp reaches 0, begin faint sequence
        if (targetSprite.Mon.Hp <= 0)
        {
            targetSprite.FaintAnimation();
            yield return dialogueBox.TypeDialogue($"{targetSprite.Mon.MonBase.MonName} fainted!");
            yield return new WaitForSeconds(1f);
            CheckForBattleOver(targetSprite);
        }
    }

    IEnumerator RunMoveEffects(Move move, Monster source, Monster target)
    {
        var effects = move.Base.Effects;
        //make sure boost is not null to prevent error
        if (effects.Boosts != null)
        {
            //if status move target is the enemy, direct towards enemy sprite; else, direct towards source sprite
            if (move.Base.Target == MoveTarget.Foe)
            {
                target.ApplyBoosts(effects.Boosts);
            }
            else
            {
                source.ApplyBoosts(effects.Boosts);
            }

            //Show status changes for source and target, if any
            yield return ShowStatusChanges(source);
            yield return ShowStatusChanges(target);
        }
    }

    void CheckForBattleOver(BattleSprite faintedUnit)
    {
        //if Player Sprite, check party; else, end battle and Player wins
        if (faintedUnit.IsPlayermon)
        {
            var nextMon = playerParty.GetHealthyMon();
            //if there is no healthy mon, end battle and Player loses; else, select next mon to send out
            if (nextMon == null)
            {
                BattleOver(false);
            }
            else
            {
                OpenPartyScreen();
            }
        }
        else
        {
            BattleOver(true);
        }
    }

    IEnumerator ShowStatusChanges(Monster mon)
    {
        while(mon.StatusChanges.Count > 0)
        {
            var message = mon.StatusChanges.Dequeue();
            yield return dialogueBox.TypeDialogue(message);
            yield return new WaitForSeconds(1f);
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

    IEnumerator RunAfterTurn()
    {
        if (state == BattleState.BattleOver)
        {
            yield break;
        }
        //waits until state is back to Running Turn before executing
        yield return new WaitUntil(() => state == BattleState.RunningTurn);
    }



    //Update
    public void HandleUpdate()
    {
        if(state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if(state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if(state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
    }



    //Handle Selection Methods
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
                    MoveSelection();
                }
                else
                {
                    //monsters
                    previousState = BattleState.ActionSelection;
                    OpenPartyScreen();
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
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
            {
                currentMove--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerSprite.Mon.Moves.Count - 2)
            {
                    currentMove += 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
            {
                currentMove -= 2;
            }
        }

        //highlight selection
        dialogueBox.UpdateMoveSelection(currentMove, playerSprite.Mon.Moves[currentMove]);

        //if spacebar pressed, begin Run Turns coroutine; else, if backspace pressed, go back to action selection
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //if the move's PP is at 0, do not execute
            var move = playerSprite.Mon.Moves[currentMove];
            if(move.PP == 0)
            {
                return;
            }

            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            previousState = BattleState.RunningTurn;
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableActionSelector(true);
            dialogueBox.EnableDialogueText(true);
            state = BattleState.ActionSelection;
        }
    }

    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentPartyMember++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentPartyMember--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentPartyMember += 2;

        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentPartyMember -= 2;
        }
        //will check to make sure selection is within range, else it will add/subtract accordingly
        currentPartyMember = Mathf.Clamp(currentPartyMember, 0, playerParty.Monsters.Count - 1);

        //highlight selection
        partyScreen.UpdateMonsterSelection(currentPartyMember);

        //if selected hp is empty or the selected mon is the same as the one currently out, do not switch and send message
        //else, swap out current mon for new mon and begin Enemy Move
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var selectedMember = playerParty.Monsters[currentPartyMember];
            if(selectedMember.Hp <= 0)
            {
                partyScreen.SetMessageText($"{selectedMember.MonBase.MonName} is not able to battle!");
                return;
            }
            else if(selectedMember == playerSprite.Mon)
            {
                    partyScreen.SetMessageText($"{selectedMember.MonBase.MonName} is already out!");
                return;
            }
            else
            {
                    //resetting highlighted selections
                    partyScreen.gameObject.SetActive(false);

                //if the player's mon had fainted, allow player to choose new mon before starting a new turn; else, run turns as usual
                if(previousState == BattleState.RunningTurn)
                {
/**/                    Debug.Log("previous mon had fainted, resetting previousState and starting the SwitchMonster coroutine");
                    previousState = null;
                    StartCoroutine(SwitchMonster(selectedMember));
                }
                else
                {
/**/                    Debug.Log("previous mon did not faint, continue to enemy turn");
                    state = BattleState.Busy;
                    previousState = null;
                    StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
                }
            }
        }
        //if backspace pressed and previous mon did not faint, return to Player Action screen
        else if (Input.GetKeyDown(KeyCode.Backspace) && previousState != BattleState.RunningTurn)
        {
            currentPartyMember = 0;
            partyScreen.UpdateMonsterSelection(currentPartyMember);
            partyScreen.gameObject.SetActive(false);
            dialogueBox.EnableActionSelector(true);
            dialogueBox.EnableDialogueText(true);
            state = BattleState.ActionSelection;
        }
    }



}
