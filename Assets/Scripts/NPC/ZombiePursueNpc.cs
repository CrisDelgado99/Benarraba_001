using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePursueNpc : State
{
    private NPCController npcController;
    private Transform personTransform;

    public ZombiePursueNpc(GameObject npc, NavMeshAgent agent, Animator npcAnimator, Transform playerTransform, List<Transform> personTransformList, List<Transform> zombieTransformList, Transform personTransform)
    : base(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList)
    {
        this.personTransform = personTransform;
        name = STATE.ZOMBIEPURSUENPC;

        npcController = npc.GetComponent<NPCController>();
    }

    public override void Enter()
    {
        npcAnimator.SetTrigger("zombieWalk");

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

        //Attack npc if it is less than 1 away-------------------------------------------------------------------------------------------
        if (personTransform == null || Vector3.Distance(npc.transform.position, personTransform.position) < 1)
        {
            if(personTransform != null)
            {
                nextState = new ZombieAttackNpc(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList, personTransform);
                stage = EVENT.EXIT;
            }
        }

        //Stop following npc if it gets out of range or FOV--------------------------------------------------------------------------------
        if (Vector3.Distance(npc.transform.position, personTransform.position) > 6 || !npcController.IsTransformInFOV(personTransform, 60))
        {
            nextState = new ZombieMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        //Start following player if it is near and in FOV--------------------------------------------------------------------------------
        if (Vector3.Distance(npc.transform.position, playerTransform.position) < 6 && npcController.IsTransformInFOV(playerTransform, 60))
        {
            nextState = new ZombiePursuePlayer(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        //Actual movement of this State
        Vector3 directionTowardsNpc = (personTransform.position - npc.transform.position).normalized;
        float pursuingSpeed = 1f;
        npc.transform.position += pursuingSpeed * Time.deltaTime * directionTowardsNpc;
        npcController.LookAtWithNoYRotation(personTransform);

    }

    public override void Exit()
    {
        npcAnimator.ResetTrigger("zombieWalk");
    
        base.Exit();
    }
}
