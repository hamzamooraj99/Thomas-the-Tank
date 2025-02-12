using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BulletHandler : MonoBehaviour
{
    public float launchSpeed = 75.0f;
    public GameObject objectPrefab;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
            SpawnObject();
    }
    void SpawnObject()
    {
        Vector3 spawnPos = transform.position;
        Quaternion spawnRot = Quaternion.identity;

        Vector3 localXDirection = transform.TransformDirection(Vector3.forward);
        Vector3 velocity = localXDirection * launchSpeed;

        //Instantiate Object
        GameObject newObj = Instantiate(objectPrefab, spawnPos, spawnRot);

        Rigidbody rb = newObj.GetComponent<Rigidbody>();
        rb.linearVelocity = velocity;
    }
}
