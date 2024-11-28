using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PersonMove : State
{
    NPCController npcController;
    int currentIndex;

    public PersonMove(GameObject npc, NavMeshAgent agent, Animator anim, Transform player, List<Transform> personTransformList, List<Transform> zombieTransformList)
    : base(npc, agent, anim, player, personTransformList, zombieTransformList)
    {
        name = STATE.PERSONMOVE;

        //Agent settings
        agent.speed = 2;
        agent.isStopped = false;

        //NPC movement
        npcController = npc.GetComponent<NPCController>();

        
    }

    public override void Enter()
    {
        npcAnimator.SetTrigger("isMoving");
        npcAnimator.SetTrigger("isPerson");

        currentIndex = npcController.getClosestCheckpointIndex();

        base.Enter(); //After everything is done, set state to update
    }

    public override void Update()
    {
        //Go from Idle to move
        if (Random.Range(0, 100) < 10)
        {
            //1 out of 10 times, change the nextState to PersonIdle and call stage EXIT
            nextState = new PersonIdle(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        //Move between patrol points
        if(agent.remainingDistance < 1)
        {
            if(currentIndex >= npcController.CheckpointsList.Count - 1)
            {
                GameObject.Destroy(npc);
            } 
            else
            {
                currentIndex++;
            }
        }

        base.Update(); //Continue on update while it doesnt have to exit
    }

    public override void Exit()
    {
        npcAnimator.ResetTrigger("isMoving");
        npcAnimator.ResetTrigger("isPerson");
        base.Exit();
    }
}
