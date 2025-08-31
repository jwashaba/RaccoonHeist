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
    public PlayerStates pstates;
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
    
    public Flashlight flashlight;
    [SerializeField] Transform indicatorTransform;
    [SerializeField] SpriteRenderer indicatorSR;
    private float flashlightAngle;
    bool soundAlert = false;
    bool hasInteracted = false;

    public Sprite exclamationIndicator;
    public Sprite questionIndicator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // indicatorTransform = transform.Find("ChildName/Indicator");
        // indicatorSR = indicatorTransform.GetComponent<SpriteRenderer>();
        
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

    void SetFlashlightDirectionWhenAgent()
    {
        if (agent.velocity.sqrMagnitude > 0.0001f)
        {
            flashlight.angle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg + 45f;
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
        // Debug.Log($"idle sprite set to: {enemySR.sprite?.name} (facing={f})");
    }

    void SetWalking(int facing)
    {
        if (!animator.enabled) animator.enabled = true;
        animator.SetBool("isWalking", true);
        animator.SetInteger("facingDirection", facing);
    }
    
    // Update is called once per frame
    void Update()
    {
        // VISUAL INDICATOR ANIMATIONS
        if (isChasing)
        {
            soundAlert = true;
            if (soundAlert == true && hasInteracted == false)
            {
                SoundManager.Instance.Play(SoundManager.SoundType.Alert);
                hasInteracted = true;
            }
                indicatorSR.sprite = exclamationIndicator;
            float pulse = (Mathf.Cos(Time.time * 5f) + 1f) * 0.5f; 
            // pulse goes 0 → 1
            float scale = Mathf.Lerp(2f, 3f, pulse);
            indicatorTransform.localScale = new Vector3(scale, scale, 1f);
        }
        else if (isReturningToPatrol)
        {
            indicatorSR.sprite = questionIndicator;
            float pulse = (Mathf.Cos(Time.time * 5f) + 1f) * 0.5f; 
            // pulse goes 0 → 1
            float scale = Mathf.Lerp(2f, 3f, pulse);
            indicatorTransform.localScale = new Vector3(scale, scale, 1f);
        }
        else
        {
            indicatorTransform.localScale = new Vector3(0f, 0f, 1f);
        }
        
        distToPlayer = Vector2.Distance(transform.position, player.position);

        if (!isChasing && !isReturningToPatrol)
        {
            if (distToPlayer <= chaseRadius && pstates.detection >= 1f)
            {
                isChasing = true;
                isWaiting = false;
                waitTimer = 0f;
                EnableAgent();
            }
        }
        else if (isChasing)
        {
            EnableAgent(); // idempotent
            agent.SetDestination(player.position);

            if (!animator.enabled) animator.enabled = true;
            animator.SetBool("isWalking", true);

            Vector2 v = agent.velocity;
            if (v.sqrMagnitude > 0.0001f)
            {
                int f = Mathf.Abs(v.x) >= Mathf.Abs(v.y) ? (v.x >= 0 ? 1 : 3) : (v.y >= 0 ? 0 : 2);
                animator.SetInteger("facingDirection", f);
            }

            distToPlayer = Vector2.Distance(transform.position, player.position);

            SetFlashlightDirectionWhenAgent();

            if (distToPlayer <= catchDist)
            {
                // oh no!
                SceneManager.Instance.gameIsPaused = true;
                Time.timeScale = 0f;
                isChasing = false;
                SceneManager.Instance.LoadScene("LossScene");
                Destroy(this);
                return;
            }

            if (pstates.detection <= 0f)
            {
                isChasing = false;
                isReturningToPatrol = true;
                // Send agent back to current patrol node
                agent.SetDestination(points[currPoint].position);
                return;
            }
            return;
        }

        if (isReturningToPatrol)
        {
            soundAlert = true;
            hasInteracted = false;
            // optional: face by agent velocity
            Vector2 v = agent.velocity;
            if (v.sqrMagnitude > 0.0001f)
            {
                int f = Mathf.Abs(v.x) >= Mathf.Abs(v.y) ? (v.x >= 0 ? 1 : 3) : (v.y >= 0 ? 0 : 2);
                animator.SetInteger("facingDirection", f);
            }

            SetFlashlightDirectionWhenAgent();

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
                // Agent done → hand back to RB patrol
                DisableAgent();
                isReturningToPatrol = false;
                isWaiting = true;
                waitTimer = 0f;
                animator.SetBool("isWalking", false);
                SetIdle();
            }
            return;
        }

        if (agent.enabled) DisableAgent();
        
        if (isWaiting && points.Length > 0)
        {
            enemyRB.linearVelocity = Vector2.zero;

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
        Vector2 diff = nextPoint - transform.position;
        float maxStep = moveSpeed * Time.deltaTime;
        float dist = diff.magnitude;

        if (dist <= Mathf.Max(maxStep, threshold))
        {
            // Snap to the point and stop
            transform.position = nextPoint;
            enemyRB.linearVelocity = Vector2.zero;
            isWaiting = true;
            return;
        }

        // Normalize direction and set velocity
        Vector2 dir = diff.normalized;
        enemyRB.linearVelocity = dir * moveSpeed * Time.deltaTime * 125f;

        // move by physics velocity, not manual transform
        // (the rigidbody handles actual position integration)
        if (Mathf.Abs(dir.x) > 0.001f)
        {
            SetWalking(dir.x > 0f ? 1 : 3);
        }
        else if (Mathf.Abs(dir.y) > 0.001f)
        {
            SetWalking(dir.y > 0f ? 0 : 2);
        }
        else
        {
            animator.SetBool("isWalking", false);
            SetIdle();
        }

        // update flashlight angle to match velocity direction
        if (!isReturningToPatrol && !isChasing)
        {
            if (enemyRB.linearVelocity.sqrMagnitude > 0.0001f)
            {
                flashlight.angle = Mathf.Atan2(enemyRB.linearVelocity.y, enemyRB.linearVelocity.x) * Mathf.Rad2Deg + 45f;
            }
        }
    }
    
    void EnableAgent()
    {
        if (!agent.enabled) agent.enabled = true;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.updatePosition = true; // agent moves transform
        agent.stoppingDistance = catchDist;
        agent.speed = chaseSpeed;

        // Take RB out of the sim during agent control
        enemyRB.linearVelocity = Vector2.zero;
        enemyRB.isKinematic = true;         // or enemyRB.simulated = false;
    
        // Optional: reduce weird circling
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        // Or keep avoidance but stagger priorities per enemy (0..99). Lower = higher priority.
        // agent.avoidancePriority = UnityEngine.Random.Range(20, 80);
    }

    void DisableAgent()
    {
        if (agent.enabled)
        {
            // Snap once, then stop the agent from moving the transform
            transform.position = agent.nextPosition;
            agent.ResetPath();
            agent.enabled = false;
        }

        // Give control back to Rigidbody2D
        enemyRB.isKinematic = false;        // or enemyRB.simulated = true;
        enemyRB.linearVelocity = Vector2.zero;
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
