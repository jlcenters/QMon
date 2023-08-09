using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsHUD : MonoBehaviour
{

    public TextMeshProUGUI displayName;
    public TextMeshProUGUI displayLevel;
    public TextMeshProUGUI currentHp;
    public Image fillImage;

    public void SetStats()
    {
        //set og stats
    }

    public void TakeDamage(CharacterStatus statusData, float hp)
    {
        StartCoroutine(GraduallySetHP(statusData, hp, 10, 0.05f));
    }

    IEnumerator GraduallySetHP(CharacterStatus status, float amount, int fillRate, float fillDelay)
    {
        float percentage = 1 / (float)fillRate;

        for(int i = 0; i < fillRate; i++)
        {
            //TODO: add hp of active pokemon, and potentially also instance of active pokemon, to CharacterStatus
            float decimalAmt = amount * percentage;
            

            if(status.money + decimalAmt <= status.money && decimalAmt + status.money > 0) //if total hp is less than or equal to max hp,
                                                                      // and total hp is greater than 0,
                                                                     //add to data
            {
                status.money += (int)decimalAmt; //add or subtract dec amt to total hp of active pokemon
                float rate = decimalAmt / status.money; //divide by max health of active pokemon to get the rate at which it should increase/decrease


            }
            else // else, max out hp
            {
                //TODO: create monster instance and reference its hp, maxhp
            }
            currentHp.SetText(status.money + "/" + status.money); // display new hp text on stats bar
            yield return new WaitForSeconds(fillDelay);

        }
    }
}
