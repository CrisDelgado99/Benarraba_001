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
        if (!npcController.IsZombie)
        {
            nextState = new PersonMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        if (Vector3.Distance(npc.transform.position, playerTransform.position) > 10)
        {
            nextState = new ZombieMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        Vector3 directionTowardsPlayer = (playerTransform.position - npc.transform.position).normalized;
        float pursuingSpeed = 3f;
        npc.transform.position += pursuingSpeed * Time.deltaTime * directionTowardsPlayer;

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
