using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Script : MonoBehaviour
{
    enum State {Patrolling, Chasing, Attack, Wait, Traveling}
    
    State currentState;
    private NavMeshAgent agent;


    public Transform[] destinationPoints;
    int destinationIndex;

    [SerializeField]Transform player;
    [SerializeField]float visionRange;
    [SerializeField]float attackRange;

    [SerializeField]float waitingTime;
    float elapsedTime;
    [SerializeField]
    float patrolRange = 10f;
    [SerializeField] Transform patrolZone;
    [SerializeField] 
    [Range(0, 360)]
    float visionAngle;
    [SerializeField] LayerMask obstaclesMask;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    
    void Start()
    {
        currentState = State.Patrolling;
        destinationIndex = 0;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
            break;

            case State.Wait:
                Waiting();
            break;

            case State.Chasing:
                Chase();
            break;
            case State.Traveling:
                Travel();
            break;

            case State.Attack:
                Debug.Log("Attack");
                Attack();
            break;

            default:
                Patrol();
            break;
        }
    }


//------------------------------

    /*void Patrol()
    {
        agent.destination = destinationPoints[destinationIndex].position;

        if(Vector3.Distance(transform.position, destinationPoints[destinationIndex].position) < 1)
        {
            if(destinationIndex == (destinationPoints.Length - 1))
            {
                destinationIndex = 0;
                currentState = State.Wait;
                
            }
            else
            {
                destinationIndex ++;
                currentState = State.Wait;
            }
            
        }
        
        if(Vector3.Distance(transform.position, player.position) < visionRange)
        {
            currentState = State.Chasing;
        }
    }*/
    void Patrol()
    {
        Vector3 randomPosition;
        if(RandomPoint(patrolZone.position, patrolRange, out randomPosition))
        {
            agent.destination = randomPosition;
            Debug.DrawRay(randomPosition, Vector3.up * 5, Color.blue, 5f);
        }

        if(FindTarget())
        {
            currentState = State.Chasing;
        }
        currentState = State.Traveling;
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 point)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomPoint, out hit, 4, NavMesh.AllAreas))
        {
            point = hit.position;
            return true;
        }
        point = Vector3.zero;
        return false;
    }
    
    void Attack ()
    {
        if(Vector3.Distance(transform.position, player.position) < attackRange)
        {
            currentState = State.Attack;
        }
    }

    void Travel()
    {
        if(agent.remainingDistance <= 0.2)
        {
            currentState = State.Patrolling;
        }
        if(FindTarget())
        {
            currentState = State.Chasing;
        }
    }

    void Chase()
    {
        agent.destination = player.position;
        if(!FindTarget())
        {
            currentState = State.Patrolling;
        }
    }

    void Waiting()
    {
        
        elapsedTime += Time.deltaTime;
        

        if(elapsedTime >= waitingTime)
        {
            currentState = State.Patrolling;
            elapsedTime = 0;
        }
    }

    //--------------
    bool FindTarget()
    {
        if(Vector3.Distance(transform.position, player.position)< visionRange)
        {
            Vector3 directionToTarget = (player.position = transform.position).normalized;
            if(Vector3.Angle(transform.forward, directionToTarget)< visionAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, player.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstaclesMask))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDrawGizmos() 
    {
        foreach(Transform point in destinationPoints)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(point.position, 1);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(patrolZone.position, patrolRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
