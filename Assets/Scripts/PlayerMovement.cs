using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public SpriteRenderer playerSR;
    public Rigidbody2D playerRB;
    public float moveSpeed;
    private Vector2 moveInput = Vector2.zero;
    public float dashSpeed;
    public float dashTime;
    public float dashCooldown;
    private bool canDash = true;
    private bool isDashing = false;
    private Vector2 dashDir = Vector2.down;

    // TEMP ACCESS TO PLAYER CAMERA & FLASHLIGHT -- REMOVE ON MERGE
    // public Camera _cam;
    // public Flashlight _light;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        // TEMP FLASHLIGHT MOVEMENT FOR PLAYER -- REMOVE ON MERGE
        // Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Vector3 playerPos = playerRB.position;
        // Vector2 dir = (mouseWorld - playerPos); 
        // float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // _light.angle = angle + _light.fov / 2f;

        float x = 0f, y = 0f;

        if (!isDashing)
        {
            //horizontal movement
            if (Input.GetKey(KeyCode.A))
            {
                x = -1f;
                playerSR.flipX = false;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                x = 1f;
                playerSR.flipX = true;
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
            }

            if (Input.GetKeyDown(KeyCode.Space) && canDash)
            {
                StartCoroutine(DashTiming());
            }
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
        canDash = true;
    }
}
