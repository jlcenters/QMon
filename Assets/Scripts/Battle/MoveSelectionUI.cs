using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] List<TMP_Text> moveTexts;
    [SerializeField] TextMeshProUGUI description;
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

    public void HandleMoveSelection(System.Action<int> onSelect, System.Action<int> onUpdate)
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

        UpdateMoveSelection(currentSelection, onUpdate);

        //invokes action which was passed in param
        if (Input.GetKeyDown(KeyCode.Space))
        {
            onSelect.Invoke(currentSelection);
        }
    }

    public void UpdateMoveSelection(int currentSelection, System.Action<int> onUpdate)
    {
        for(int i = 0; i < moveTexts.Count; i++)
        {
            if(i == currentSelection)
            {
                moveTexts[i].color = highlightColor;
                onUpdate.Invoke(i);
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }
    }
}
