using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


//TODO: Fix restart button so that new instance of player is created and spawned at original position
//currently cannot restart
public enum GameOverState
{
    Main,
    Confirm
}



public class GameOverMenu : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> menuOptions;

    int currentMenuSelection = 0;
    [SerializeField] Color highlightColor;

    [SerializeField] GameObject confirmQuitMenu;
    [SerializeField] List<TextMeshProUGUI> confirmOptions;

    //public event Action OnRestartGame;

    GameOverState state;

    public void HandleUpdate()
    {
        if(state == GameOverState.Main)
        {
            confirmQuitMenu.SetActive(false);
            CheckForSelection();
        }
        else if(state == GameOverState.Confirm)
        {
            confirmQuitMenu.SetActive(true);
            CheckForQuit();
        }
        
    }



    void HighlightSelection(List<TextMeshProUGUI> options)
    {
        for(int i = 0; i < options.Count; i++)
        {
            if(i == currentMenuSelection)
            {
                options[i].color = highlightColor;
            }
            else
            {
                options[i].color = Color.white;
            }
        }
    }



    void CheckForSelection()
    {
        //determine and highlight selection
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMenuSelection < menuOptions.Count - 1)
            {
                currentMenuSelection++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMenuSelection > 0)
            {
                currentMenuSelection--;
            }
        }
        //HighlightSelection(menuOptions);
        //TODO: Add retry logic
        //if space is pressed, select highlighted option
        if (Input.GetKeyDown(KeyCode.Space))
        {
            /*if (currentMenuSelection == 0)
            {
                currentMenuSelection = 0;
                OnRestartGame();
            }
            else
            {
                currentMenuSelection = 0;
                state = GameOverState.Confirm;
           }*/

            //QUIT
            Debug.Log("Goodbye");
            Application.Quit();
        }
    }



    void CheckForQuit()
    {
        //match current menu selection with what the player wants to highlight
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMenuSelection < confirmOptions.Count - 1)
            {
                currentMenuSelection++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMenuSelection > 0)
            {
                currentMenuSelection--;
            }
        }
        HighlightSelection(confirmOptions);

        //if space is pressed, select highlighted option
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentMenuSelection == 0)
            {
                Debug.Log("Goodbye");
                Application.Quit();
            }
            else
            {
                currentMenuSelection = 0;
                state = GameOverState.Main;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            currentMenuSelection = 0;
            state = GameOverState.Main;
        }
    }
}
