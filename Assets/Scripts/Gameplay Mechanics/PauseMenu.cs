using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public enum PauseState
{
    Main,
    Party,
    Swap,
    Release
}



public class PauseMenu : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> menuOptions;
    [SerializeField] GameObject pauseMenu;

    [SerializeField] List<TextMeshProUGUI> partyMenuOptions;
    [SerializeField] GameObject partyMenu;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MonParty playerParty;


    int currentMenuSelection;
    public Color highlightColor;

    PauseState state;

    public event Action OnResume;

    private void Awake()
    {
        partyScreen.Init();
        SetUpPauseMenu();
    }



    void SetUpPauseMenu()
    {
        partyMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }


    void SetUpPartyMenu()
    {
        partyScreen.SetPartyData(playerParty.Monsters);
        partyScreen.SetMessageText("What would you like to do?");
        pauseMenu.SetActive(false);
        partyMenu.SetActive(true);
    }



    public void HandleUpdate()
    {
        if(state == PauseState.Main)
        {
            HandlePauseMenu();
        }
        else if(state == PauseState.Party)
        {
            HandlePartyMenu();
        }
    }



    void HandlePauseMenu()
    {
        HandleMenu(menuOptions);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentMenuSelection == 0)
            {
                OnResume();
            }
            else if (currentMenuSelection == 1)
            {
                //party menu
                currentMenuSelection = 0;
                SetUpPartyMenu();
                state = PauseState.Party;
            }
            else
            {
                //quit
                currentMenuSelection = 0;
                Debug.Log("goodbye");

            }
        }
    }



    void HandlePartyMenu()
    {
        HandleMenu(partyMenuOptions);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentMenuSelection == 0)
            {
                //cannot swap if less than 2 healthy mons available; else swap
                if(playerParty.GetHealthyMons().Count < 2)
                {
                    partyScreen.SetMessageText("You need at least two mons to swap.");
                }
                else
                {
                    HandleSwap();
                }
            }
            else if (currentMenuSelection == 1)
            {
                //cannot release if less than 2 healthy mons available; else check for release
                if(playerParty.GetHealthyMons().Count < 2)
                {
                    partyScreen.SetMessageText("You need at least one mon in your party.");
                }
                else
                {
                    currentMenuSelection = 0;
                    HandleRelease();
                }

            }
            else
            {
                //return to previous menu
                currentMenuSelection = 0;
                SetUpPauseMenu();
                state = PauseState.Main;

            }
        }
    }



    void HandleSwap()
    {
        //select first mon
        //select second mon
        //swap
        //send message
        Debug.Log("swapping");
    }



    void HandleRelease()
    {
        //select mon
        //check confirm
        //release
        //send message and return to main state
        Debug.Log("releasing");
    }



    void HandleMenu(List<TextMeshProUGUI> menu)
    {
        //check which menu item the player selected
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMenuSelection > 0)
            {
                currentMenuSelection--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMenuSelection < menu.Count - 1)
            {
                currentMenuSelection++;
            }
        }

        //highlight selection
        HighlightSelection(menu);
    }



    void HighlightSelection(List<TextMeshProUGUI> menu)
    {
        for (int i = 0; i < menu.Count; i++)
        {
            if (i == currentMenuSelection)
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
