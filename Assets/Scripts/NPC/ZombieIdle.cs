using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieIdle : State
{
    private float timeSpentInState = 0f;
    private float timeToTransition = 3f;
    private NPCController npcController;

    public ZombieIdle(GameObject npc, NavMeshAgent agent, Animator anim, Transform player, List<Transform> personTransformList, List<Transform> zombieTransformList)
        : base(npc, agent, anim, player, personTransformList, zombieTransformList)
    {
        name = STATE.ZOMBIEIDLE;

        npcController = npc.GetComponent<NPCController>();
    }

    public override void Enter()
    {
        //npcAnimator.SetTrigger("isIdle");
        //npcAnimator.SetTrigger("isZombie");

        npcController.SetSpriteColor(npcController.ZombieMaterial);

        agent.isStopped = true;
        timeSpentInState = 0f;
        timeToTransition = Random.Range(2.0f, 4.0f);
        base.Enter(); //After everything is done, set state to update
    }

    public override void Update()
    {
        if (!npcController.IsZombie)
        {
            nextState = new PersonMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        timeSpentInState += Time.deltaTime;
        
        //Go from Idle to move
        if (timeSpentInState >= timeToTransition)
        {
            //3 out of 10 times, change the nextState to PersonMove and call stage EXIT
            nextState = new ZombieMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        if (Vector3.Distance(npc.transform.position, playerTransform.position) < 7)
        {
            nextState = new ZombiePursuePlayer(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        if (npcController.NearestNpcOfTypeTransform(LevelManager.Instance.personTransformList, 10) != null)
        {
            Transform zombieTransform = npcController.NearestNpcOfTypeTransform(LevelManager.Instance.personTransformList, 10);
            nextState = new ZombiePursueNpc(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList, zombieTransform);
            stage = EVENT.EXIT;
        }

        if (stage != EVENT.EXIT)
        {
            base.Update(); //Continue on Update while it doesn't have to exit
        }
    }

    public override void Exit()
    {
        //npcAnimator.ResetTrigger("isIdle");
        //npcAnimator.ResetTrigger("isPerson");

        base.Exit();
    }

}
