using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSurpriseEffect : MonoBehaviour
{
    [SerializeField] GameObject playerUI;

    public IEnumerator EncounterEffect()
    {
        playerUI.SetActive(true);
        yield return new WaitForSeconds(1f);
    }

    public void RemoveEffect()
    {
        playerUI.SetActive(false);
    }
}
