using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHealer : MonoBehaviour
{
    public IEnumerator Heal(Transform player, Dialogue dialogue)
    {
        //TODO: can talk to mom and choose between heal and allowance

        //show first dialogue; wait one second and continue when done with first line
        yield return DialogueManager.Instance.ShowDialogue(dialogue);
        yield return new WaitUntil(() => !DialogueManager.Instance.isTyping);

        //get reference to mon party and fade in
        var playerParty = player.GetComponent<MonParty>();
        yield return Fader.instance.FadeIn();

        //heal all mon and fade out
        playerParty.Monsters.ForEach(m => m.Heal());
        yield return Fader.instance.FadeOut();

        //TODO: when incorporating health potions, add hp ui fixing event which will update the ui of mons
    }
}
