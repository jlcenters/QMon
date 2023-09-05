using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum GameState
{
    MainMenu,
    FreeRoam,
    PauseMenu,
    Battle,
    Dialogue,
    GameOverMenu,
}



public class GameController : MonoBehaviour
{
    [SerializeField] MainMenu mainMenu;
    [SerializeField] PlayerController playerController;
    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera mainCamera;
    [SerializeField] GameOverMenu gameOverMenu;
    [SerializeField] NPCNarrator narrator;

    [SerializeField] GameOverMenu winGameMenu;
    [SerializeField] GameObject bossFlowers;

    GameState state;
    bool win;



    //set up events
    private void Start()
    {
        Debug.Log("Note: currently cannot restart game upon game over");

        playerController.OnEncounter += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
        mainMenu.OnStartGame += StartGame;
        playerController.OnPause += PauseGame;
        pauseMenu.OnResume += ResumeGame;
        narrator.OnWinGame += WinGame;
        
        //gameOverMenu.OnRestartGame += Retry;

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



    void StartGame()
    {
        state = GameState.FreeRoam;
        mainMenu.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
    }



    void PauseGame()
    {
        state = GameState.PauseMenu;
        pauseMenu.gameObject.SetActive(true);
    }


    void ResumeGame()
    {
        state = GameState.FreeRoam;
        pauseMenu.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
    }



    void Retry()
    {
        state = GameState.FreeRoam;
        gameOverMenu.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
    }



    void WinGame()
    {
        win = true;
        state = GameState.GameOverMenu;
        winGameMenu.gameObject.SetActive(true);
    }



    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);


        Monster wildMon = null;
        MapArea[] grids = FindObjectsOfType<MapArea>();
        foreach (var grid in grids)
        {
            if(grid.flowerType == playerController.flowerType)
            {
                //run through all separate grids in map until you find the one that matches flower types with the player
                wildMon = grid.GetRandomWildMon();




            }
        }



        //if no grid matched the player flower type, end method without starting battle; else, init mon and start battle
        if (wildMon == null)
        {
                Debug.Log("No monster attached");
         }
        else
        {
            var playerParty = playerController.GetComponent<MonParty>();
            var qballCount = playerController.GetComponent<Inventory>().Qballs;
            var copy = new Monster(wildMon.MonBase, wildMon.Level);
            battleSystem.StartBattle(playerParty, copy, qballCount);
        }


    }



    void EndBattle(bool won)
    {
        playerController.surpriseEffect.RemoveEffect();
        if (won)
        {
            //check if boss was defeated or caught
            if (battleSystem.bossBattle)
            {
                if (battleSystem.defeatedBoss)
                {
                    playerController.bossDefeated = true;
                    //deactivates boss flowers
                    bossFlowers.SetActive(false);
                }
                else if (battleSystem.caughtBoss)
                {
                    playerController.bossCaught = true;
                    //deactivates boss flowers
                    bossFlowers.SetActive(false);
                }

            }

            playerController.GetComponent<Inventory>().Qballs = battleSystem.QballCount;
            state = GameState.FreeRoam;
            battleSystem.gameObject.SetActive(false);
            mainCamera.gameObject.SetActive(true);
        }
        else
        {
            state = GameState.GameOverMenu;
            battleSystem.gameObject.SetActive(false);
            gameOverMenu.gameObject.SetActive(true);
        }

    }


    
    private void Update()
    {
        if (state == GameState.MainMenu)
        {
            mainMenu.HandleUpdate();
        }
        else if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.PauseMenu)
        {
            pauseMenu.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if(state == GameState.Dialogue)
        {
            DialogueManager.Instance.HandleUpdate();
        }
        else if (state == GameState.GameOverMenu)
        {
            if (win)
            {
                winGameMenu.HandleUpdate();
            }
            else
            {
                gameOverMenu.HandleUpdate();
            }
        }
    }
}
