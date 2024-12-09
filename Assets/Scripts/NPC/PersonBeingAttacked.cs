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

        Debug.Log("I am " + npc.name + " and I am being attacked");

        agent.isStopped = true;

        npcController.SetSpriteColor(npcController.PersonMaterial);

        npcController.IsZombie = true;
        LevelManager.Instance.personTransformList.Remove(npc.transform);
        LevelManager.Instance.zombieTransformList.Add(npc.transform);

        npcController.AttackingZombie = null;

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
