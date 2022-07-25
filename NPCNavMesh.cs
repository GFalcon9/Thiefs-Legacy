using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCNavMesh : MonoBehaviour
{
    [Header("NPC Type")]
    public bool enemy;
    public bool npc;

    bool moves;
    [Header("Movement")]
    public bool distractedByDrone;
    public bool distractedByObject;
    public bool moveAway;
    public bool patrolling;

    [Header("Type")]

    [Header("Nav Points")]
    [SerializeField] NavMeshAgent navMeshAgent;
    [Space]
    [SerializeField] Transform[] points;
    int destPoint = 0;
    [Space]
    public Transform distractionLocation;
    public Transform moveAwayLocation;

    [Header("Animation")]
    [SerializeField] NPCAnimations nPCAnimations;

    [Header("Wait Time")]
    public float timeLeft = 5f;
    public float timeToWait = 5f;


    [Header("Location")]
    NPCLocation nPCLocation;
    bool isInOriginalPosition;

    [Header("Enemy Type")]
    [SerializeField] EnemyTypes enemyTypes;

    [Header("Distract Object && Drone")]
    [SerializeField] GameObject distractObject;
    [SerializeField] DroneActions drone;

    void Awake()
    {
        nPCAnimations = GetComponent<NPCAnimations>();

        nPCLocation = GetComponent<NPCLocation>();

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.autoBraking = false;

        drone = FindObjectOfType<DroneActions>();

        if(enemy)
        {
            enemyTypes = GetComponent<EnemyTypes>();
        }

        if (moves)
        {
            GotoNextPoint();
        }

        timeLeft = timeToWait;
    }

    void GotoNextPoint()
    {
        if (points.Length == 0)
        {
            return;
        }

        if (navMeshAgent.remainingDistance > 0.1f)
        {
            moves = true;
        }
        else
        {
            moves = false;
        }

        navMeshAgent.destination = points[destPoint].position;

        destPoint = (destPoint + 1) % points.Length;
    }

    void GotoDroneDistraction()
    {
        moves = true;

        distractionLocation = GameObject.FindWithTag("Drone").transform;

        if (distractionLocation != null)
        {
            navMeshAgent.destination = distractionLocation.position;

            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

            if (navMeshAgent.remainingDistance > 0.1f)
            {
                moves = true;
            }
            else
            {
                moves = false;

                Timer();
            }
        }
    }

    void GotoObjectDistraction()
    {
        moves = true;

        distractionLocation = distractObject.transform;

        if (distractionLocation != null)
        {
            navMeshAgent.destination = distractionLocation.position;

            if (navMeshAgent.remainingDistance > 0.1f)
            {
                moves = true;
            }
            else
            {
                moves = false;

                Timer();
            }
        }
    }

    void MoveAway()
    {
        moves = true;

        if (moveAwayLocation != null)
        {
            navMeshAgent.destination = moveAwayLocation.position;

            if (navMeshAgent.remainingDistance > 0.1f)
            {
                moves = true;
            }
            else
            {
                moves = false;

                Timer();
            }
        }
    }

    void Update()
    {
        if (distractedByDrone)
        {
            if(drone.droneActive)
            {
                patrolling = false;

                if (npc)
                {
                    GotoDroneDistraction();
                }

                if (enemy && enemyTypes.canBeDistractedByDrone)
                {
                    GotoDroneDistraction();
                }
            }
            else
            {
                patrolling = true;

                distractedByDrone = false;
            }

        }

        if (distractedByObject)
        {
            patrolling = false;
            GotoObjectDistraction();
        }

        if (moveAway)
        {
            patrolling = false;
            MoveAway();
        }

        if(patrolling)
        {
            moves = true;

            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
            {
                GotoNextPoint();
            }
        }

        if (moves)
        {
            nPCAnimations.anim.SetBool("IsWalking", true);
        }
        else
        {
            nPCAnimations.anim.SetBool("IsWalking", false);
        }

        if(destPoint == 0)
        {
            isInOriginalPosition = true;
        }
        else
        {
            isInOriginalPosition = false;
        }

        if(isInOriginalPosition && navMeshAgent.remainingDistance < 0.5f)
        {
            nPCLocation.transform.rotation = nPCLocation.spawnPos;
        }
    }

    void Timer()
    {
        timeLeft -= 1 * Time.deltaTime;

        if (timeLeft <= 0)
        {
            timeLeft = 0;

            if (distractedByDrone)
            {
                distractedByDrone = false;

                navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            }

            if (distractedByObject)
            {
                distractedByObject = false;
            }

            if (moveAway)
            {
                moveAway = false;
            }

            patrolling = true;
        }
    }

    public void ResetTimer()
    {
        timeLeft = timeToWait;
    }

void OnDrawGizmos()
    {
        foreach (Transform waypoint in points)
        {
            if (enemy)
            {
                Gizmos.color = Color.red;
            }

            if (npc)
            {
                Gizmos.color = Color.blue;
            }

            Gizmos.DrawSphere(waypoint.position, 0.3f);
        }
    }
}

