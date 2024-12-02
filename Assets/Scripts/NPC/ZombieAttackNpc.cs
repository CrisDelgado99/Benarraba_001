using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ZombieAttackNpc : State
{
    private NPCController npcController;
    private Transform personTransform;

    private float timeSpentInState = 0f;
    private float timeToTransition = 2f;

    public ZombieAttackNpc(GameObject npc, UnityEngine.AI.NavMeshAgent agent, Animator anim, Transform player, List<Transform> personTransformList, List<Transform> zombieTransformList, Transform personTransform)
    : base(npc, agent, anim, player, personTransformList, zombieTransformList)
    {
        this.personTransform = personTransform;
        name = STATE.ZOMBIEATTACKNPC;

        npcController = npc.GetComponent<NPCController>();
    }

    public override void Enter()
    {
        //npcAnimator.SetTrigger("isAttacking");
        //npcAnimator.SetTrigger("isZombie");

        npcController.SetSpriteColor(npcController.ZombieMaterial);

        agent.isStopped = true;

        personTransform.GetComponent<NPCController>().IsZombie = true;
        LevelManager.Instance.personTransformList.Remove(personTransform);
        LevelManager.Instance.zombieTransformList.Add(personTransform);

        base.Enter(); //After everything is done, set state to update
    }

    public override void Update()
    {
        timeSpentInState += Time.deltaTime;

        if (timeSpentInState >= timeToTransition)
        {
            nextState = new ZombieMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        if (stage != EVENT.EXIT)
        {
            base.Update(); //Continue on Update while it doesn't have to exit
        }
    }

    public override void Exit() { 
        base.Exit();
    }
}
