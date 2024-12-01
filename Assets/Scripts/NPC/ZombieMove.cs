using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieMove : State
{
    NPCController npcController;
    int currentIndex;

    private float timeSpentInState = 0f;
    private float timeToTransition = 3f;

    public ZombieMove(GameObject npc, NavMeshAgent agent, Animator anim, Transform player, List<Transform> personTransformList, List<Transform> zombieTransformList)
    : base(npc, agent, anim, player, personTransformList, zombieTransformList)
    {
        name = STATE.ZOMBIEMOVE;

        //Agent settings
        agent.speed = 1f;
        agent.isStopped = false;

        //NPC movement
        npcController = npc.GetComponent<NPCController>();


    }

    public override void Enter()
    {
        //npcAnimator.SetTrigger("isMoving");
        //npcAnimator.SetTrigger("isZombie");

        npcController.SetSpriteColor(npcController.ZombieMaterial);

        agent.isStopped = false;
        timeSpentInState = 0f;
        timeToTransition = Random.Range(5.0f, 8.0f);

        currentIndex = npcController.GetClosestCheckpointIndex(npcController.ZombieCheckpointsList);

        base.Enter(); //After everything is done, set state to update
    }

    public override void Update()
    {
        if (!npcController.IsZombie)
        {
            nextState = new PersonMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        //Go from Move to Idle
        timeSpentInState += Time.deltaTime;

        if (timeSpentInState >= timeToTransition)
        {
            nextState = new ZombieIdle(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        if (Vector3.Distance(npc.transform.position, playerTransform.position) > 10)
        {
            nextState = new ZombiePursuePlayer(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        if (npcController.NearestNpcOfTypeTransform(LevelManager.Instance.personTransformList, 10) != null)
        {
            Transform personTransform = npcController.NearestNpcOfTypeTransform(LevelManager.Instance.personTransformList, 10);
            nextState = new ZombiePursueNpc(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList, personTransform);
            stage = EVENT.EXIT;
        }

        //Move between patrol points
        if (agent.remainingDistance < 1)
        {
            if (currentIndex >= npcController.ZombieCheckpointsList.Count - 1)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }

            agent.SetDestination(npcController.ZombieCheckpointsList[currentIndex].transform.position);
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
