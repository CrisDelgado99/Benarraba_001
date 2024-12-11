using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PersonEscape : State
{
    private Transform zombieTransform;
    private NPCController npcController;
    private float fleeingSpeed;
    public PersonEscape(GameObject npc, NavMeshAgent agent, Animator npcAnimator, Transform playerTransform, List<Transform> personTransformList, List<Transform> zombieTransformList, Transform zombieTransform)
    : base(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList)
    {
        this.zombieTransform = zombieTransform;
        name = STATE.PERSONESCAPE;

        npcController = npc.GetComponent<NPCController>();

        //Agent settings
        agent.isStopped = false;
    }

    public override void Enter()
    {
        npcAnimator.SetTrigger("personRun");

        npcController.SetSpriteColor(npcController.PersonMaterial);

        agent.isStopped = false;
        fleeingSpeed = Random.Range(1.0f, 4f);

        base.Enter();
    }

    public override void Update()
    {
        Transform zombieAttackingTransform = npcController.AttackingZombie;
        if (zombieAttackingTransform != null && zombieAttackingTransform.gameObject.GetComponent<NPCController>().IsAttacking)
        {
            nextState = new PersonBeingAttacked(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        if (npcController.IsZombie)
        {
            nextState = new ZombieMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        if (zombieTransform == null || Vector3.Distance(npc.transform.position, zombieTransform.position) > 5)
        {
            nextState = new PersonMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        Vector3 directionAwayFromZombie = (npc.transform.position - zombieTransform.position).normalized;
        
        npc.transform.position +=  fleeingSpeed * Time.deltaTime * directionAwayFromZombie;

        Vector3 lookDirection = npc.transform.position + directionAwayFromZombie;
        npc.transform.LookAt(new Vector3(lookDirection.x, npc.transform.position.y, lookDirection.z));


        if (stage != EVENT.EXIT)
        {
            base.Update(); //Continue on Update while it doesn't have to exit
        }
    }

    public override void Exit() 
    {
        npcAnimator.ResetTrigger("personRun");

        base.Exit();
    }
}
