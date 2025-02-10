using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform orientation;

    [Header("Movement")]
    [SerializeField] float movementSpeed = 5f;
    public float movementMultiplier = 5f;
    [SerializeField] float rbDrag = 5f;

    float horizontalMovement;
    float verticalMovement;

    [Header("Grounding")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 0.4f;
    bool isGrounded;
    
    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    
    Vector3 moveDirection;

    Rigidbody rb;

    private bool CheckGrounded()
    {
        foreach(Transform checkPoint in groundCheck){
            if(Physics.CheckSphere(checkPoint.position, groundDistance, groundMask))
                return true;
            
        }
        return false;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        isGrounded = CheckGrounded();
        // isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        // Debug.Log("Grounded: " + isGrounded);

        _Input();
        ControlDrag();      
    }

    void _Input()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    void ControlDrag()
    {
        rb.linearDamping = rbDrag;
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        if(isGrounded)
            rb.AddForce(moveDirection.normalized * movementSpeed * movementMultiplier, ForceMode.Acceleration);
        
        if(OnSlope()) {
            rb.AddForce(GetSlopeMoveDirection() * movementSpeed * movementMultiplier, ForceMode.Acceleration);
        }

        rb.useGravity = !OnSlope();
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, 0.5f + 0.1f)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
