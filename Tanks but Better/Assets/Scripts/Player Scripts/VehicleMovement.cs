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
    public float acceleration = 2.5f;
    public float brakingForce = 3f;
    public float maxSteerAngle = 20f;
    public float accelerationWhileTurning = 3f;

    [Header("Sound Clips")]
    [SerializeField] public AudioClip idle;
    [SerializeField] public AudioClip moving;

    private float currAcceleration;
    private float currBrakeForce;
    private float turnInput;
    private AudioSource engineAudio;
    private float targetVolume;

    void Awake()
    {
        engineAudio = GetComponent<AudioSource>();
        if(engineAudio == null) engineAudio = gameObject.AddComponent<AudioSource>();

        engineAudio.clip = idle;
        engineAudio.loop = true;
        engineAudio.volume = 0.5f;
        engineAudio.Play();
    }

    private void FixedUpdate()
    {
        PlayerControl();
        HandleEngineSound();
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
            float steeringAngle = maxSteerAngle * turnInput;
            frontLeft.steerAngle = Mathf.Lerp(frontLeft.steerAngle, steeringAngle, Time.deltaTime*5f);
            frontRight.steerAngle = Mathf.Lerp(frontRight.steerAngle, steeringAngle, Time.deltaTime*5f);
            backLeft.steerAngle = -Mathf.Lerp(backLeft.steerAngle, steeringAngle, Time.deltaTime*5f);
            backRight.steerAngle = -Mathf.Lerp(backRight.steerAngle, steeringAngle, Time.deltaTime*5f);


            // if(currAcceleration < 0.5){
            //     frontRight.motorTorque = -turnInput * turnSpeed; backRight.motorTorque = -turnInput * turnSpeed;
            //     frontLeft.motorTorque = turnInput * turnSpeed; backLeft.motorTorque = turnInput * turnSpeed;
            // } else {
            //     frontRight.motorTorque = -turnInput * accelerationWhileTurning; backRight.motorTorque = -turnInput * accelerationWhileTurning;
            //     frontLeft.motorTorque = turnInput * accelerationWhileTurning; backLeft.motorTorque = turnInput * accelerationWhileTurning;
            // }
            
        }else{
            frontLeft.steerAngle = 0;
            frontRight.steerAngle = 0;
            backLeft.steerAngle = 0;
            backRight.steerAngle = 0;
        }
    }

    void UpdateWheel(WheelCollider collider, Transform mesh)
    {
        Vector3 position;
        Quaternion rotation;

        collider.GetWorldPose(out position, out rotation);

        mesh.position = position; mesh.rotation = rotation;
    }

    private void HandleEngineSound()
    {
        float speed = Mathf.Abs(currAcceleration);
        targetVolume = Mathf.Lerp(0.3f, 1f, speed);

        if(speed > 0.1f){
            if(engineAudio.clip != moving){
                engineAudio.clip = moving;
                engineAudio.Play();
            }
        }else{
            if(engineAudio.clip != idle){
                engineAudio.clip = idle;
                engineAudio.Play();
            }
        }

        engineAudio.volume = Mathf.Lerp(engineAudio.volume, targetVolume, Time.deltaTime * 10f);
    }
}
