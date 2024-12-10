using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieIdle : State
{
    private float timeSpentInState = 0f;
    private float timeToTransition = 3f;
    private NPCController npcController;

    public ZombieIdle(GameObject npc, NavMeshAgent agent, Animator anim, Transform player, List<Transform> personTransformList, List<Transform> zombieTransformList)
        : base(npc, agent, anim, player, personTransformList, zombieTransformList)
    {
        name = STATE.ZOMBIEIDLE;

        npcController = npc.GetComponent<NPCController>();
    }

    public override void Enter()
    {
        npcAnimator.SetTrigger("zombieIdle");

        npcController.SetSpriteColor(npcController.ZombieMaterial);

        agent.isStopped = true;
        timeSpentInState = 0f;
        timeToTransition = Random.Range(2.0f, 4.0f);
        base.Enter(); //After everything is done, set state to update
    }

    public override void Update()
    {
        //Set npc as person instead of zombie if has been saved---------------------------------------------------------------------------------------------
        if (!npcController.IsZombie)
        {
            nextState = new PersonMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        //Go from Idle to Move------------------------------------------------------------------------------------------------------------
        timeSpentInState += Time.deltaTime;
        
        if (timeSpentInState >= timeToTransition)
        {
            //3 out of 10 times, change the nextState to PersonMove and call stage EXIT
            nextState = new ZombieMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        //Pursue player if player is near and in FOV------------------------------------------------------------------------------------------------
        if (Vector3.Distance(npc.transform.position, playerTransform.position) < 10 && npcController.IsTransformInFOV(playerTransform, 60))
        {
            nextState = new ZombiePursuePlayer(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        //Pursue NPC if NPC is near and in FOV------------------------------------------------------------------------------------------------
        Transform zombieTransform = npcController.NearestNpcOfTypeTransform(LevelManager.Instance.personTransformList, 10);
        if (zombieTransform != null && npcController.IsTransformInFOV(zombieTransform, 60))
        {
            nextState = new ZombiePursueNpc(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList, zombieTransform);
            stage = EVENT.EXIT;
        }

    }

    public override void Exit()
    {
        npcAnimator.ResetTrigger("zombieIdle");

        base.Exit();
    }

}
