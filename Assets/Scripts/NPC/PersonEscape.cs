using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PersonEscape : State
{
    private Transform zombieTransform;
    private NPCController npcController;
    public PersonEscape(GameObject npc, NavMeshAgent agent, Animator anim, Transform player, List<Transform> personTransformList, List<Transform> zombieTransformList, Transform zombieTransform)
    : base(npc, agent, anim, player, personTransformList, zombieTransformList)
    {
        this.zombieTransform = zombieTransform;
        name = STATE.PERSONESCAPE;

        npcController = npc.GetComponent<NPCController>();

        //Agent settings
        agent.isStopped = false;
    }

    public override void Enter()
    {
        //npcAnimator.SetTrigger("isMoving");
        //npcAnimator.SetTrigger("isPerson");

        npcController.SetSpriteColor(npcController.PersonMaterial);

        agent.isStopped = false;

        base.Enter();
    }

    public override void Update()
    {

        if (npcController.IsZombie)
        {
            nextState = new ZombieMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        if (Vector3.Distance(npc.transform.position, zombieTransform.position) > 10)
        {
            nextState = new PersonMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }
       
        Vector3 directionAwayFromZombie = (npc.transform.position - zombieTransform.position).normalized;
        float fleeingSpeed = 3.5f;
        npc.transform.position +=  fleeingSpeed * Time.deltaTime * directionAwayFromZombie;
     

        if (stage != EVENT.EXIT)
        {
            base.Update(); //Continue on Update while it doesn't have to exit
        }
    }

    public override void Exit() 
    {
        //npcAnimator.ResetTrigger("isMoving");
        //npcAnimator.ResetTrigger("isPerson");

        base.Exit();
    }
}
