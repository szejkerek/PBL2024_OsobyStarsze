using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public GameObject anchorPoint;
    public float speed = 10f;//10
    public float distance = 25f; //25

    void Update()
    {
        transform.RotateAround(anchorPoint.transform.position, Vector3.up, (speed * Time.deltaTime)/15);
    }
}
