using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] Transform player;
    NPCHealer healer;
    NPCShop shop;
    NPCNarrator narrator;

    private void Awake()
    {
        healer = GetComponent<NPCHealer>();
        shop = GetComponent<NPCShop>();
        narrator = GetComponent<NPCNarrator>();
    }

    public void Interact()
    {
        //if npc is narrator, preform special dialogue;
        //else if npc is a healer, heal;
        //else, if npc is a shop, shop;
        //else, npc is just a dialogue npc
        if(narrator != null)
        {
            //if initial dialogue, show initial
            //else if boss was defeated, show boss defeated dialogue
            //else if boss was caught, show boss caught dialogue
            //else show default dialogue
            if (narrator.firstInteraction)
            {
                StartCoroutine(narrator.InitialDialogue());
            }
            /*else if (false)
            {
                StartCoroutine(narrator.DefeatedBossDialogue());
            }
            else if (true)
            {
                StartCoroutine(narrator.CaughtBossDialogue());
            }*/
            else
            {
                StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
            }
        }
        else if(healer != null)
        {
            StartCoroutine(healer.Heal(player, dialogue));
        }
        else if (shop != null)
        {
            //if player has less than 10 qballs, offer more; else, deny
            if(player.GetComponent<Inventory>().Qballs < 10)
            {
                shop.CanShop = true;
                StartCoroutine(shop.Shop(player, dialogue));
            }
            else
            {
                shop.CanShop = false;
                StartCoroutine(shop.Shop(player, shop.CannotShopDialogue));
            }
        }
        else
        {
            StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
        }
    }
}
