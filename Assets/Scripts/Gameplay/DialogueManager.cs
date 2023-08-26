using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialogueBox;
    [SerializeField] TMP_Text dialogue;

    public event Action OnShowDialogue;
    public event Action OnCloseDialogue;

    public static DialogueManager Instance { get; private set; }

    public float lettersPerSecond;
    int currentLine = 0;
    Dialogue lines;

    public bool isTyping;


    //to reference instance throughout the game
    private void Awake()
    {
        Instance = this;
    }


    //called when state has changed in game controller
    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTyping)
        {
            ++currentLine;
            if(currentLine < lines.Lines.Count)
            {
                StartCoroutine(TypeDialogue(lines.Lines[currentLine]));
            }
            else
            {
                currentLine = 0;
                dialogueBox.SetActive(false);
                OnCloseDialogue.Invoke();
            }
        }
    }



    //show dialogue box and type out dialogue
    public IEnumerator ShowDialogue(Dialogue dialogue)
    {
        yield return new WaitForEndOfFrame();
        lines = dialogue;
        OnShowDialogue.Invoke();

        dialogueBox.SetActive(true);

        StartCoroutine(TypeDialogue(dialogue.Lines[0]));
    }



    public IEnumerator TypeDialogue(string line)
    {
        isTyping = true;
        //start as an empty box
        dialogue.text = "";

        foreach (var letter in line.ToCharArray())
        {
            dialogue.text += letter;

            //will display specified number of letters per second
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
        yield return new WaitForSeconds(1f);

    }
}
