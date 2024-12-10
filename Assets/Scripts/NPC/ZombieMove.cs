using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class ZombieMove : State
{
    NPCController npcController;
    List<GameObject> currentGroup; // The group the zombie will move through
    int closestGroupIndex;
    int currentIndex; // The current checkpoint within the group

    private float timeSpentInState = 0f;
    private float timeToTransition = 3f;

    public ZombieMove(GameObject npc, NavMeshAgent agent, Animator anim, Transform player, List<Transform> personTransformList, List<Transform> zombieTransformList)
    : base(npc, agent, anim, player, personTransformList, zombieTransformList)
    {
        name = STATE.ZOMBIEMOVE;

        // Agent settings
        agent.speed = 1f;
        agent.isStopped = false;

        // NPC movement
        npcController = npc.GetComponent<NPCController>();
    }

    public override void Enter()
    {
        npcController.SetSpriteColor(npcController.ZombieMaterial);

        agent.isStopped = false;
        timeSpentInState = 0f;
        timeToTransition = Random.Range(5.0f, 8.0f);

        // Lock onto the closest group
        closestGroupIndex = npcController.GetClosestGroupIndex(npcController.ZombieGroupOfCheckpointsList, npc.transform);
        currentGroup = npcController.ZombieGroupOfCheckpointsList[closestGroupIndex];

        // Get the closest checkpoint within this group
        currentIndex = npcController.GetClosestCheckpointIndex(currentGroup);
        

        base.Enter(); // Call base implementation after setup
    }

    public override void Update()
    {
        // Check if zombie was saved and transition to PersonMove
        if (!npcController.IsZombie)
        {
            nextState = new PersonMove(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
            return;
        }

        // Transition to Idle state after a random duration
        timeSpentInState += Time.deltaTime;
        if (timeSpentInState >= timeToTransition)
        {
            nextState = new ZombieIdle(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
            return;
        }

        // Pursue player if nearby and within FOV
        if (Vector3.Distance(npc.transform.position, playerTransform.position) < 10 && npcController.IsTransformInFOV(playerTransform, 60))
        {
            nextState = new ZombiePursuePlayer(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList);
            stage = EVENT.EXIT;
            return;
        }

        // Pursue NPC if nearby and within FOV
        Transform zombieTransform = npcController.NearestNpcOfTypeTransform(LevelManager.Instance.personTransformList, 10);
        if (zombieTransform != null && npcController.IsTransformInFOV(zombieTransform, 60))
        {
            nextState = new ZombiePursueNpc(npc, agent, npcAnimator, playerTransform, personTransformList, zombieTransformList, zombieTransform);
            stage = EVENT.EXIT;
            return;
        }

        // Move through checkpoints within the locked group
        if (agent.remainingDistance < 1)
        {
            currentIndex++;
            if (currentIndex >= currentGroup.Count)
            {
                currentIndex = 0; // Loop back to the first checkpoint within the group
            }

            // Set the next destination
            GameObject currentDestination = currentGroup[currentIndex];
            agent.SetDestination(currentDestination.transform.position);
            npcController.LookAtWithNoYRotation(currentDestination.transform);
        }
    }

    public override void Exit()
    {
        currentGroup = null;
        currentIndex = -1;

        base.Exit();
    }
}