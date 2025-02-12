using UnityEngine;

public class MinimapCam : MonoBehaviour
{
    [Header("Reference")]
    public Transform playerReference;
    [HideInInspector]public float playerOffset = 10f;

    void Update()
    {
        if(playerReference != null){
            transform.position = new Vector3(playerReference.position.x, playerReference.position.y+playerOffset, playerReference.position.z);
            transform.rotation = Quaternion.Euler(90f, playerReference.eulerAngles.y, 0f);
        }
    }
}
