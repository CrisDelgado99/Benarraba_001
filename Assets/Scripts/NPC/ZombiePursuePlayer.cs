using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePursuePlayer : State
{
    private NPCController npcController;

    public ZombiePursuePlayer(GameObject npc, NavMeshAgent agent, Animator anim, Transform player, List<Transform> personTransformList, List<Transform> zombieTransformList)
    : base(npc, agent, anim, player, personTransformList, zombieTransformList)
    {
        name = STATE.ZOMBIEPURSUEPLAYER;

        npcController = npc.GetComponent<NPCController>();
    }

    public override void Enter()
    {
        //npcAnimator.SetTrigger("isMoving");
        //npcAnimator.SetTrigger("isZombie");

        npcController.SetSpriteColor(npcController.ZombieMaterial);

        agent.isStopped = false;

        base.Enter(); //After everything is done, set state to update
    }

    public override void Update()
    {
        //Set npc as person instead of zombie if has been saved----------------------------------------------------------------------------
        if (!npcController.IsZombie)
        {
            nextState = new PersonMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        if(Vector3.Distance(npc.transform.position, playerTransform.position) < 1)
        {
            nextState = new ZombieAttackingPlayer(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        //Stop following player if it is too far or out of FOV------------------------------------------------------------------------------
        if (Vector3.Distance(npc.transform.position, playerTransform.position) > 10 || !npcController.IsTransformInFOV(playerTransform, 60))
        {
            nextState = new ZombieMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        //Actual movement of the state
        Vector3 directionTowardsPlayer = (playerTransform.position - npc.transform.position).normalized;
        float pursuingSpeed = 2.5f;
        npc.transform.position += pursuingSpeed * Time.deltaTime * directionTowardsPlayer;
        npcController.LookAtWithNoYRotation(playerTransform);
    }

    public override void Exit()
    {
        //npcAnimator.ResetTrigger("isMoving");
        //npcAnimator.ResetTrigger("isZombie");

        base.Exit();
    }
}
