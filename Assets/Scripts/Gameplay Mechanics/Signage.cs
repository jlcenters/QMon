using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signage : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] Transform player;

    public void Interact()
    {
        StartCoroutine(ShowText(dialogue));
    }



    public IEnumerator ShowText(Dialogue dialogue)
    {
        yield return DialogueManager.Instance.ShowText(dialogue);
    }
}
