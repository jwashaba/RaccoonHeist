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
    public Animator animator;
    public Sprite idleUp;
    public Sprite idleRight;
    public Sprite idleDown;
    public Sprite idleLeft;

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

        animator = GetComponent<Animator>();
        animator.SetInteger("facingDirection", 2);
    }

    // Update is called once per frame
    void Update()
    {
        distToPlayer = Vector2.Distance(transform.position, player.position);

        if (!isChasing && !isReturningToPatrol)
        {
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

            if (!animator.enabled) animator.enabled = true;
            animator.SetBool("isWalking", true);

            // optional: face by agent velocity
            Vector2 v = agent.velocity;
            if (v.sqrMagnitude > 0.0001f)
            {
                int f = Mathf.Abs(v.x) >= Mathf.Abs(v.y) ? (v.x >= 0 ? 1 : 3) : (v.y >= 0 ? 0 : 2);
                animator.SetInteger("facingDirection", f);
            }

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
            // if (Vector3.Distance(agent.transform.position, agent.nextPosition) < 0.001f)
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                // Snap to the agent's nextPosition for cleanliness
                transform.position = agent.nextPosition;
                agent.ResetPath();
                agent.enabled = false;

                isReturningToPatrol = false;
                isWaiting = true;   // optional: pause at the node
                waitTimer = 0f;
                animator.SetBool("isWalking", false);
                Debug.Log("idling 2");
                SetIdle();
            }
            return;
        }

        if (isWaiting && points.Length > 0)
        {
            enemyRB.linearVelocity = Vector2.zero;
            Debug.Log("idling 1");

            SetIdle();

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
        if (Mathf.Abs(stepLength.x) > 0.001f)
        {
            SetWalking(stepLength.x > 0f ? 1 : 3);
        }
        else if (Mathf.Abs(stepLength.y) > 0.001f)
        {
            SetWalking(stepLength.y > 0f ? 0 : 2);
        }
        else
        {
            animator.SetBool("isWalking", false);
            SetIdle();
        }
    }

    void SetIdle()
    {
        if (animator.enabled) animator.enabled = false;

        int f = animator.GetInteger("facingDirection");
        switch (f)
        {
            case 0: enemySR.sprite = idleUp;    break;
            case 1: enemySR.sprite = idleRight; break;
            case 2: enemySR.sprite = idleDown;  break;
            case 3: enemySR.sprite = idleLeft;  break;
        }
        Debug.Log($"idle sprite set to: {enemySR.sprite?.name} (facing={f})");
    }

    void SetWalking(int facing)
    {
        if (!animator.enabled) animator.enabled = true;
        animator.SetBool("isWalking", true);
        animator.SetInteger("facingDirection", facing);
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
