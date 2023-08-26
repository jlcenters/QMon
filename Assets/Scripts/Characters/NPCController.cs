using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] Transform player;
    NPCHealer healer;

    private void Awake()
    {
        healer = GetComponent<NPCHealer>();
    }

    public void Interact()
    {
        if(healer != null)
        {
            StartCoroutine(healer.Heal(player, dialogue));
        }
        else
        {
            StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
        }
    }
}
