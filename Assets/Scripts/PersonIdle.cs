using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PersonIdle : State
{
    public PersonIdle(GameObject npc, NavMeshAgent agent, Animator anim, Transform player, List<Transform> personTransformList, List<Transform> zombieTransformList)
        : base(npc, agent, anim, player, personTransformList, zombieTransformList)
    {
        name = STATE.PERSONIDLE;
    }

    public override void Enter()
    {
        npcAnimator.SetTrigger("isIdle");
        npcAnimator.SetTrigger("isPerson");
        base.Enter(); //After everything is done, set state to update
    }

    public override void Update()
    {
        //Go from Idle to move
        if(Random.Range(0, 100) < 30)
        {
            //3 out of 10 times, change the nextState to PersonMove and call stage EXIT
            nextState = new PersonMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT; 
        }
        base.Update(); //Continue on update while it doesnt have to exit
    }

    public override void Exit()
    {
        npcAnimator.ResetTrigger("isIdle");
        npcAnimator.ResetTrigger("isPerson");
        base.Exit();
    }

}
