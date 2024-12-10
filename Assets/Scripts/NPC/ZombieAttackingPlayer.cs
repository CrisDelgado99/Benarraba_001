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
    public ZombieAttackingPlayer(GameObject npc, NavMeshAgent agent, Animator anim, Transform playerTransform, List<Transform> personTransformList, List<Transform> zombieTransformList)
: base(npc, agent, anim, playerTransform, personTransformList, zombieTransformList)
    {
        name = STATE.ZOMBIEATTACKINGPLAYER;

        npcController = npc.GetComponent<NPCController>();
        playerController = playerTransform.GetComponent<PlayerController>();

    }
    public override void Enter()
    {
        //npcAnimator.SetTrigger("isAttacking");
        //npcAnimator.SetTrigger("isZombie");

        npcController.SetSpriteColor(npcController.ZombieMaterial);

        agent.isStopped = true;
        agent.speed = 0;

        attackRate = 1f;
        timeSinceLastAttack = 0f;

        Debug.Log("I AM ATTACKING");

        base.Enter(); //After everything is done, set state to update
    }

    public override void Update()
    {
        if (Vector3.Distance(playerTransform.position, npc.transform.position) > 1)
        {
            Debug.Log("Now move");
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
        base.Exit();
    }
}
