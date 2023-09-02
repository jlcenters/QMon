using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    public float distance;



    void Update()
    {
        Follow();
    }

    void Follow()
    {
        transform.position = new Vector3(target.position.x, target.position.y, target.position.z - distance);
    }

}
