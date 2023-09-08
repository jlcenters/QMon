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
    Release,
    ConfirmQuit
}



public class PauseMenu : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> menuOptions;
    [SerializeField] GameObject pauseMenu;

    [SerializeField] List<TextMeshProUGUI> partyMenuOptions;
    [SerializeField] GameObject partyMenu;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MonParty playerParty;

    [SerializeField] List<TextMeshProUGUI> confirmQuitMenuOptions;
    [SerializeField] GameObject confirmQuitMenu;


    int currentMenuSelection;
    int firstMonSelection;
    int secondMonSelection;
    public Color highlightColor;

    PauseState state;

    public event Action OnResume;

    private void Awake()
    {
        partyScreen.Init();
        currentMenuSelection = 0;
        SetUpPauseMenu();
    }



    void SetUpPauseMenu()
    {
        partyMenu.SetActive(false);
        confirmQuitMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }



    void SetUpPartyMenu()
    {
        partyScreen.SetPartyData(playerParty.Monsters);
        partyScreen.SetMessageText("What would you like to do?");
        pauseMenu.SetActive(false);
        partyMenu.SetActive(true);
    }



    void SetUpConfirmQuitMenu()
    {
        partyMenu.SetActive(false);
        pauseMenu.SetActive(false);
        confirmQuitMenu.SetActive(true);
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
        else if(state == PauseState.Swap)
        {
            HandleSwap();
        }
        else if(state == PauseState.Release)
        {
            HandleRelease();
        }
        else if(state == PauseState.ConfirmQuit)
        {
            HandleConfirmQuit();
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
                SetUpConfirmQuitMenu();
                state = PauseState.ConfirmQuit;

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
                    partyScreen.SetMessageText("You need at least two healthy mons to swap.");
                }
                else
                {
                    partyScreen.SetMessageText("Choose which mons to swap.");
                    state = PauseState.Swap;
                }
            }
            else if (currentMenuSelection == 1)
            {
                //cannot release if less than 2 healthy mons available; else check for release
                if(playerParty.GetHealthyMons().Count < 2)
                {
                    partyScreen.SetMessageText("You need at least one healthy mon in your party at all times.");
                }
                else
                {
                    partyScreen.SetMessageText("Choose which mon to release.");
                    currentMenuSelection = 0;
                    state = PauseState.Release;
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
        //HandlePartyScreen(firstMonSelection);
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (firstMonSelection < playerParty.Monsters.Count - 1)
            {
                firstMonSelection++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (firstMonSelection > 0)
            {
                firstMonSelection--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (firstMonSelection < playerParty.Monsters.Count - 2)
            {
                firstMonSelection += 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (firstMonSelection > 2)
            {
                firstMonSelection -= 2;
            }
        }
        //will check to make sure selection is within range, else it will add/subtract accordingly
        //firstMonSelection = Mathf.Clamp(firstMonSelection, 0, playerParty.Monsters.Count - 1);

        partyScreen.UpdateMonsterSelection(firstMonSelection);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("first selected");
            Debug.Log($"first index: {firstMonSelection}");
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (secondMonSelection < playerParty.Monsters.Count - 1)
                {
                    secondMonSelection++;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (secondMonSelection > 0)
                {
                    secondMonSelection--;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (secondMonSelection < playerParty.Monsters.Count - 2)
                {
                    secondMonSelection += 2;
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (secondMonSelection > 2)
                {
                    secondMonSelection -= 2;
                }
            }

            HandlePartyScreen(secondMonSelection);

            if (Input.GetKeyDown(KeyCode.Space) && firstMonSelection != secondMonSelection)
            {
                Debug.Log("second selected");
                Debug.Log($"second index: {secondMonSelection}");


                playerParty.SwapMons(playerParty.Monsters[firstMonSelection], firstMonSelection, playerParty.Monsters[secondMonSelection], secondMonSelection);
                partyScreen.SetPartyData(playerParty.Monsters);
                firstMonSelection = 0;
                secondMonSelection = 0;
                partyScreen.SetMessageText("Mons swapped.");
                //yield return new WaitForSeconds(0.5f);
                state = PauseState.Party;
            }
        }

        //select first mon
        //select second mon
        //swap
        //send message
    }



    void HandleRelease()
    {
        partyScreen.SetMessageText("Choose a mon to release.");
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (firstMonSelection < playerParty.Monsters.Count - 1)
            {
                firstMonSelection++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (firstMonSelection > 0)
            {
                firstMonSelection--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (firstMonSelection < playerParty.Monsters.Count - 2)
            {
                firstMonSelection += 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (firstMonSelection > 2)
            {
                firstMonSelection -= 2;
            }
        }
        HandlePartyScreen(firstMonSelection);
        //check confirm
        //release
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var monChosen = playerParty.Monsters[firstMonSelection];

            if (monChosen.MonBase.IsBoss)
            {
                List<string> dialogueList = new List<string>{ "That's too dangerous.", "Actually, nevermind.", "You'd really unleash this upon your town?", "Your gut is telling you not to.", "That's a terrible idea." };
                partyScreen.SetMessageText(dialogueList[UnityEngine.Random.Range(0, dialogueList.Count)]);
            }
            else
            {
                firstMonSelection = 0;
                playerParty.RemoveMon(monChosen);
                partyScreen.SetPartyData(playerParty.Monsters);
                HighlightSelection(partyMenuOptions);
                partyScreen.SetMessageText($"Goodbye, {monChosen.MonBase.MonName}!");
            }
            state = PauseState.Party;

        }

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



    void HandlePartyScreen(int currentPartyMember)
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentPartyMember < playerParty.Monsters.Count - 1)
            {
                currentPartyMember++;
                Debug.Log("moved right");

            }
            Debug.Log("cant move");
            Debug.Log($"{currentPartyMember}");
            Debug.Log($"{firstMonSelection}");


        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(currentPartyMember > 0)
            {
                currentPartyMember--;
                Debug.Log("moved left");

            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(currentPartyMember < playerParty.Monsters.Count - 2)
            {
                currentPartyMember += 2;
                Debug.Log("moved down");

            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(currentPartyMember > 2)
            {
                currentPartyMember -= 2;
                Debug.Log("moved up");


            }
        }
        //will check to make sure selection is within range, else it will add/subtract accordingly
        currentPartyMember = Mathf.Clamp(currentPartyMember, 0, playerParty.Monsters.Count - 1);

        partyScreen.UpdateMonsterSelection(currentPartyMember);
    }



    void HandleConfirmQuit()
    {
        //check which menu item the player selected
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMenuSelection > 0)
            {
                currentMenuSelection--;

            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMenuSelection < confirmQuitMenuOptions.Count - 1)
            {
                currentMenuSelection++;

            }
        }

        //highlight selection
        HighlightSelection(confirmQuitMenuOptions);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(currentMenuSelection == 0)
            {
                //cancel
                SetUpPauseMenu();
                state = PauseState.Main;
            }
            else if (currentMenuSelection == 1)
            {
                currentMenuSelection = 0;
                Debug.Log("goodbye");
                Application.Quit();
            }
        }
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
