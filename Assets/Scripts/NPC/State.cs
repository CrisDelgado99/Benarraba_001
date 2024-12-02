using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class State
{
    public enum STATE
    {
        PERSONIDLE, PERSONMOVE, PERSONESCAPE, PERSONBEINGATTACKED, ZOMBIEIDLE, ZOMBIEMOVE, ZOMBIEPURSUEPLAYER, ZOMBIEPURSUENPC, ZOMBIEATTACKPLAYER, ZOMBIEATTACKNPC
    };

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    //Class attributes
    public STATE name;
    protected EVENT stage;
    protected GameObject npc;
    protected Animator npcAnimator;
    protected NavMeshAgent agent;
    protected Transform playerTransform;
    protected List<Transform> personTransformList;
    protected List<Transform> zombieTransformList;
    protected State nextState;

    private float visDistance = 10.0f;
    private float visAngle = 30.0f;
    private float attackDistance = 5.0f;

    //Constructor
    public State(GameObject npc, NavMeshAgent agent, Animator anim, Transform player, List<Transform> personTransformList, List<Transform> zombieTransformList)
    {
        this.npc = npc;
        this.agent = agent;
        this.npcAnimator = anim;
        this.playerTransform = player;
        this.personTransformList = personTransformList;
        this.zombieTransformList = zombieTransformList;
        this.stage = EVENT.ENTER;
    }

    public virtual void Enter() { stage = EVENT.UPDATE; } // I set next stage
    public virtual void Update() { stage = EVENT.UPDATE; } //I want to continue in update until something throws me out
    public virtual void Exit() { stage = EVENT.EXIT; } //I exit this state when it is set to exit

    public State Process()
    {
        Debug.Log("Processing state: " + this.GetType().Name + " with stage: " + stage);

        if (stage == EVENT.ENTER)
        {
            Enter();
        }

        if (stage == EVENT.UPDATE)
        {
            Update();
        }

        if (stage == EVENT.EXIT)
        {
            Exit();
            Debug.Log("Exiting " + this.GetType().Name + ", nextState: " + nextState?.GetType().Name);
            return nextState; // Transition to next state
        }

        return this; //If I don't exit, I return this state
    }
}
