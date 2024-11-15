using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour{

    [SerializeField] private Transform follower;

    void LateUpdate(){
        transform.rotation = Quaternion.LookRotation(transform.position - follower.position);
    }
}
