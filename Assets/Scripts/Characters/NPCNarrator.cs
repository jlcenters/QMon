using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCNarrator : MonoBehaviour
{
    public bool firstInteraction;
    [SerializeField] Dialogue initialDialogue;
    [SerializeField] Dialogue defeatedBossDialogue;
    [SerializeField] Dialogue caughtBossDialogue;

    public IEnumerator InitialDialogue()
    {
        yield return DialogueManager.Instance.ShowDialogue(initialDialogue);
    }

    public IEnumerator NormalDialogue(Dialogue midGameDialogue)
    {
        yield return DialogueManager.Instance.ShowDialogue(midGameDialogue);
    }

    public IEnumerator DefeatedBossDialogue()
    {
        yield return DialogueManager.Instance.ShowDialogue(defeatedBossDialogue);
    }

    public IEnumerator CaughtBossDialogue()
    {
        yield return DialogueManager.Instance.ShowDialogue(caughtBossDialogue);
    }
}
