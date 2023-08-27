using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCShop : MonoBehaviour
{
    [SerializeField] Dialogue cannotShopDialogue;
    
    public bool CanShop { get; set; }
    public Dialogue CannotShopDialogue { get { return cannotShopDialogue; } }



    public IEnumerator Shop(Transform player, Dialogue dialogue)
    {
        //greet player
        yield return DialogueManager.Instance.ShowDialogue(dialogue);

        //if can shop, give player 5 more qballs
        if (CanShop)
        {
            player.GetComponent<Inventory>().Qballs += 5;
        }
    }
}
