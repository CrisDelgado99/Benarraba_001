using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    #region Attributes
    [Header("Enemy Data")]
    [SerializeField] EnemyData enemyData;
    [SerializeField] private int currentLife;
    [SerializeField] private int maxLife;
    [SerializeField] private int enemyScorePoint;

    [Header("Patrol")]
    [SerializeField] private GameObject patrolPointsContainer;
    private List<Transform> patrolPointsList = new();
    private int destinationPoint = 0;
    private bool isChasingPlayer;

    private NavMeshAgent agent;
    private WeaponController weaponController;
    private Renderer enemyRenderer;

    private Transform playerTransform;
    #endregion

    #region Event Functions
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        weaponController = GetComponent<WeaponController>();
        enemyRenderer = GetComponentInChildren<Renderer>();

        //Enemy Data
        agent.speed = enemyData.Speed;
        currentLife = maxLife = enemyData.MaxLife;
        enemyRenderer.material = enemyData.EnemyMaterial;
        weaponController.ShootingRate = enemyData.ShootRate;

        //Get the children of patrolPointsContainer and add them to the array
        foreach (Transform child in patrolPointsContainer.transform)
        {
            patrolPointsList.Add(child);
        }

        destinationPoint = Random.Range(0, patrolPointsList.Count);

        GoToNextPatrolPoint();

    }

    private void Update()
    {
        if (!isChasingPlayer)
        {
            GoToNextPatrolPoint();
        }

        //Search player with RayCast
        SearchPlayer();
    }
    #endregion

    #region Other Methods
    /// <summary>
    /// This method handles the damage when the enemy recieves a bullet
    /// </summary>
    /// <param name="quantity">Damage quantity</param>
    public void DamageEnemy(int quantity)
    {
        currentLife -= quantity;
        if(currentLife <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// This method controlls that the enemy goes to next patrol point
    /// </summary>
    private void GoToNextPatrolPoint()
    {
        if (agent.remainingDistance < 3f)
        {
            //Choose next destinationPoint in the list
            //Cycling to the start point
            destinationPoint = (destinationPoint + 1) % patrolPointsList.Count;
        }

        //Restart the stopping distance to 0
        agent.stoppingDistance = 0;
        agent.SetDestination(patrolPointsList[destinationPoint].position);

    }

    /// <summary>
    /// This method makes the enemy look for the player and go towards it
    /// </summary>
    private void SearchPlayer()
    {
        NavMeshHit hit;

        //if there are no obstacles between agent and player 
        if(!agent.Raycast(playerTransform.position, out hit))
        {
            if(hit.distance <= 10f)
            {

                if(hit.distance > 5)
                {
                    agent.isStopped = false;
                    agent.SetDestination(playerTransform.position);
                    agent.stoppingDistance = 5f;
                    
                    isChasingPlayer = true;
                } 
                else
                {
                    //If less than 5 meters to player, stop recalculation
                    agent.isStopped = true;
                }

                transform.LookAt(playerTransform.position);
                if (hit.distance <= 6f)
                {

                    if (weaponController.CanShoot())
                    {
                        weaponController.Shoot();
                    }

                }
            }
            else
            {
                isChasingPlayer =false;
                agent.isStopped = false; //Activate recalculate path
            }
            
        } else
        {
            //There are obstacles between player and enemy
            isChasingPlayer = false;
            agent.isStopped = false;

        }
    }

    #endregion

}
