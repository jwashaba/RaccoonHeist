using UnityEngine;
using UnityEngine.AI;
using System;
using System.Runtime.CompilerServices;

public class EnemyMovement : MonoBehaviour
{
    public SpriteRenderer enemySR;
    public Rigidbody2D enemyRB;
    public float moveSpeed;
    public float idleTime;
    public bool looping;
    public Transform[] points;
    public float threshold;
    private int currPoint = 0;
    private int direction = 1;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    // navmesh chasing stuff
    public Transform player;
    public float chaseSpeed;
    public float chaseRadius; // to be removed and switched to flashlight cone?
    public float escapeDist;
    public float catchDist;
    private float distToPlayer;
    private bool isChasing = false;
    private bool isReturningToPatrol = false; // returns to patrol via nav mesh agent, not by interpolating
    private NavMeshAgent agent;

    // private CircleCollider2D chaseTrigger;
    private Vector2 facingDir = Vector2.down;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.stoppingDistance = catchDist;
        agent.speed = chaseSpeed;
        agent.enabled = false;

        // chaseTrigger = GetComponent<CircleCollider2D>();
        // chaseTrigger.radius = chaseRadius;
        // chaseTrigger.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        distToPlayer = Vector2.Distance(transform.position, player.position);

        if (!isChasing && !isReturningToPatrol)
        {
            Debug.Log(Vector3.Distance(transform.position, agent.destination));
            if (distToPlayer <= chaseRadius)
            {
                Debug.Log("starting chase");
                isChasing = true;
                isWaiting = false;
                waitTimer = 0f;
                if (!agent.enabled) agent.enabled = true;
            }
        }
        else if (isChasing)
        {
            if (!agent.enabled) agent.enabled = true;
            enemyRB.linearVelocity = Vector2.zero;
            agent.SetDestination(player.position);
            distToPlayer = Vector2.Distance(transform.position, player.position);
            if (distToPlayer <= catchDist)
            {
                // oh no!
                Debug.Log("player caught");
                // load lose screen?   
            }
            if (distToPlayer > escapeDist)
            {
                Debug.Log("ending chase");
                isReturningToPatrol = true;
                isChasing = false;
                // agent.ResetPath();
                // agent.enabled = false;

                Vector3 targetPatrolPos = points[currPoint].position;
                agent.SetDestination(targetPatrolPos);
                
                return;
            }
            return;
        }

        if (isReturningToPatrol)
        {
            // If we somehow lost the agent, bail out to manual
            if (!agent.enabled || points.Length == 0)
            {
                isReturningToPatrol = false;
                return;
            }

            // When the agent reaches the patrol point, disable it and resume manual patrol
            if (Vector3.Distance(agent.transform.position, agent.nextPosition) < 0.001f)
            {
                // Snap to the agent's nextPosition for cleanliness
                transform.position = agent.nextPosition;
                agent.ResetPath();
                agent.enabled = false;

                isReturningToPatrol = false;
                isWaiting = true;   // optional: pause at the node
                waitTimer = 0f;
            }
            return;
        }
        
        if (isWaiting && points.Length > 0)
        {
            enemyRB.linearVelocity = Vector2.zero;

            waitTimer += Time.deltaTime;
            if (waitTimer >= idleTime)
            {
                isWaiting = false;
                waitTimer = 0f;
                if (looping)
                {
                    currPoint = (currPoint + 1) % points.Length;
                }
                else // go back across path
                {
                    if (currPoint == 0) direction = 1;
                    else if (currPoint == points.Length - 1) direction = -1;
                    currPoint += direction;
                }
            }
            return;
        }

        Vector3 nextPoint = points[currPoint].position;
        Vector3 diff = nextPoint - transform.position;
        float maxStep = moveSpeed * Time.deltaTime;
        float dist = diff.magnitude;
        if (dist <= Math.Max(maxStep, threshold))
        {
            transform.position = nextPoint;
            enemyRB.linearVelocity = Vector2.zero;
            isWaiting = true;
            return;
        }

        Vector3 stepLength = diff.normalized * maxStep;
        // if (stepLength.sqrMagnitude > dist.sqrMagnitude) stepLength = dist;
        transform.position += stepLength;
        // need to flip sprite?
        if (Mathf.Abs(stepLength.x) > 0.1f)
        {
            if (stepLength.x > 0f)
            {
                enemySR.flipX = true;
                facingDir = Vector2.right;
            }
            else
            {
                enemySR.flipX = false;
                facingDir = Vector2.left;
            }
        }
    }

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.transform != player) return;
    //     Debug.Log("collision detected with: " + other.name);
    //     isChasing = true;
    //     waitTimer = 0f;
    //     isWaiting = false;
    // }
}
