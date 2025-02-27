using UnityEngine;

public class Compass : MonoBehaviour
{
    public Transform viewDirection;
    public RectTransform compassElement;
    public float compassSize; //Distance between North and South Elements (used for Wrapping)

    void LateUpdate()
    {
        //Direction of camera
        Vector3 forwardVector = Vector3.ProjectOnPlane(viewDirection.forward, Vector3.up).normalized;
        //Signed angle relative to North Direction(Vector3.forward)
        float forwardAngle = Vector3.SignedAngle(forwardVector, Vector3.forward, Vector3.up);
        //Offset based on angle
        float compassOffset = (forwardAngle / 180f) * compassSize;
        compassElement.anchoredPosition = new Vector3(compassOffset, 0);
    }
}
