using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    [Header("Colliders")]
    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider backLeft;

    [Header("Meshes")]
    [SerializeField] Transform frontRightMesh;
    [SerializeField] Transform frontLeftMesh;
    [SerializeField] Transform backRightMesh;
    [SerializeField] Transform backLeftMesh;

    [Header("Movement")]
    public float acceleration = 3f;
    public float brakingForce = 3f;
    public float turnSpeed = 4f;
    public float accelerationWhileTurning = 5f;

    private float currAcceleration;
    private float currBrakeForce;
    private float turnInput;

    private float smoothTorqueFrontRight;
    private float smoothTorqueFrontLeft;
    private float smoothTorqueBackRight;
    private float smoothTorqueBackLeft;

    void Awake()
    {
    }

    private void FixedUpdate()
    {
        PlayerControl();
    }

    private void PlayerControl()
    {
        //Input Handling
        currAcceleration = acceleration * Input.GetAxis("Vertical");
        if(Input.GetKey(KeyCode.Space))
            currBrakeForce = brakingForce;
        else
            currBrakeForce = 0f;

        turnInput = Input.GetAxis("Horizontal");

        ApplyMovement(currAcceleration, turnInput);
    }

    public void ApplyMovement(float acceleration, float turnInput)
    {
        Accelerate(acceleration);
        Brake();
        Turn(turnInput);

        //Update Wheel Meshes
        UpdateWheel(frontRight, frontRightMesh);
        UpdateWheel(frontLeft, frontLeftMesh);
        UpdateWheel(backRight, backRightMesh);
        UpdateWheel(backLeft, backLeftMesh);
    }

    void Accelerate(float acceleration)
    {
        frontRight.motorTorque = acceleration;
        frontLeft.motorTorque = acceleration;
        backRight.motorTorque = acceleration;
        backLeft.motorTorque = acceleration;
    }

    void Brake()
    {
        frontRight.brakeTorque = currBrakeForce;
        frontLeft.brakeTorque = currBrakeForce;
        backRight.brakeTorque = currBrakeForce;
        backLeft.brakeTorque = currBrakeForce;
    }
    
    void Turn(float turnInput)
    {
        if(turnInput != 0){
            float differentialTorque = turnInput * turnSpeed;

            // Smoothly apply torque
            smoothTorqueFrontRight = Mathf.Lerp(smoothTorqueFrontRight, -differentialTorque, Time.deltaTime * 10f);
            smoothTorqueFrontLeft = Mathf.Lerp(smoothTorqueFrontLeft, differentialTorque, Time.deltaTime * 10f);
            smoothTorqueBackRight = Mathf.Lerp(smoothTorqueBackRight, -differentialTorque, Time.deltaTime * 10f);
            smoothTorqueBackLeft = Mathf.Lerp(smoothTorqueBackLeft, differentialTorque, Time.deltaTime * 10f);

            frontRight.motorTorque += smoothTorqueFrontRight;
            frontLeft.motorTorque += smoothTorqueFrontLeft;
            backRight.motorTorque += smoothTorqueBackRight;
            backLeft.motorTorque += smoothTorqueBackLeft;


            // if(currAcceleration < 0.5){
            //     frontRight.motorTorque = -turnInput * turnSpeed; backRight.motorTorque = -turnInput * turnSpeed;
            //     frontLeft.motorTorque = turnInput * turnSpeed; backLeft.motorTorque = turnInput * turnSpeed;
            // } else {
            //     frontRight.motorTorque = -turnInput * accelerationWhileTurning; backRight.motorTorque = -turnInput * accelerationWhileTurning;
            //     frontLeft.motorTorque = turnInput * accelerationWhileTurning; backLeft.motorTorque = turnInput * accelerationWhileTurning;
            // }
            
        }
    }

    void UpdateWheel(WheelCollider collider, Transform mesh)
    {
        Vector3 position;
        Quaternion rotation;

        collider.GetWorldPose(out position, out rotation);

        mesh.position = position; mesh.rotation = rotation;
    }
}
