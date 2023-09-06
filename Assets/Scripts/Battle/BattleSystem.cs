using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



//Specifies which part of the turn sequence you are in
public enum BattleState
{
    Start,
    ActionSelection,
    PartyScreen,
    MoveSelection,
    RunningTurn,
    ForgetMove,
    Busy,
    BattleOver
}

//Checking which Action you have selected in the Action Menu
public enum BattleAction
{
    Move,
    SwitchMon,
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

    //qball reference
    [SerializeField] GameObject ball;

    //move forgetting menu
    [SerializeField] MoveSelectionUI moveSelectUI;

    //boss encounter checks
    public bool bossBattle;
    public bool defeatedBoss;
    public bool caughtBoss;

    //ghost encounter checks
    public bool ghostBattle;
    public bool defeatedGhost;

    //event to determine whether or not player won battle
    public event Action<bool> OnBattleOver;

    public int QballCount { get; private set; }
    MonParty playerParty;
    Monster wildMon;
    int escapeAttempts;
    MoveBase moveToLearn;



    //Setup
    public void StartBattle(MonParty party, Monster encounter, int qballAmount)
     {
        playerParty = party;
        wildMon = encounter;
        QballCount = qballAmount;
        escapeAttempts = 0;
        bossBattle = encounter.MonBase.IsBoss;
        caughtBoss = false;
        defeatedBoss = false;
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

        //set catch ui
        //dialogueBox.UpdateQballText("Catch");

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
        dialogueBox.EnableActionSelector(true, QballCount);
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;

        partyScreen.SetPartyData(playerParty.Monsters);
        partyScreen.SetMessageText("Choose a monster");
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        dialogueBox.EnableActionSelector(false, QballCount);
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

        //if player chooses to attack, preform moves in order of speed; else, player preforms their other action
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
            if (enemyPriority < playerPriority)
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
            //go to action coroutine which is determined by state
            if(action == BattleAction.SwitchMon)
            {
                var selectedMon = playerParty.Monsters[currentPartyMember];
                state = BattleState.Busy;
                yield return SwitchMonster(selectedMon);
            }
            else if(action == BattleAction.UseItem)
            {
                dialogueBox.EnableActionSelector(false, QballCount);
                yield return ThrowBall();
            }
            else if(action == BattleAction.Run)
            {
                yield return TryToEscape();
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

    IEnumerator ThrowBall()
    {
        state = BattleState.Busy;

        QballCount--;
        yield return dialogueBox.TypeDialogue("Used QBall!");

        var catchObj = Instantiate(ball, playerSprite.transform.position - new Vector3(2, 0), Quaternion.identity);
        var catchObjSprite = catchObj.GetComponent<SpriteRenderer>();

        //animate ball being thrown and mon going into ball
        yield return catchObjSprite.transform.DOJump(enemySprite.transform.position + new Vector3(0, 2), 1f, 1, 1f).WaitForCompletion();
        yield return enemySprite.CatchingAnimation();
        yield return catchObjSprite.transform.DOMoveY(enemySprite.transform.position.y - 1, 0.5f).WaitForCompletion();

        int shakeCount = TryToCatch(enemySprite.Mon);


        //animate shaking up to three times
        for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            yield return catchObjSprite.transform.DOPunchRotation(new Vector3(0, 0, 10), 0.8f).WaitForCompletion();
            yield return new WaitForSeconds(0.5f);
        }

        //if Shake Count is 4, catch mon and end game; else, escape ball and continue game
        if (shakeCount == 4)
        {
            //caught
            yield return dialogueBox.TypeDialogue($"{enemySprite.Mon.MonBase.MonName} was caught!");
            yield return catchObjSprite.DOFade(0f, 1.5f).WaitForCompletion();

            if (enemySprite.Mon.MonBase.IsBoss)
            {
                caughtBoss = true;
            }
            else if (enemySprite.Mon.MonBase.IsGhost)
            {
                defeatedGhost = true;
            }

            playerParty.AddMon(enemySprite.Mon);
            yield return dialogueBox.TypeDialogue($"{enemySprite.Mon.MonBase.MonName} has been added to your party.");

            Destroy(catchObjSprite);
            BattleOver(true);

        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            catchObjSprite.DOFade(0, 0.2f);
            yield return enemySprite.CatchingFailAnimation();

            //if shook up to 2 times, normal dialogue; else, almost had it
            if(shakeCount <= 2)
            {
                yield return dialogueBox.TypeDialogue($"{enemySprite.Mon.MonBase.MonName} broke free!");

            }
            else
            {
                yield return dialogueBox.TypeDialogue($"Almost had it!");
            }

            Destroy(catchObjSprite);

            state = BattleState.RunningTurn;
        }

    }

    int TryToCatch(Monster mon)
    {
        int statusBonus = 1;
        float a = (3 * mon.MaxHp - 2 * mon.Hp) * mon.MonBase.CatchRate * statusBonus / (3 * mon.MaxHp);
        if(a >= 255)
        {
            return 4;
        }

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;

        while(shakeCount < 4)
        {
            if(UnityEngine.Random.Range(0, 65535) >= b)
            {
                break;
            }
            shakeCount++;
        }

        return shakeCount;
    }

    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        escapeAttempts++;
        int playerSpeed = playerSprite.Mon.Speed;
        int enemySpeed = enemySprite.Mon.Speed;
        dialogueBox.EnableActionSelector(false, QballCount);
        yield return dialogueBox.TypeDialogue("Got away safely!");
        BattleOver(true);

        

        if (enemySpeed < playerSpeed)
        {
            dialogueBox.EnableActionSelector(false, QballCount);
            yield return dialogueBox.TypeDialogue("Got away safely!");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f %= 256;

            if(UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogueBox.TypeDialogue("Got away safely!");
                BattleOver(true);
            }
            else
            {
                yield return dialogueBox.TypeDialogue("You couldn't escape!");
                state = BattleState.RunningTurn;
            }
        }
        yield return null;
    }

