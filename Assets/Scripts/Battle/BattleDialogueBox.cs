using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleDialogueBox : MonoBehaviour
{
    //Initial Dialogue View
    [SerializeField] int lettersPerSecond;
    [SerializeField] TMP_Text dialogue;

    //Action View
    [SerializeField] TextMeshProUGUI qballText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] List<TextMeshProUGUI> actionTexts;
    [SerializeField] Color highlightedColor;

    //Move View
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] List<TMP_Text> moveTexts;
    [SerializeField] TMP_Text ppText;
    [SerializeField] TMP_Text elementText;



    // Displaying text in text boxes
    public IEnumerator TypeDialogue(string message)
    {
        //start as an empty box
        dialogue.text = "";

        foreach(var letter in message.ToCharArray())
        {
            dialogue.text += letter;

            //will display specified number of letters per second
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        yield return new WaitForSeconds(0.5f);
    }
    public void SetDialogue(string txt)
    {
        dialogue.text = txt;
    }

    //Enabling / Disabling the different views
    public void EnableDialogueText(bool isEnabled)
    {
        dialogue.enabled = isEnabled;
    }
    public void EnableActionSelector(bool isEnabled, int qballCount)
    {
        actionSelector.SetActive(isEnabled);
        UpdateQballText($"Catch (x{qballCount})");
    }
    public void EnableMoveSelector(bool isEnabled)
    {
        moveSelector.SetActive(isEnabled);
        moveDetails.SetActive(isEnabled);
    }


    //Updating the different views
    public void UpdateActionSelection(int selectedAction)
    {
        //highlight text
        for(int i = 0; i < actionTexts.Count; i++)
        {
            if(i == selectedAction)
            {
                actionTexts[i].color = highlightedColor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        //highlight text
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i == selectedMove)
            {
                moveTexts[i].color = highlightedColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }

        //update PP and Element GameObjects
        ppText.text = $"PP {move.PP} / {move.Base.PP}";
        elementText.text = move.Base.Type.ToString();
        //if pp is at zero, display will be red; else, pp display will be black
        if (move.PP == 0)
        {
            ppText.color = Color.red;
        }
        else
        {
            ppText.color = Color.black;
        }

    }

    public void UpdateQballText(string text)
    {
        qballText.text = text;
    }

    public void SetMoveNames(List<Move> moves)
    {
        //sets move name to dialogue box; if less than 4 will set to a hyphen
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if(i < moves.Count)
            {
                moveTexts[i].text = moves[i].Base.MoveName;
            }
            else
            {
                moveTexts[i].text = "-";
            }
        }
    }

}
