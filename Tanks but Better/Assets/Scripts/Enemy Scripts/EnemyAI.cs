using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public Transform target;
    public float moveSpeed = 2f;
    public float maxSteerAngle = 20f;
    public float accelerationWhileTurning = 2f;
    public float brakeForce = 2f;

    [Header("Path Following")]
    public float lookAheadDistance = 3f;
    public float stoppingDistance = 10f;

    [Header("Speed Adjustment")]
    public float minSpeed = 1f;
    public float maxAngle = 90f;
    
    [Header("Obstacle Avoidance")]
    public float avoidanceDistance = 2f;
    public float avoidanceForce = 2f;
    [SerializeField] public LayerMask obstacleLayer;

    [Header("Field of View (FOV)")]
    public float fovAngle = 90f;
    public float fovRange = 20f;

    [Header("Tower Movement")]
    public float towerRotationSpeed = 0.5f;
    public float towerScanSpeed = 0.5f;
    public float patrolTowerScanAngle = 90f;

    [Header("Patrolling")]
    [SerializeField] public Transform patrolAreaCenter;
    [SerializeField] public float patrolAreaRadius = 10f;
    [SerializeField] public float minPatrolDistance = 2f;
    [HideInInspector] public Transform[] waypoints;
    private int currentWaypoint;

    [Header("Tank Parts")]
    [SerializeField] GameObject tankBody;
    [SerializeField] GameObject tankTower;
    [SerializeField] GameObject tankCannon;

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

    private NavMeshAgent agent;
    private EnemyTankInfo tankInfo;
    private EnemyTankShoot tankShoot;
    private bool targetEscaped = false;
    private bool wasInFOV = false;
    private float searchTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = true;
        agent.speed = moveSpeed;
        agent.angularSpeed = 2f;
        agent.acceleration = 50f;
        agent.stoppingDistance = stoppingDistance;
        agent.autoBraking = true;
        // agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

        agent.SetDestination(GetRandomWaypoint());

        tankInfo = GetComponent<EnemyTankInfo>();
        tankShoot = GetComponentInChildren<EnemyTankShoot>();
    }

    void FixedUpdate()
    {
        if(target != null){
            if(isPlayerInFOV()){
                Debug.Log("PLAYER IN FOV");
                targetEscaped = false;
                wasInFOV = true;

                agent.SetDestination(target.position);
                Move();
                TowerMovement("attack");
                tankShoot.Shoot();
            }else if(wasInFOV && targetEscaped){
                Debug.Log("SEARCHING");
                SearchForTarget();
            }else if(tankInfo.GetBattery() < 50){
                Debug.Log("RETREAT");
                Retreat();
                TowerMovement("attack");
            }
            else{
                Debug.Log("PATROL");
                Patrol();
                TowerMovement("scan");
            }
        }
    }

    void Move()
    {
        Vector3 lookAheadPoint = GetLookAheadPoint(lookAheadDistance);
        Vector3 desiredVelocity = (lookAheadPoint - transform.position).normalized * moveSpeed;
        // Vector3 desiredVelocity = agent.desiredVelocity.normalized * moveSpeed;
        float forwardAmount = Vector3.Dot(transform.forward, desiredVelocity.normalized);
        float turnAmount = Vector3.Dot(transform.right, desiredVelocity.normalized);

        // Debug.Log($"Forward: {forwardAmount}, Turn: {turnAmount}");
        float adjustedSpeed = GetAdjustedSpeed(moveSpeed);

        float avoidanceTurn = AvoidObstacles();
        if(avoidanceTurn != 0)
            turnAmount += avoidanceTurn;

        if (agent.desiredVelocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.desiredVelocity.normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 2f * Time.deltaTime);
        }

        ApplyMovement(forwardAmount * adjustedSpeed, turnAmount * 2f);
        agent.nextPosition = transform.position;
    }

    void ApplyMovement(float acceleration, float turnInput)
    {
        if(isGrounded()){
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if(distanceToTarget <= stoppingDistance){
                Brake(brakeForce);
                Accelerate(0);
            }else{
                Brake(0);
                Accelerate(acceleration);
            }
            Turn(turnInput);
        }else{
            Accelerate(0);
        }

        UpdateWheel(frontRight, frontRightMesh);
        UpdateWheel(frontLeft, frontLeftMesh);
        UpdateWheel(backRight, backRightMesh);
        UpdateWheel(backLeft, backLeftMesh);

        void Accelerate(float acceleration)
        {
            // frontRight.motorTorque = acceleration;
            // frontLeft.motorTorque = acceleration;
            backRight.motorTorque = acceleration;
            backLeft.motorTorque = acceleration;

            frontRight.brakeTorque = 0f;
            frontLeft.brakeTorque = 0f;
            backRight.brakeTorque = 0f;
            backLeft.brakeTorque = 0f;

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
                frontLeft.steerAngle = Mathf.Lerp(frontLeft.steerAngle, 0, Time.deltaTime * 5f);
                frontRight.steerAngle = Mathf.Lerp(frontRight.steerAngle, 0, Time.deltaTime * 5f);
                backLeft.steerAngle = Mathf.Lerp(backLeft.steerAngle, 0, Time.deltaTime * 5f);
                backRight.steerAngle = Mathf.Lerp(backRight.steerAngle, 0, Time.deltaTime * 5f);
            }
        }

        void UpdateWheel(WheelCollider collider, Transform mesh)
        {
            Vector3 position;
            Quaternion rotation;

            collider.GetWorldPose(out position, out rotation);

            mesh.position = position;
            mesh.rotation = rotation;
        }
    
        bool isGrounded()
        {
            return frontRight.isGrounded && frontLeft.isGrounded && backRight.isGrounded && backLeft.isGrounded;
        }

        void Brake(float brakeForce)
        {
            frontRight.brakeTorque = brakeForce;
            frontLeft.brakeTorque = brakeForce;
            backRight.brakeTorque = brakeForce;
            backLeft.brakeTorque = brakeForce;
        }    
    }

    Vector3 GetLookAheadPoint(float distance)
    {
        if(agent.path.corners.Length > 1){
            float remainingDistance = distance;
            for(int i=0 ; i<agent.path.corners.Length-1 ; i++){
                Vector3 start = agent.path.corners[i];
                Vector3 end = agent.path.corners[i+1];
                float segmentLength = Vector3.Distance(start, end);

                if(remainingDistance <= segmentLength)
                    return Vector3.Lerp(start, end, remainingDistance/segmentLength);
                else
                    remainingDistance -= segmentLength;
            }
        }
        return agent.path.corners[agent.path.corners.Length-1];
    }

    float GetAdjustedSpeed(float baseSpeed)
    {
        float turnAngle = CalculateTurnAngle();

        if(turnAngle > maxAngle){
            float speedReduction = Mathf.InverseLerp(0, maxAngle, turnAngle);
            return Mathf.Lerp(baseSpeed, minSpeed, speedReduction);
        }
        return baseSpeed;

        float CalculateTurnAngle()
        {
            if(agent.path.corners.Length > 2){
                Vector3 directionToNextCorner = (agent.path.corners[1] - transform.position).normalized;
                Vector3 directionToCornerAfterNext = (agent.path.corners[2] - agent.path.corners[1]).normalized;

                return Vector3.Angle(directionToNextCorner, directionToCornerAfterNext);
            }
            return 0f;
        }
    }

    float AvoidObstacles()
    {
        float avoidanceTurn = 0f;

        if(SphereCast(transform.forward, avoidanceDistance))
            avoidanceTurn = -Mathf.Sign(Vector3.Dot(transform.right, transform.forward)) * avoidanceForce;
        else if(SphereCast(transform.forward+transform.right*0.5f, avoidanceDistance))
            avoidanceTurn = -avoidanceForce;
        else if(SphereCast(transform.forward-transform.right*0.5f, avoidanceDistance))
            avoidanceTurn = avoidanceForce;
        
        return avoidanceTurn;

        bool SphereCast(Vector3 direction, float distance)
        {
            RaycastHit hit;
            float sphereRadius = 0.5f;
            if(Physics.SphereCast(transform.position, sphereRadius, direction, out hit, distance, obstacleLayer)){
                // Debug.DrawRay(transform.position, direction*distance, Color.yellow);
                return true;
            }
            // Debug.DrawRay(transform.position, direction*distance, Color.green);
            return false;
        }
    }

    bool isPlayerInFOV()
    {
        if(target == null) return false;

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float angleToTarget = Vector3.Angle(tankTower.transform.forward, directionToTarget);

        if(angleToTarget < fovAngle/2f){
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if(distanceToTarget <= fovRange){
                RaycastHit hit;
                if(Physics.Raycast(transform.position, directionToTarget, out hit, fovRange)){
                    if(hit.transform == target)
                        return true;
                }
            }
        }

        if(!targetEscaped && wasInFOV){
            targetEscaped = true;
        }
        return false;
    }

    void Retreat()
    {
        Debug.Log("RETREAT");
        Transform healthItem = FindNearestHealthItem();
        Debug.Log($"Looking for {healthItem.name}");
        if(healthItem != null){
            agent.SetDestination(healthItem.position);
            agent.stoppingDistance = 0f;
        }
        Move();
        agent.stoppingDistance = stoppingDistance;

        Transform FindNearestHealthItem()
        {
            GameObject[] healthItems = GameObject.FindGameObjectsWithTag("HealthItem");
            return healthItems
                .Select(item => item.transform)
                .OrderBy(item => Vector3.Distance(transform.position, item.position))
                .FirstOrDefault();
        }
    }

    void Patrol()
    {
        Debug.Log("PATROL");
        if(Vector3.Distance(transform.position, agent.destination) < 2f)
            agent.SetDestination(GetRandomWaypoint());
            Move();
    }

    Vector3 GetRandomWaypoint()
    {
        Vector2 randomPoint = Random.insideUnitCircle * patrolAreaRadius;
        Vector3 randomPos = new Vector3(randomPoint.x, 0f, randomPoint.y) + patrolAreaCenter.transform.position;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomPos, out hit, patrolAreaRadius, NavMesh.AllAreas))
            return hit.position;

        return transform.position;
    }

    void TowerMovement(string action)
    {
        if(action == "attack"){
            Vector3 directionToTarget = (target.position - tankTower.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, tankBody.transform.up);
            tankTower.transform.rotation = Quaternion.Slerp(tankTower.transform.rotation, targetRotation, Time.deltaTime*towerRotationSpeed);
        }else if(action == "scan"){
            float scanAngle = Mathf.Sin(Time.time * towerScanSpeed) * patrolTowerScanAngle;
            Quaternion targetRotation = Quaternion.Euler(0f, scanAngle, 0f) * tankBody.transform.rotation;

            tankTower.transform.rotation = Quaternion.Slerp(tankTower.transform.rotation, targetRotation, Time.deltaTime*towerRotationSpeed);
        }
    }

    void SearchForTarget()
    {
        float searchTimeLimit = 5f;
        searchTimer += Time.deltaTime;

        if(searchTimer >= searchTimeLimit){
            targetEscaped = false;
            wasInFOV = false;
            searchTimer = 0f;
            Patrol();
        }else{
            agent.SetDestination(target.position);
            Move();
            Debug.Log($"SEARCH: {searchTimer} / {searchTimeLimit}");
        }
    }

    void OnDrawGizmos()
    {
        // Draw FOV cone
        Gizmos.color = Color.yellow;
        float halfFOV = fovAngle / 2f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * tankTower.transform.forward;
        Vector3 rightRayDirection = rightRayRotation * tankTower.transform.forward;

        Gizmos.DrawRay(transform.position, leftRayDirection * fovRange);
        Gizmos.DrawRay(transform.position, rightRayDirection * fovRange);
    }

    // void Move()

    // void ApplyMovement(float acceleration, float turnInput)

    // Vector3 GetLookAheadPoint(float distance)

    // float GetAdjustedSpeed(float baseSpeed)

    // float AvoidObstacles()

}
