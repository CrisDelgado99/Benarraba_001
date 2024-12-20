using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PersonMove : State
{
    NPCController npcController;
    List<GameObject> currentGroup; // The group the zombie will move through
    int closestGroupIndex;
    int currentIndex;

    private float timeSpentInState = 0f;
    private float timeToTransition = 3f;

    public PersonMove(GameObject npc, NavMeshAgent agent, Animator npcAnimator, Transform playerTransform, List<Transform> personTransformList, List<Transform> zombieTransformList)
    : base(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList)
    {
        name = STATE.PERSONMOVE;

        //Agent settings
        agent.speed = 2;
        agent.isStopped = false;

        //NPC movement
        npcController = npc.GetComponent<NPCController>();

        
    }

    public override void Enter()
    {
        npcAnimator.SetTrigger("personRun");

        npcController.SetSpriteColor(npcController.PersonMaterial);

        agent.isStopped = false;
        timeSpentInState = 0f;
        timeToTransition = Random.Range(8.0f, 10.0f);

        // Lock onto the closest group
        closestGroupIndex = npcController.GetClosestGroupIndex(npcController.PersonGroupOfCheckpointsList, npc.transform);
        currentGroup = npcController.PersonGroupOfCheckpointsList[closestGroupIndex];

        // Get the closest checkpoint within this group
        currentIndex = npcController.GetClosestCheckpointIndex(currentGroup);

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

        //Go from Move to Idle
        timeSpentInState += Time.deltaTime;

        if (timeSpentInState >= timeToTransition)
        {
            nextState = new PersonIdle(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
        }

        zombieTransform = npcController.NearestNpcOfTypeTransform(LevelManager.Instance.zombieTransformList, 4);
        if (zombieTransform != null)
        {
            nextState = new PersonEscape(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList, zombieTransform);
            stage = EVENT.EXIT;
        }

        //Move between patrol points
        if (agent.remainingDistance < 1)
        {
            if(currentIndex >= currentGroup.Count - 1)
            {
                LevelManager.Instance.NumberOfSavedPeople++;
                LevelManager.Instance.personTransformList.Remove(npc.transform);
                //GameManager.Instance.PlayerPoints += 300;
                GameObject.Destroy(npc);
                return;
            } 
            else
            {
                currentIndex++;
            }

            agent.SetDestination(currentGroup[currentIndex].transform.position);
        }
    }

    public override void Exit()
    {
        npcAnimator.ResetTrigger("personRun");

        base.Exit();
    }
}
