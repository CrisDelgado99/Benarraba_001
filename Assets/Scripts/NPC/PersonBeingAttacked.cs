using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PersonBeingAttacked : State
{
    private NPCController npcController;

    private float timeSpentInState = 0f;
    private float timeToTransition = 2f;

    public PersonBeingAttacked(GameObject npc, UnityEngine.AI.NavMeshAgent agent, Animator anim, Transform player, List<Transform> personTransformList, List<Transform> zombieTransformList)
        :base(npc, agent, anim, player, personTransformList, zombieTransformList)
    {
        name = STATE.PERSONBEINGATTACKED;

        npcController = npc.GetComponent<NPCController>();
    }

    public override void Enter()
    {
        //npcAnimator.SetTrigger("isBeingAttacked");
        //npcAnimator.SetTrigger("isPerson");

        npcController.SetSpriteColor(npcController.PersonMaterial);

        agent.isStopped = true;

        base.Enter();
    }

    public override void Update()
    {
        timeSpentInState += Time.deltaTime;

        if (timeSpentInState >= timeToTransition)
        {
            nextState = new ZombieIdle(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        if (stage != EVENT.EXIT)
        {
            base.Update(); //Continue on Update while it doesn't have to exit
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
