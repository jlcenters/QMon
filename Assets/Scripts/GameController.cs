using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum GameState
{
    FreeRoam,
    Battle,
    Dialogue
}



public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera mainCamera;
    GameState state;

    private void Start()
    {
        playerController.OnEncounter += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        DialogueManager.Instance.OnShowDialogue += () =>
        {
            state = GameState.Dialogue;
        };
        DialogueManager.Instance.OnCloseDialogue += () =>
        {
            if(state == GameState.Dialogue)
            {
                state = GameState.FreeRoam;
            }
        };
    }

    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<MonParty>();
        var qballCount = playerController.GetComponent<Inventory>().Qballs;
        var wildMon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildMon();
        var copy = new Monster(wildMon.MonBase, wildMon.Level);
        battleSystem.StartBattle(playerParty, copy, qballCount);
    }

    void EndBattle(bool won)
    {
        playerController.GetComponent<Inventory>().Qballs = battleSystem.QballCount;
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
    }


    
    private void Update()
    {
        if(state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if(state == GameState.Dialogue)
        {
            DialogueManager.Instance.HandleUpdate();
        }
    }
}
