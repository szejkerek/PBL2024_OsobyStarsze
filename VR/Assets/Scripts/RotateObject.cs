using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float speed = 30f;

    void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        float rotationAmount = speed * Time.deltaTime;
        transform.Rotate(new Vector3(0, rotationAmount, 0));
    }
}
