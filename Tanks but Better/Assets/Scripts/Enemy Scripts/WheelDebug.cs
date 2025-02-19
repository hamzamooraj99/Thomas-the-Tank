using UnityEngine;

public class WheelDebug : MonoBehaviour
{
    [Header("Wheel Colliders")]
    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider backLeft;

    [Header("Debug Settings")]
    public bool showDebug = true;
    public float debugLineLength = 1f;

    void OnDrawGizmos()
    {
        if (!showDebug) return;

        // Draw debug lines for each wheel
        DrawWheelForce(frontRight, Color.red);
        DrawWheelForce(frontLeft, Color.magenta);
        DrawWheelForce(backRight, Color.blue);
        DrawWheelForce(backLeft, Color.black);
    }

    void DrawWheelForce(WheelCollider wheel, Color color)
    {
        if (wheel == null) return;

        // Get the wheel's position and rotation
        Vector3 wheelPosition;
        Quaternion wheelRotation;
        wheel.GetWorldPose(out wheelPosition, out wheelRotation);

        // Calculate the force direction based on motor torque
        float motorTorque = wheel.motorTorque;
        Vector3 forceDirection = wheel.transform.forward * Mathf.Sign(motorTorque);
        float forceMagnitude = Mathf.Abs(motorTorque);

        // Draw the force direction
        Gizmos.color = color;
        Gizmos.DrawLine(wheelPosition, wheelPosition + forceDirection * debugLineLength * forceMagnitude);

        // Label the wheel
        UnityEditor.Handles.Label(wheelPosition, $"{wheel.name}\nTorque: {motorTorque:F2}", new GUIStyle { normal = { textColor = color } });
    }
}
