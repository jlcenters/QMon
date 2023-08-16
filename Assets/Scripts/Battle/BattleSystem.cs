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
    PreformMove,
    Busy,
    BattleOver
}



//Class
public class BattleSystem : MonoBehaviour
{
    //sprite details
    [SerializeField] BattleSprite playerSprite;
    //[SerializeField] BattleHud playerHud;

    [SerializeField] BattleSprite enemySprite;
    //[SerializeField] BattleHud enemyHud;

    //UI details
    [SerializeField] BattleDialogueBox dialogueBox;

    //state reference
    BattleState state;

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

    IEnumerator PlayerMove()
    {
        state = BattleState.PreformMove;

        //get move and preform Run Move method
        var move = playerSprite.Mon.Moves[currentMove];
        yield return RunMove(playerSprite, enemySprite, move);

        //will only start the next state if the battle mantained its Preform Move state (did not become Battle Over)
        if(state == BattleState.PreformMove)
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.PreformMove;

        //get move and preform Run Move method
        var move = enemySprite.Mon.GetRandomMove();
        yield return RunMove(enemySprite, playerSprite, move);

        //will only start the next state if the battle mantained its Preform Move state (did not become Battle Over)
        if (state == BattleState.PreformMove)
        {
            ActionSelection();
        }
    }

    void BattleOver(bool isWin)
    {
        state = BattleState.BattleOver;

        //resets stat boosts of all mons in player party
        playerParty.Monsters.ForEach(m => m.OnBattleOver());
        OnBattleOver(isWin);
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

        //if move is a status effect, boost stats; else, attack
        if(move.Base.Category == MoveCategory.Status)
        {
            var effects = move.Base.Effects;
            //make sure boost is not null to prevent error
            if(effects.Boosts != null)
            {
                //if status move target is the enemy, direct towards enemy sprite; else, direct towards source sprite
                if (move.Base.Target == MoveTarget.Foe)
                {
                    targetSprite.Mon.ApplyBoosts(effects.Boosts);
                }
                else
                {
                    srcSprite.Mon.ApplyBoosts(effects.Boosts);
                }

                //Show status changes for source and target, if any
                yield return ShowStatusChanges(srcSprite.Mon);
                yield return ShowStatusChanges(targetSprite.Mon);
            }
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
                Debug.Log("current state: " + state.ToString());
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
                    MoveSelection();
                }
                else
                {
                    //monsters
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
            if (currentMove < this.playerSprite.Mon.Moves.Count - 1)
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
            if (currentMove < this.playerSprite.Mon.Moves.Count - 2)
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
            dialogueBox.UpdateMoveSelection(currentMove, this.playerSprite.Mon.Moves[currentMove]);

        //if spacebar pressed, preform player move; else, if backspace pressed, go back to action selection
        if (Input.GetKeyDown(KeyCode.Space))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            StartCoroutine(PlayerMove());
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
        //otherwise, swap out current mon for new mon and begin Enemy Move
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var selectedMember = playerParty.Monsters[currentPartyMember];
            if(selectedMember.Hp <= 0)
            {
                partyScreen.SetMessageText($"{selectedMember.MonBase.MonName} is not able to battle!");
                return;
            }
            else if(selectedMember == this.playerSprite.Mon)
            {
                    partyScreen.SetMessageText($"{selectedMember.MonBase.MonName} is already out!");
                return;
            }
            else
            {
                    partyScreen.gameObject.SetActive(false);
                    currentAction = 0;
                    dialogueBox.UpdateActionSelection(currentAction);
                    currentPartyMember = 0;
                    partyScreen.UpdateMonsterSelection(currentPartyMember);
                    state = BattleState.Busy;
                    StartCoroutine(SwitchMonster(selectedMember));
            }
        }
        //if backspace pressed, return to Player Action screen
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            currentPartyMember = 0;
            partyScreen.UpdateMonsterSelection(currentPartyMember);
            partyScreen.gameObject.SetActive(false);
            dialogueBox.EnableActionSelector(true);
            dialogueBox.EnableDialogueText(true);
            state = BattleState.ActionSelection;
        }
    }



    IEnumerator SwitchMonster(Monster newMon)
    {
        bool swappingHealthy = this.playerSprite.Mon.Hp > 0;
        //if the mons swapped out are both healthy, run this dialogue and animation
        if (swappingHealthy)
        {
            yield return dialogueBox.TypeDialogue($"Come back, {this.playerSprite.Mon.MonBase.MonName}!");
            this.playerSprite.FaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        //StartCoroutine(dialogueBox.TypeDialogue($"Come back, {playerSprite.Mon.MonBase.MonName}!"));
        this.playerSprite.Setup(newMon);
        playerSprite.Hud.SetData(newMon);
        dialogueBox.SetMoveNames(newMon.Moves);
        StartCoroutine(dialogueBox.TypeDialogue($"Go, {newMon.MonBase.MonName}!"));
        yield return new WaitForSeconds(1f);

        //if the mons swapped out are both healthy, run Enemy Move
        if (swappingHealthy)
        {
            StartCoroutine(EnemyMove());
        }
        else
        {
            ActionSelection();
        }
    }
}
