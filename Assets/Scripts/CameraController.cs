using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    public float distance;

    private void Awake()
    {
        //Follow();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            Debug.Log("Goodbye");
        }
        Follow();
    }

    void Follow()
    {
        this.transform.position = new Vector3(target.position.x, target.position.y, target.position.z - distance);
    }

}
