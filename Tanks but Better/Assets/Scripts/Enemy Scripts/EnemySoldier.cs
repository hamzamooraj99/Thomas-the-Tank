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

    [Header("Audio Stuffs")]
    private AudioSource footStepSource;
    [SerializeField] private AudioClip footStepClip;
    private AudioSource gunSource;
    [SerializeField] private AudioClip gunClip;

    private bool isRunning = false;
    private bool isShooting = false;

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

        // if (footStepSource == null)
        //     footStepSource = gameObject.AddComponent<AudioSource>();

        // if (gunSource == null)
        //     gunSource = gameObject.AddComponent<AudioSource>();

        // footStepSource.clip = footStepClip;
        // footStepSource.loop = true;
        // footStepSource.spatialBlend = 1.0f;
        // footStepSource.rolloffMode = AudioRolloffMode.Logarithmic;
        // footStepSource.minDistance = 5f;
        // footStepSource.maxDistance = 50f;
        // footStepSource.dopplerLevel = 1.0f;

        // gunSource.clip = gunClip;
        // gunSource.loop = false;
        // gunSource.spatialBlend = 1.0f;
        // gunSource.rolloffMode = AudioRolloffMode.Logarithmic;
        // gunSource.minDistance = 5f;
        // gunSource.maxDistance = 50f;
        // gunSource.dopplerLevel = 1.0f;
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

                // isRunning = true;
                // isShooting = false;

                // RunningSound();
                // ShootingSound();
            }
            else if (tankAI.isPlayerInFOV())
            {
                current_state = ai_states.attack;
                transform.LookAt(firePoint.transform);
                agent.SetDestination(transform.position);
                anim.SetBool("idle", false);
                anim.SetBool("run", false);
                anim.SetBool("shoot", true);

                // isRunning = false;
                // isShooting = true;

                // ShootingSound();
                // RunningSound();
            }
            else
            {
                current_state = ai_states.idle;
                agent.SetDestination(transform.position);
                anim.SetBool("idle", true);
                anim.SetBool("run", false);
                anim.SetBool("shoot", false);

                // isRunning = false;
                // isShooting = false;

                // ShootingSound();
                // RunningSound();
            }
        }
        else
        {
            anim.SetBool("isDead", true);
            Destroy(gameObject, 0.5f);
        }
    }

    void RunningSound()
    {
        if (isRunning)
        {
            if (!footStepSource.isPlaying)
                footStepSource.Play();
        }
        else
        {
            footStepSource.Stop();
        }
    }

    void ShootingSound()
    {
        if (isShooting)
        {
            if (!gunSource.isPlaying)
                gunSource.Play();
        }
        else
        {
            gunSource.Stop();
        }
    }
}
