using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ZombieAttackNpc : State
{
    private NPCController npcController;
    private Transform personTransform;

    private float timeSpentInState = 0f;
    private float timeToTransition = 2f;

    public ZombieAttackNpc(GameObject npc, NavMeshAgent agent, Animator npcAnimator, Transform playerTransform, List<Transform> personTransformList, List<Transform> zombieTransformList, Transform personTransform)
    : base(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList)
    {
        this.personTransform = personTransform;
        name = STATE.ZOMBIEATTACKNPC;

        npcController = npc.GetComponent<NPCController>();
    }

    public override void Enter()
    {
        npcAnimator.SetTrigger("zombieAttack");
       
        npcController.SetSpriteColor(npcController.ZombieMaterial);

        agent.isStopped = true;

        npcController.IsAttacking = true;

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

        timeSpentInState += Time.deltaTime;

        if (timeSpentInState >= timeToTransition)
        {
            nextState = new ZombieMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit() {
        npcAnimator.ResetTrigger("zombieAttack");

        npcController.IsAttacking = false;
        base.Exit();
    }
}
