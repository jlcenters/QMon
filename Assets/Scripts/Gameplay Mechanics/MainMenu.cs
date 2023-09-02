using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;



public enum MenuState
{
    Main,
    Settings
}



public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] List<TextMeshProUGUI> menuOptions;
    [SerializeField] GameObject settingsMenu;
    //[SerializeField] List<TextMeshProUGUI> settingsOptions;

    int currentMenuSelection = 0;
    [SerializeField] Color highlightColor;

    MenuState state;

    public event Action OnStartGame;



    public void HandleUpdate()
    {
        if(state == MenuState.Main)
        {
            settingsMenu.SetActive(false);
            mainMenu.SetActive(true);
            HandleMainMenu();
        }
        else if(state == MenuState.Settings)
        {
            mainMenu.SetActive(false);
            settingsMenu.SetActive(true);
            HandleSettingsMenu();
        }
    }



    void HandleMainMenu()
    {
        //check which menu item the player selected
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(currentMenuSelection < menuOptions.Count - 1)
            {
                currentMenuSelection++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(currentMenuSelection > 0)
            {
                currentMenuSelection--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(currentMenuSelection > 1)
            {
                currentMenuSelection -= 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(currentMenuSelection < 2)
            {
                if(currentMenuSelection == 0)
                {
                    currentMenuSelection += 2;
                }
                else
                {
                    currentMenuSelection++;
                }
            }
        }

        //highlight selection
        HighlightSelection(menuOptions, currentMenuSelection);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentMenuSelection == 0)
            {
                currentMenuSelection = 0;
                OnStartGame();
            }
            else if (currentMenuSelection == 1)
            {
                currentMenuSelection = 0;
                state = MenuState.Settings;
            }
            else
            {
                Debug.Log("goodbye");
                Application.Quit();
            }
        }
    }



    void HandleSettingsMenu()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            state = MenuState.Main;
        }
    }



    void HighlightSelection(List<TextMeshProUGUI> menu, int selection)
    {
        for(int i = 0; i < menu.Count; i++)
        {
            if(i == selection)
            {
                menu[i].color = highlightColor;
            }
            else
            {
                menu[i].color = Color.white;
            }
        }
    }
}
