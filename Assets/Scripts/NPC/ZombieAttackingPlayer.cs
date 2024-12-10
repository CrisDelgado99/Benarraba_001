using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAttackingPlayer : State
{
    private NPCController npcController;
    private PlayerController playerController;
    private float attackRate = 1f;
    private float timeSinceLastAttack = 0f;
    private int damage = 1;
    public ZombieAttackingPlayer(GameObject npc, NavMeshAgent agent, Animator npcAnimator, Transform playerTransform, List<Transform> personTransformList, List<Transform> zombieTransformList)
: base(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList)
    {
        name = STATE.ZOMBIEATTACKINGPLAYER;

        npcController = npc.GetComponent<NPCController>();
        playerController = playerTransform.GetComponent<PlayerController>();

    }
    public override void Enter()
    {
        npcAnimator.SetTrigger("zombieAttack");

        npcController.SetSpriteColor(npcController.ZombieMaterial);

        agent.isStopped = true;
        agent.speed = 0;

        attackRate = 3f;
        timeSinceLastAttack = 2f;

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

        if (Vector3.Distance(playerTransform.position, npc.transform.position) > 1.5)
        {
            nextState = new ZombieMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        timeSinceLastAttack += Time.deltaTime;
        if(timeSinceLastAttack >= attackRate)
        {
            playerController.DamagePlayer(damage);
            timeSinceLastAttack = 0f;
        }

        if (stage != EVENT.EXIT)
        {
            base.Update(); //Continue on Update while it doesn't have to exit
        }
    }

    public override void Exit()
    {
        npcAnimator.ResetTrigger("zombieAttack");

        base.Exit();
    }
}