    IEnumerator ForgetAMove(Monster mon, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return dialogueBox.TypeDialogue("Choose a move to forget.");

        moveSelectUI.gameObject.SetActive(true);
        moveSelectUI.SetMoveData(mon.Moves.Select(x => x.Base).ToList(), newMove);

        state = BattleState.ForgetMove;

    }



    //Move State Methods
    IEnumerator RunMove(BattleSprite srcSprite, BattleSprite targetSprite, Move move)
    {
        //Run dialogue and animations, and reduce PP
        yield return dialogueBox.TypeDialogue($"{srcSprite.Mon.MonBase.MonName} used {move.Base.MoveName}.");
        move.PP--;

        //if rng returns unit greater than accuracy value, send fail message and begin next sequence
        if (UnityEngine.Random.Range(0, 101) > move.Base.Accuracy)
        {
            yield return dialogueBox.TypeDialogue($"{srcSprite.Mon.MonBase.MonName} missed!");
            yield return new WaitForSeconds(0.5f);
            yield break;
        }

        srcSprite.AttackAnimation();

        //if move is a status effect, run effects ienumerator; else, attack
        if(move.Base.Category == MoveCategory.Status)
        {
            yield return RunMoveEffects(move, srcSprite.Mon, targetSprite.Mon);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            targetSprite.DamageAnimation();

            var targetDamage = targetSprite.Mon.TakeDamage(move, srcSprite.Mon);

            yield return targetSprite.Hud.UpdateHP();
            yield return ShowDamageDetails(targetDamage);
        }


        //if hp reaches 0, begin faint sequence
        if (targetSprite.Mon.Hp <= 0)
        {
            yield return FaintingSequence(targetSprite);
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
                //foreach (var boost in target.StatusChanges)
                //{
                    
                //}
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
            if (enemySprite.Mon.MonBase.IsBoss)
            {
                defeatedBoss = true;
            }
            else if (enemySprite.Mon.MonBase.IsGhost)
            {
                defeatedGhost = true;
            }
            BattleOver(true);
        }
    }

