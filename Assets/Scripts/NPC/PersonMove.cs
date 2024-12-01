using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PersonMove : State
{
    NPCController npcController;
    int currentIndex;

    private float timeSpentInState = 0f;
    private float timeToTransition = 3f;

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
        //npcAnimator.SetTrigger("isMoving");
        //npcAnimator.SetTrigger("isPerson");

        npcController.SetSpriteColor(npcController.PersonMaterial);

        agent.isStopped = false;
        timeSpentInState = 0f;
        timeToTransition = Random.Range(8.0f, 10.0f);

        currentIndex = npcController.GetClosestCheckpointIndex(npcController.PersonCheckpointsList);

        base.Enter(); //After everything is done, set state to update
    }

    public override void Update()
    {
        if (npcController.IsZombie)
        {
            nextState = new ZombieMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        //Go from Move to Idle
        timeSpentInState += Time.deltaTime;

        if (timeSpentInState >= timeToTransition)
        {
            nextState = new PersonIdle(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        if (npcController.NearestNpcOfTypeTransform(LevelManager.Instance.zombieTransformList, 5) != null)
        {
            Transform zombieTransform = npcController.NearestNpcOfTypeTransform(LevelManager.Instance.zombieTransformList, 5);
            nextState = new PersonEscape(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList, zombieTransform);
            stage = EVENT.EXIT;
        }

        //Move between patrol points
        if (agent.remainingDistance < 1)
        {
            if(currentIndex >= npcController.PersonCheckpointsList.Count - 1)
            {
                GameObject.Destroy(npc);
            } 
            else
            {
                currentIndex++;
            }

            agent.SetDestination(npcController.PersonCheckpointsList[currentIndex].transform.position);
        }

        if (stage != EVENT.EXIT)
        {
            base.Update(); //Continue on Update while it doesn't have to exit
        }
    }

    public override void Exit()
    {
        //npcAnimator.ResetTrigger("isMoving");
        //npcAnimator.ResetTrigger("isPerson");
        
        base.Exit();
    }
}
