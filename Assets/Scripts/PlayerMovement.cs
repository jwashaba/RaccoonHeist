using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public SpriteRenderer playerSR;
    public Rigidbody2D playerRB;
    public PlayerStates playerStates;
    public float moveSpeed;
    private Vector2 moveInput = Vector2.zero;
    public float dashSpeed;
    public float dashTime;
    public float dashCooldown;
    public bool canDash = true;
    public bool benchMashing = false;
    private bool isDashing = false;
    private Vector2 dashDir = Vector2.down;
    private int lastDir = 2;
    private int lastXDir = -1;
    public Animator animator;
    public Sprite idleUp1;
    public Sprite idleUp2;
    public Sprite idleUp3;
    public Sprite idleDown1;
    public Sprite idleDown2;
    public Sprite idleDown3;
    public Sprite idleLeft1;
    public Sprite idleLeft2;
    public Sprite idleLeft3;
    public Sprite dashUp1;
    public Sprite dashUp2;
    public Sprite dashUp3;
    public Sprite dashDown1;
    public Sprite dashDown2;
    public Sprite dashDown3;
    public Sprite dashLeft1;
    public Sprite dashLeft2;
    public Sprite dashLeft3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("facingDirection", 2);
        getVisualWeight(playerStates.weight);
    }

    int getVisualWeight(int weight)
    {
        if (weight == 1)
        {
            animator.SetInteger("weight", 1);
            return 1;
        }
        else if (weight == 2 || weight == 3)
        {
            animator.SetInteger("weight", 2);
            return 2;
        }
        else
        {
            animator.SetInteger("weight", 3);
            return 3;
        }
    }

    int dirFromVect(Vector2 v)
    {
        return (Mathf.Abs(v.x) >= Mathf.Abs(v.y)) ? 1 : (v.y >= 0 ? 0 : 2);
    }

    void flipX(Vector2 v)
    {
        if (Mathf.Abs(v.x) > 0.0001f) lastXDir = (v.x > 0f) ? +1 : -1;
        playerSR.flipX = lastXDir > 0; // true when facing right
    }

    void SetIdle()
    {
        animator.SetBool("isWalking", false);
        if (animator.enabled) animator.enabled = false;

        int f = lastDir;
        int g = getVisualWeight(playerStates.weight);
        switch (f)
        {
            case 0:
                playerSR.sprite = (g == 1) ? idleUp1 : (g == 2) ? idleUp2 : idleUp3;
                break;
            case 1:
                playerSR.sprite = (g == 1) ? idleLeft1 : (g == 2) ? idleLeft2 : idleLeft3;
                playerSR.flipX = lastXDir > 0;
                break;
            case 2:
                playerSR.sprite = (g == 1) ? idleDown1 : (g == 2) ? idleDown2 : idleDown3;
                break;
        }
        // Debug.Log($"idle sprite set to: {enemySR.sprite?.name} (facing={f})");
    }

    void SetWalking(int facing, Vector2 dir)
    {
        lastDir = facing;
        if (!animator.enabled) animator.enabled = true;
        animator.SetBool("isWalking", true);
        animator.SetInteger("facingDirection", facing);
        getVisualWeight(playerStates.weight);

        if (facing == 1) flipX(dir);
    }

    void SetDash(Vector2 dir)
    {
        animator.SetBool("isWalking", false);
        if (animator.enabled) animator.enabled = false;

        int f = dirFromVect(dir);
        lastDir = f;
        int g = getVisualWeight(playerStates.weight);
        switch (f)
        {
            case 0:
                playerSR.sprite = (g == 1) ? dashUp1 : (g == 2) ? dashUp2 :  dashUp3;
                break;
            case 1:
                playerSR.sprite = (g == 1) ?  dashLeft1 : (g == 2) ? dashLeft2 : dashLeft3;
                playerSR.flipX = lastXDir > 0;
                break;
            case 2:
                playerSR.sprite = (g == 1) ? dashDown1 : (g == 2) ? dashDown2 : dashDown3;
                break;
        }
        // Debug.Log($"idle sprite set to: {enemySR.sprite?.name} (facing={f})");
    }

    // Update is called once per frame
    void Update()
    {
        // stop ANY non-time-based controller inputs/processes when game paused
        if (SceneManager.Instance.gameIsPaused) return;

        float x = 0f, y = 0f;

        if (!isDashing)
        {
            //horizontal movement
            if (Input.GetKey(KeyCode.A))
            {
                x = -1f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                x = 1f;
            }

            // vertical movement
            if (Input.GetKey(KeyCode.W))
            {
                y = 1f;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                y = -1f;
            }

            moveInput = new Vector2(x, y).normalized;

            if (moveInput != Vector2.zero)
            {
                dashDir = moveInput;
                int facing = dirFromVect(moveInput);
                SetWalking(facing, moveInput);
            }
            else
            {
                animator.SetBool("isWalking", false);
                SetIdle();
            }

            if (Input.GetKeyDown(KeyCode.Space) && canDash)
            {
                StartCoroutine(DashTiming());
                SoundManager.Instance.Play(SoundManager.SoundType.Dash);
            }
        }
        else
        {
           SetDash(dashDir);
        }
    }

    // physics loop for movement
    void FixedUpdate()
    {
        if (isDashing)
        {
            playerRB.linearVelocity = dashDir * dashSpeed;
        }
        else
        {
            playerRB.linearVelocity = moveInput * moveSpeed;
        }
    }

    IEnumerator DashTiming()
    {
        canDash = false;
        isDashing = true;
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        
        if (benchMashing)
        {
            canDash = false;
        }
        else
        {
            canDash = true;
        }
    }
    
    public void SetMovementToZero()
    {
        moveInput = Vector2.zero;
        playerRB.linearVelocity = Vector2.zero;
        isDashing = false;
    }
}
