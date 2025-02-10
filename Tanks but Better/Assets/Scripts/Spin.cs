using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float spinSpeed = 90.0f;
    

    void Update()
    {
        if(Input.GetAxis("Mouse X")>0)
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
        if(Input.GetAxis("Mouse X")<0)
            transform.Rotate(Vector3.up, -spinSpeed * Time.deltaTime);
    }
}
