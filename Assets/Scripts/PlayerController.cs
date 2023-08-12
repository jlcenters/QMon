using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public CharacterStatus storedData;
    bool isMoving;


    public LayerMask solidObjectLayer;
    public LayerMask longGrassLayer;

    Vector2 userInput; 
    Animator ani;


    public event Action OnEncounter;

    //TODO: potentially add instance of current active mon


    private void Awake()
    {
        ani = GetComponent<Animator>();
    }
    public void HandleUpdate()
    {
        if (!isMoving)
        {
            //using the Raw method will only return 1,-1, or 0
            userInput.x = Input.GetAxisRaw("Horizontal");
            userInput.y = Input.GetAxisRaw("Vertical");

            //removes diag movement
            if(userInput.x != 0)
            {
                userInput.y = 0;
            }

            if(userInput != Vector2.zero)
            {
                //set params of Animator
                ani.SetFloat("moveX", userInput.x);
                ani.SetFloat("moveY", userInput.y);

                var target = transform.position;
                target.x += userInput.x;
                target.y += userInput.y;

                if (IsWalkable(target))
                {
                    StartCoroutine(Move(target));
                    CheckForEncounters();
                }
            }
        }

        ani.SetBool("isMoving", isMoving);

    }






    bool IsWalkable(Vector3 target)
    {
        //is not walkable if overlaps with a 
        if (Physics2D.OverlapCircle(target, 0.2f, solidObjectLayer) != null)
        {
            return false;
        }

        return true;
    }
    IEnumerator Move(Vector3 target)
    {
        isMoving = true;
        // epsilon is a very small number used to represent error bounds 
        // until difference is super close to 0
        while ((target - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
        isMoving = false;
    }

    void CheckForEncounters()
    {
        if(Physics2D.OverlapCircle(transform.position, 0.2f, longGrassLayer))
        {
            if(UnityEngine.Random.Range(1, 101) <= 10){
                isMoving = false;
                //will utilize the observer design pattern to avoid a circular dependency
                //create event in player controller
                //game controller will subscribe
                //game objects subscribed will be notified
                OnEncounter();
            }
        }
    }

    /*public void MoveAround()
    {
        //use vel to move depending on key down
        userInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.Translate(speed * Time.deltaTime * userInput);
        
    }
    public void Run()
    {
        //use vel to move depending on key down
        userInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.Translate((speed * 1.5f) * Time.deltaTime * userInput);

    }*/
}
