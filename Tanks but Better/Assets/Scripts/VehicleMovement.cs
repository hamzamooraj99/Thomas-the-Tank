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
    public float turnSpeedWhileMoving = 6f;

    private float currAcceleration;
    private float currBrakeForce;

    private void FixedUpdate()
    {
        //Input Handling
        currAcceleration = acceleration * Input.GetAxis("Vertical");
        if(Input.GetKey(KeyCode.Space))
            currBrakeForce = brakingForce;
        else
            currBrakeForce = 0f;

        float turnInput = Input.GetAxis("Horizontal");
        
        //Movement
        Accelerate();
        Brake();
        Turn(turnInput);

        //Update Wheel Meshes
        UpdateWheel(frontRight, frontRightMesh);
        UpdateWheel(frontLeft, frontLeftMesh);
        UpdateWheel(backRight, backRightMesh);
        UpdateWheel(backLeft, backLeftMesh);

    }

    void UpdateWheel(WheelCollider collider, Transform mesh)
    {
        Vector3 position;
        Quaternion rotation;

        collider.GetWorldPose(out position, out rotation);

        mesh.position = position; mesh.rotation = rotation;
    }

    void Accelerate()
    {
        frontRight.motorTorque = currAcceleration;
        frontLeft.motorTorque = currAcceleration;
        backRight.motorTorque = currAcceleration;
        backLeft.motorTorque = currAcceleration;
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
            if(currAcceleration < 0.5){
                frontRight.motorTorque = -turnInput * turnSpeed; backRight.motorTorque = -turnInput * turnSpeed;
                frontLeft.motorTorque = turnInput * turnSpeed; backLeft.motorTorque = turnInput * turnSpeed;
            } else {
                frontRight.motorTorque = -turnInput * turnSpeedWhileMoving; backRight.motorTorque = -turnInput * turnSpeedWhileMoving;
                frontLeft.motorTorque = turnInput * turnSpeedWhileMoving; backLeft.motorTorque = turnInput * turnSpeedWhileMoving;
            }
            
        }
    }
}
