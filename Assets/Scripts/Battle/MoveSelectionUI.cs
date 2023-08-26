using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] List<TMP_Text> moveTexts;
    public Color highlightColor;
    int currentSelection = 0;

    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        for(int i = 0; i < currentMoves.Count; i++)
        {
            moveTexts[i].text = currentMoves[i].MoveName;
        }

        moveTexts[currentMoves.Count].text = newMove.MoveName;
    }

    public void HandleMoveSelection(System.Action<int> onSelect)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentSelection++;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentSelection--;
        }
        currentSelection = Mathf.Clamp(currentSelection, 0, MonsterBase.maxMoves);

        UpdateMoveSelection(currentSelection);

        //invokes action which was passed in param
        if (Input.GetKeyDown(KeyCode.Space))
        {
            onSelect.Invoke(currentSelection);
        }
    }

    public void UpdateMoveSelection(int currentSelection)
    {
        for(int i = 0; i < moveTexts.Count; i++)
        {
            if(i == currentSelection)
            {
                moveTexts[i].color = highlightColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }
    }
}
