using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCNarrator : MonoBehaviour
{
    [SerializeField] Dialogue defeatedBossDialogue;
    [SerializeField] Dialogue caughtBossDialogue;

    public event Action OnWinGame;

    public IEnumerator NormalDialogue(Dialogue midGameDialogue)
    {
        yield return DialogueManager.Instance.ShowDialogue(midGameDialogue);
    }

    public IEnumerator DefeatedBossDialogue()
    {
        yield return DialogueManager.Instance.ShowDialogue(defeatedBossDialogue);
        OnWinGame();
    }

    public IEnumerator CaughtBossDialogue()
    {
        yield return DialogueManager.Instance.ShowDialogue(caughtBossDialogue);
        OnWinGame();
    }
}
