using System.Runtime.CompilerServices;
using UnityEngine;

public class Enemy_Tank_FSM : ScriptableObject
{
    private enum State
{
    idle, 
    aggresiveAttack,  
    recover, 
    dead,
}
private State currentState;


private delegate void OnTargetReach();
private OnTargetReach onTargetReach;

[SerializeField] private CharacterController tankController;

private Vector3 targetPosition;
private Vector3 playerPosition;
private Vector3 batteryPosition;

private float playerNoticedDelay;
private const float NoticeDelay = 2.0f;

private void Awake()
{
    //set default state
    currentState = State.idle;
}

private void UpdateState()
{
    // check current
    if (currentState == State.moveTo)
        moveTo();
    else if (currentState == State.idle)
        idle();
    else if (currentState == State.recover)
        recover();
    else if (currentState == State.dead)
        dead() 
}

private void moveTo()
{
    //move to target
    // when target reached call the target reached 
}

private void onPlayerTankReach()
{
    // start tank aiming system
}

private void onBatteryReach()
{
    // reacharg
    // destroy the battery object
    // go to idle state
}

private void onDeath()
{
    // stop controller
    // make object static
    // explode
}
}