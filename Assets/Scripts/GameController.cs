using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum GameState
{
    FreeRoam,
    MainMenu,
    PauseMenu,
    Battle,
    Dialogue,
    GameOver
}



public class GameController : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject gameOverScreen;

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

        //run through all separate grids in map until you find the one that matches flower types with the player
        Monster wildMon = null;
        MapArea[] grids = FindObjectsOfType<MapArea>();
        foreach(var grid in grids)
        {
            if(grid.flowerType == playerController.flowerType)
            {
                wildMon = grid.GetRandomWildMon();
            }
        }

        //if no grid matched the player flower type, end method without starting battle; else, init mon and start battle
        if(wildMon == null)
        {
            Debug.Log("No monster attached");
        }
        else
        {
            var copy = new Monster(wildMon.MonBase, wildMon.Level);
            battleSystem.StartBattle(playerParty, copy, qballCount);
        }
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
        /*if (state == GameState.MainMenu)
        {
            //main menu
        }*/
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        /*else if (state == GameState.PauseMenu)
        {
            //pause menu
        }*/
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if(state == GameState.Dialogue)
        {
            DialogueManager.Instance.HandleUpdate();
        }
        /*else if (state == GameState.GameOver)
        {
            //game over screen
        }*/
    }
}
