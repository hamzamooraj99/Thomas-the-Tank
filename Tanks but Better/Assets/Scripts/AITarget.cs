using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class AITarget : MonoBehaviour
{   
    public Transform target;
    public float attackDistance;

    private NavMeshAgent agent;
    private float distance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();        
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(agent.transform.position, target.position);
        if(distance < attackDistance){
            agent.isStopped = true;
        } else {
            agent.isStopped = false;
            agent.destination = target.position;
        }
    }
}
