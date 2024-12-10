using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator npcAnimator;
    private NPCController npcController;
    [SerializeField] private Transform playerTransform;
    private State currentState;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        npcAnimator = GetComponentInChildren<Animator>();
        npcController = GetComponent<NPCController>();

        if (npcController.IsZombie)
        {
            currentState = new ZombieIdle(this.gameObject, agent, npcAnimator, playerTransform, LevelManager.Instance.personTransformList, LevelManager.Instance.zombieTransformList);
        }
        else
        {
            currentState = new PersonIdle(this.gameObject, agent, npcAnimator, playerTransform, LevelManager.Instance.personTransformList, LevelManager.Instance.zombieTransformList);
        }
        
    }

    private void Update()
    {
        currentState = currentState.Process();
    }
}
