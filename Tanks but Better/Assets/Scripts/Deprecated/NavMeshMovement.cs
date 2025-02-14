using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class NavMeshMovement : MonoBehaviour
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

    [SerializeField] Transform tankFront; //Front of the Tank to calculate distance to travel

    [Header("Movement")]
    public List<string> NavMeshLayers; //Layers that the Tank can travel on
    public float acceleration = 3f;             //Copied from VehicleMovement - Tank optimised
    public float brakingForce = 3f;             //Copied from VehicleMovement - Tank optimised
    public float turnSpeed = 4f;                //Copied from VehicleMovement - Tank optimised
    public float turnSpeedWhileMoving = 6f;     //Copied from VehicleMovement - Tank optimised

    [Header("Target Settings")]
    public bool Patrol = false;
    public Transform target;

    [HideInInspector] public bool move;

    [Header("Debug")]
    public bool ShowGizmos;
    public bool Debugger;

    private Vector3 positionToFollow = Vector3.zero;
    private int currWaypoint; //idx for waypoints List object
    private float FOV = 80;
    private bool allowMove;
    private int NavMeshLayerBite; 
    private List<Vector3> waypoints = new List<Vector3>(); //List of waypoints set by Path Finding algo
    private float localMaxSpeed;
    private int fails;
    private float movementTorque;

    void Awake()
    {
        currWaypoint = 0;
        allowMove = true;
        move = true;  
    }

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = Vector3.zero;
        CalculateNavMashLayerBite();
    }

    void FixedUpdate()
    {
        UpdateWheels();
        ApplySteering();
        PathProgress();
    }

    private void CalculateNavMashLayerBite()
    {
        NavMeshLayerBite += 1 << NavMesh.GetAreaFromName(NavMeshLayers[0]);
    }

    private void PathProgress()
    {
        waypointManager();
        Movement();
        ListOptimiser();

        void waypointManager()
        {
            if(currWaypoint >= waypoints.Count)
                allowMove = false;
            else{
                positionToFollow = waypoints[currWaypoint];
                allowMove = true;
                if(Vector3.Distance(tankFront.position, positionToFollow) < 2)
                    currWaypoint++;
            }

            if(currWaypoint >= waypoints.Count - 3)
                CreatePath();
        }

        void CreatePath()
        {
            if(target == null){
                if(Patrol)
                    RandomPath();
                else{
                    allowMove = false;
                }
            }else{
                TargetPath(target);
            }
        }

        void ListOptimiser()
        {
            if(currWaypoint > 1 && waypoints.Count > 30){
                waypoints.RemoveAt(0);
                currWaypoint--;
            }
        }
    }

    public void RandomPath()
    {
        NavMeshPath path = new NavMeshPath();
        Vector3 sourcePos;

        if(waypoints.Count==0){
            Vector3 randDirection = Random.insideUnitSphere * 100;
            randDirection += transform.position;
            sourcePos = tankFront.position;
            Calculate(randDirection, sourcePos, tankFront.forward, NavMeshLayerBite);
        }else{
            sourcePos = waypoints[waypoints.Count - 1];
            Vector3 randPos = Random.insideUnitSphere * 100;
            randPos += sourcePos;
            Vector3 direction = (waypoints[waypoints.Count - 1] - waypoints[waypoints.Count - 2]).normalized;
            Calculate(randPos, sourcePos, direction, NavMeshLayerBite);
        }

        void Calculate(Vector3 dest, Vector3 source, Vector3 direction, int NavMeshBite)
        {
            if(NavMesh.SamplePosition(dest, out NavMeshHit hit, 150, 1 << NavMesh.GetAreaFromName(NavMeshLayers[0])) && NavMesh.CalculatePath(source, hit.position, NavMeshBite, path) && path.corners.Length > 2){
                if(CheckForAngle(path.corners[1], source, direction))
                    waypoints.AddRange(path.corners.ToList());
                else{
                    if(CheckForAngle(path.corners[2], source, direction))
                        waypoints.AddRange(path.corners.ToList());
                    else
                        fails++;
                }
            }else
                fails++;
        }
    }

    public void TargetPath(Transform target)
    {
        NavMeshPath path = new NavMeshPath();
        Vector3 sourcePos;

        if(waypoints.Count == 0){
            sourcePos = tankFront.position;
            Calculate(target.position, sourcePos, tankFront.forward, NavMeshLayerBite);
        }else{
            sourcePos = waypoints[waypoints.Count - 1];
            Vector3 direction = (waypoints[waypoints.Count - 1] - waypoints[waypoints.Count - 2]).normalized;
            Calculate(target.position, sourcePos, direction, NavMeshLayerBite);
        }

        void Calculate(Vector3 dest, Vector3 source, Vector3 direction, int NavMeshBite)
        {
            if (NavMesh.SamplePosition(dest, out NavMeshHit hit, 150, NavMeshBite) && NavMesh.CalculatePath(source, hit.position, NavMeshBite, path)){
                if (path.corners.ToList().Count() > 1&& CheckForAngle(path.corners[1], source, direction))
                    waypoints.AddRange(path.corners.ToList());
                else
                {
                    if (path.corners.Length > 2 && CheckForAngle(path.corners[2], source, direction))
                        waypoints.AddRange(path.corners.ToList());
                    else
                        fails++;
                }
            }
            else
                fails++;
        }
    }

    private bool CheckForAngle(Vector3 pos, Vector3 source, Vector3 direction)
    {
        Vector3 distance = (pos - source).normalized;
        float cosAngle = Vector3.Dot(distance, direction);
        float angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;
        if(angle < FOV)
            return true;
        else
            return false;
    }

    private void ApplyBrakes()
    {
        frontRight.brakeTorque = brakingForce;
        frontLeft.brakeTorque = brakingForce;
        backRight.brakeTorque = brakingForce;
        backLeft.brakeTorque = brakingForce;
    }

    private void UpdateWheels()
    {
        void UpdateWheel(WheelCollider collider, Transform mesh)
        {
            Vector3 position;
            Quaternion rotation;
            collider.GetWorldPose(out position, out rotation);
            mesh.position = position; mesh.rotation = rotation;
        }
        UpdateWheel(frontRight, frontRightMesh);
        UpdateWheel(frontLeft, frontLeftMesh);
        UpdateWheel(backRight, backRightMesh);
        UpdateWheel(backLeft, backLeftMesh);
    }

    void ApplySteering() //Need to edit to optimise to Tank and not steerable car
    {
        Vector3 relativeVector = transform.InverseTransformPoint(positionToFollow);
        float turn = relativeVector.x / relativeVector.magnitude;

        float leftTorque = acceleration;
        float rightTorque = acceleration;

        if(Mathf.Abs(turn) > 0.1f){
            leftTorque = (turn > 0) ? -turnSpeed : turnSpeed;
            rightTorque = (turn > 0) ? turnSpeed : -turnSpeed;
        }

        frontRight.motorTorque = rightTorque;
        frontLeft.motorTorque = leftTorque;
        backRight.motorTorque = rightTorque;
        backLeft.motorTorque = leftTorque;

        // float SteeringAngle = (relativeVector.x / relativeVector.magnitude) * maxSteeringAngle;
        // if (SteeringAngle > 15) localMaxSpeed = 100;
        // else localMaxSpeed = acceleration;

        // frontLeft.steerAngle = SteeringAngle;
        // frontRight.steerAngle = SteeringAngle;
    } 

    void Movement()
    {
        if(move && allowMove)
            allowMove = true;
        else
            allowMove = false;
        
        if(allowMove){
            frontRight.motorTorque = acceleration;
            frontLeft.motorTorque = acceleration;
            backRight.motorTorque = acceleration;
            backLeft.motorTorque = acceleration;
        }else{
            ApplyBrakes();
        }
    }

    private void OnDrawGizmos() // shows a Gizmos representing the waypoints and AI FOV
    {
        if (ShowGizmos == true)
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                if (i == currWaypoint)
                    Gizmos.color = Color.blue;
                else
                {
                    if (i > currWaypoint)
                        Gizmos.color = Color.red;
                    else
                        Gizmos.color = Color.green;
                }
                Gizmos.DrawWireSphere(waypoints[i], 2f);
            }
            CalculateFOV();
        }

        void CalculateFOV()
        {
            Gizmos.color = Color.white;
            float totalFOV = FOV * 2;
            float rayRange = 10.0f;
            float halfFOV = totalFOV / 2.0f;
            Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
            Vector3 leftRayDirection = leftRayRotation * transform.forward;
            Vector3 rightRayDirection = rightRayRotation * transform.forward;
            Gizmos.DrawRay(tankFront.position, leftRayDirection * rayRange);
            Gizmos.DrawRay(tankFront.position, rightRayDirection * rayRange);
        }
    }

}
