using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //for NavMeshAgent

public class EnemySoldier : MonoBehaviour
{
    [Header("Tank Info")]
    [SerializeField] public GameObject tank;
    [SerializeField] float followDistance = 10;
    [SerializeField] public Transform followTarget;
    [SerializeField] public Transform firePoint;
    private int tankBattery;
    private Animator anim;
    private NavMeshAgent agent;
    private EnemyTankInfo tankInfo;
    private EnemyAI tankAI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public enum ai_states
    {
        idle,
        follow,
        attack
    }
    public ai_states current_state = ai_states.idle;

    void Start()
    {
        tank = GameObject.FindWithTag("Enemy");
        anim = GetComponent<Animator>();
        tankInfo = tank.GetComponent<EnemyTankInfo>();
        tankAI = tank.GetComponent<EnemyAI>();
        agent = GetComponent<NavMeshAgent>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(tankInfo.GetBattery() > 0)
        {
            if (Vector3.Distance(transform.position, followTarget.position) > followDistance)
            {
                current_state = ai_states.follow;
                agent.isStopped = false;
                anim.SetBool("idle", false);
                anim.SetBool("run", true);
                anim.SetBool("shoot", false);
            
                agent.SetDestination(followTarget.position);
            }
            else if (tankAI.isPlayerInFOV())
            {
                current_state = ai_states.attack;
                transform.LookAt(firePoint.transform);
                agent.SetDestination(transform.position);
                anim.SetBool("idle", false);
                anim.SetBool("run", false);
                anim.SetBool("shoot", true);
            }
            else
            {
                current_state = ai_states.idle;
                agent.SetDestination(transform.position);
                anim.SetBool("idle", true);
                anim.SetBool("run", false);
                anim.SetBool("shoot", false);
            }
        }
        else
        {
            anim.SetBool("isDead", true);
            // agent.enabled = false;
            gameObject.transform.DetachChildren();
            Destroy(gameObject, 2f);
        }
    }

    void stateIdle()
    {

    }

    void stateFollow()
    {


    }

    void stateAttack()
    {


    }
}
