using System;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Tank Parts")]
    [SerializeField] GameObject tankBody;
    [SerializeField] GameObject tankTower;
    [SerializeField] GameObject tankCannon;
    
    [Header("Mouse Settings")]
    [SerializeField] private float sensitivityX = 120f;
    [SerializeField] private float sensitivityY = 120f;
    
    [Header("Camera Assignments")]
    [SerializeField] Transform cam = null;
    [SerializeField] Transform orientation = null;

    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRot; //Rotation around X axis (Vertical) - mouseY
    float yRot; //Rotation around Y axis (Horizontal) - mouseX
    float baseYRot;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        xRot -= mouseY * sensitivityY * multiplier;
        yRot += mouseX * sensitivityX * multiplier;

        xRot = Mathf.Clamp(xRot, -10f, 10f);
        tankTower.transform.rotation = tankBody.transform.rotation * Quaternion.Euler(0f, yRot, 0f);
        tankCannon.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);

        UpdateCamera();
    }

    void UpdateCamera()
    {
        cam.transform.rotation = tankBody.transform.rotation * Quaternion.Euler(xRot, yRot, 0f);
        orientation.transform.rotation = Quaternion.Euler(0f, yRot, 0f);
    }
}
