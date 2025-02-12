using UnityEngine;

public class Minimap : MonoBehaviour
{
    [Header("References")]
    public BoxCollider mapReference;
    public Camera playerCamera;
    public Transform playerTransform;

    [Header("References - UI")]
    public RectTransform mapContainer;
    public RectTransform playerIndicator;
    
    [Header("Parameters")]
    public Vector2 mapTextureSize = new Vector2(1024,1024);
    public Bounds mapBounds;

    [Header("Player Options")]
    public float minimapScale = 1f;
    public bool rotateMap;

    void Awake()
    {
        if(mapReference){
            mapReference.gameObject.SetActive(true);
            mapBounds = mapReference.bounds;
            mapReference.gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        //Assign References
        Transform rotationReference = playerCamera.transform;
        Transform positionReference = playerTransform;

        //Calculate unit scale and pos offset
        Vector2 unitScale = new Vector2(mapTextureSize.x / mapBounds.size.x, mapTextureSize.y / mapBounds.size.y);
        Vector3 positionOffset = mapBounds.center - positionReference.position;

        //Assign Values
        Vector2 mapPosition = new Vector2(positionOffset.x * unitScale.x, positionOffset.z * unitScale.y) * minimapScale;
        Quaternion mapRotation = default;
        Vector3 mapScale = new Vector3(minimapScale, minimapScale, minimapScale);
        Quaternion playerRotation = Quaternion.Euler(0, 0, -rotationReference.eulerAngles.y);

        if(rotateMap){
            mapRotation = Quaternion.Euler(0, 0, rotationReference.eulerAngles.y);
            Vector3 right = rotationReference.right;
            Vector3 forward = Vector3.ProjectOnPlane(rotationReference.forward, Vector3.up).normalized;
            mapPosition = mapPosition.x * new Vector2(right.x, -right.z) + mapPosition.y * new Vector2(-forward.x, forward.z);
            playerRotation = default;
        }

        //Set Values
        playerIndicator.rotation = playerRotation;
        mapContainer.localPosition = mapPosition;
        mapContainer.rotation = mapRotation;
        mapContainer.localScale = mapScale;
    }
}
