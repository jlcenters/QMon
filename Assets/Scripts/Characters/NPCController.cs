using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] Transform player;
    NPCHealer healer;
    NPCShop shop;

    private void Awake()
    {
        healer = GetComponent<NPCHealer>();
        shop = GetComponent<NPCShop>();
    }

    public void Interact()
    {
        //if npc is a healer, heal; else, if npc is a shop, shop; else, npc is just a dialogue npc
        if(healer != null)
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
