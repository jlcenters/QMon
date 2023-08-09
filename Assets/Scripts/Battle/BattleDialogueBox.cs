using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleDialogueBox : MonoBehaviour
{
    [SerializeField] TMP_Text dialogue;

    public void SetDialogue(string txt)
    {
        dialogue.text = txt;
    }
}
