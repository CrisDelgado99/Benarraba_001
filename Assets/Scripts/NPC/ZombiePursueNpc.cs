using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePursueNpc : State
{
    private NPCController npcController;
    private Transform personTransform;

    public ZombiePursueNpc(GameObject npc, NavMeshAgent agent, Animator anim, Transform player, List<Transform> personTransformList, List<Transform> zombieTransformList, Transform personTransform)
    : base(npc, agent, anim, player, personTransformList, zombieTransformList)
    {
        this.personTransform = personTransform;
        name = STATE.PERSONESCAPE;

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
        if (!npcController.IsZombie)
        {
            nextState = new PersonMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        if (Vector3.Distance(npc.transform.position, personTransform.position) > 10)
        {
            nextState = new ZombieMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        if (Vector3.Distance(npc.transform.position, playerTransform.position) < 7)
        {
            nextState = new ZombiePursuePlayer(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        Vector3 directionTowardsNpc = (personTransform.position - npc.transform.position).normalized;
        float pursuingSpeed = 3f;
        npc.transform.position += pursuingSpeed * Time.deltaTime * directionTowardsNpc;

        if (stage != EVENT.EXIT)
        {
            base.Update(); //Continue on Update while it doesn't have to exit
        }
    }

    public override void Exit()
    {
        //npcAnimator.ResetTrigger("isMoving");
        //npcAnimator.ResetTrigger("isZombie");

        base.Exit();
    }
}
