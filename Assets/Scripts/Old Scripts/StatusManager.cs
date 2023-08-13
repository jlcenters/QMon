using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatusManager : MonoBehaviour
{
    public CharacterStatus statusData;
    bool battling = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the trigger collider is an enemy, store data and start battling
        if(collision.CompareTag("Enemy"))
        {
            if (!battling)
            {
                battling = true;
                SetPlayerData();
                SceneManager.LoadScene("Battle");
            }
        }
    }

    void SetPlayerData()
    {
        //instance of character status is made based on the stored data in the player object
        CharacterStatus playerData = gameObject.GetComponent<PlayerController>().storedData;
        
        //store position in world
        statusData.position[0] = this.transform.position.x;
        statusData.position[1] = this.transform.position.y;

        //store money
        statusData.money = playerData.money;

        //store mon and inventory
        foreach(Monster mon in statusData.party)
        {
            playerData.party.Add(mon);
        }
        foreach (GameObject item in statusData.inventory)
        {
            playerData.inventory.Add(item);
        }
    }
}