    IEnumerator ShowStatusChanges(Monster mon)
    {
        while (mon.StatusChanges.Count > 0)
        {
            var message = mon.StatusChanges.Dequeue();
            yield return dialogueBox.TypeDialogue(message);
            yield return new WaitForSeconds(0.5f);
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

    IEnumerator FaintingSequence(BattleSprite fainted)
    {
        fainted.FaintAnimation();
        yield return dialogueBox.TypeDialogue($"{fainted.Mon.MonBase.MonName} fainted!");
        yield return new WaitForSeconds(1f);

        //if enemy fainted, give xp to playermon
        if (!fainted.IsPlayermon)
        {
            //check if boss
            if (enemySprite.Mon.MonBase.IsBoss)
            {
                defeatedBoss = true;
            }

            //gain XP
            int xpYield = playerSprite.Mon.MonBase.BaseXp;
            int enemyLvl = enemySprite.Mon.Level;
            int xpGained = Mathf.FloorToInt((xpYield * enemyLvl) / 7); ;

            //add new XP to mon and display dialogue text
            playerSprite.Mon.Xp += xpGained;
            yield return dialogueBox.TypeDialogue($"{playerSprite.Mon.MonBase.MonName} gained {xpGained} XP!");
            yield return playerSprite.Hud.UpdateXP();

            //check if able to level up
            while(playerSprite.Mon.LeveledUp())
            {
                //level up!
                playerSprite.Mon.CalculateStats();
                playerSprite.Hud.SetLevel();
                playerSprite.Mon.Hp = playerSprite.Mon.MaxHp;
                playerSprite.Hud.SetHP();
                yield return dialogueBox.TypeDialogue($"{playerSprite.Mon.MonBase.MonName} is now level {playerSprite.Mon.Level}!");

                //if there is a move to learn this level, begin attempt to learn move
                var newMove = playerSprite.Mon.GetCurrentMove();
                if(newMove != null)
                {
                    yield return dialogueBox.TypeDialogue($"{playerSprite.Mon.MonBase.MonName} learned {newMove.Base.MoveName}!");
                    //if mon has not yet learned max number of moves, auto add; else, ask if player would like to forget a move for the new one
                    if (playerSprite.Mon.Moves.Count < MonsterBase.maxMoves)
                    {
                        playerSprite.Mon.LearnMove(newMove);
                        dialogueBox.SetMoveNames(playerSprite.Mon.Moves);
                    }
                    else
                    {
                        moveToLearn = newMove.Base;
                        yield return new WaitForSeconds(1f);
                        yield return dialogueBox.TypeDialogue($"{playerSprite.Mon.MonBase.MonName} has learned too many moves.");
                        yield return ForgetAMove(playerSprite.Mon, newMove.Base);
                        yield return new WaitUntil(() => state != BattleState.ForgetMove);
                    }
                }

                yield return playerSprite.Hud.UpdateXP(true);
            }

            yield return new WaitForSeconds(1.5f);
        }

        CheckForBattleOver(fainted);
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
        else if(state == BattleState.ForgetMove)
        {
            //action will take the data from the handle move selection method and decide which move the player wants to forget
            Action<int> onMoveSelect = (moveIndex) =>
            {
                moveSelectUI.gameObject.SetActive(false);
                //if the move selected is the new move, do not learn; else, forget chosen move and learn the new move
                if(moveIndex == MonsterBase.maxMoves)
                {
                    StartCoroutine(dialogueBox.TypeDialogue($"{playerSprite.Mon.MonBase.MonName} did not learn {moveToLearn.MoveName}."));
                    moveToLearn = null;
                    state = BattleState.RunningTurn;
                }
                else
                {
                    var moveToForget = playerSprite.Mon.Moves[moveIndex].Base.MoveName;
                    StartCoroutine(dialogueBox.TypeDialogue($"{playerSprite.Mon.MonBase.MonName} forgot {moveToForget} and has learned {moveToLearn.MoveName}!"));
                    playerSprite.Mon.Moves[moveIndex] = new Move(moveToLearn);                    
                    moveToLearn = null;
                    state = BattleState.RunningTurn;
                }
            };
            moveSelectUI.HandleMoveSelection(onMoveSelect);
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
                    //catch
                    if(QballCount > 0)
                    {
                        if(playerParty.Monsters.Count >= 6)
                        {
                            StartCoroutine(dialogueBox.TypeDialogue("Your MonStash is full!"));

                        }
                        else
                        {
                            StartCoroutine(RunTurns(BattleAction.UseItem));
                        }
                    }
                    else
                    {
                        StartCoroutine(dialogueBox.TypeDialogue("You have no more Qballs!"));
                    }
                }
                else
                {
                    //run
                    StartCoroutine(RunTurns(BattleAction.Run));

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
            Debug.Log("attack chosen");
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableActionSelector(true,QballCount);
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
                    previousState = null;
                    StartCoroutine(SwitchMonster(selectedMember));
                }
                else
                {
                    state = BattleState.Busy;
                    previousState = null;
                    StartCoroutine(RunTurns(BattleAction.SwitchMon));
                }
            }
        }
        //if backspace pressed and previous mon did not faint, return to Player Action screen
        else if (Input.GetKeyDown(KeyCode.Backspace) && previousState != BattleState.RunningTurn)
        {
            currentPartyMember = 0;
            partyScreen.UpdateMonsterSelection(currentPartyMember);
            partyScreen.gameObject.SetActive(false);
            dialogueBox.EnableActionSelector(true,QballCount);
            dialogueBox.EnableDialogueText(true);
            state = BattleState.ActionSelection;
        }
    }

}
