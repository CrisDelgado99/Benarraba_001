using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PersonIdle : State
{
    private float timeSpentInState = 0f; 
    private float timeToTransition = 3f;
    private NPCController npcController;

    public PersonIdle(GameObject npc, NavMeshAgent agent, Animator npcAnimator, Transform playerTransform, List<Transform> personTransformList, List<Transform> zombieTransformList)
        : base(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList)
    {
        name = STATE.PERSONIDLE;

        npcController = npc.GetComponent<NPCController>();
    }

    public override void Enter()
    {
        npcAnimator.SetTrigger("personIdle");
       
        npcController.SetSpriteColor(npcController.PersonMaterial);
        
        agent.isStopped = true;
        timeSpentInState = 0f;
        timeToTransition = Random.Range(1.5f, 3.5f);
        base.Enter(); //After everything is done, set state to update
    }

    public override void Update()
    {
        if (npcController.IsZombie)
        {
            nextState = new ZombieMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        Transform zombieTransform = npcController.AttackingZombie;
        if (zombieTransform != null && zombieTransform.gameObject.GetComponent<NPCController>().IsAttacking)
        {
            nextState = new PersonBeingAttacked(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        timeSpentInState += Time.deltaTime;
        //Go from Idle to move
        
        if (timeSpentInState >= timeToTransition)
        {
            //3 out of 10 times, change the nextState to PersonMove and call stage EXIT
            nextState = new PersonMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT; 
        }

        zombieTransform = npcController.NearestNpcOfTypeTransform(LevelManager.Instance.zombieTransformList, 4);
        if (zombieTransform != null)
        {
            nextState = new PersonEscape(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList, zombieTransform);
            stage = EVENT.EXIT;
        }

        if (stage != EVENT.EXIT)
        {
            base.Update(); //Continue on Update while it doesn't have to exit
        }
    }

    public override void Exit()
    {
        npcAnimator.ResetTrigger("personIdle");
        
        base.Exit();
    }

}
