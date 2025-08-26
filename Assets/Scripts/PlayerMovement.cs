using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D playerRB;
    public SpriteRenderer playerSR;
    public float speed;

    // TEMP ACCESS TO PLAYER CAMERA & FLASHLIGHT -- REMOVE ON MERGE
    public Camera _cam;
    public Flashlight _light;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // TEMP FLASHLIGHT MOVEMENT FOR PLAYER -- REMOVE ON MERGE
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerPos = playerRB.position;
        Vector2 dir = (mouseWorld - playerPos); 
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _light.angle = angle + _light.fov / 2f;
    }

    // physics loop for movement
    void FixedUpdate()
    {
        float x = 0f, y = 0f;

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

        Vector2 input = new Vector2(x, y).normalized;
        playerRB.linearVelocity = input * speed;
    }
}
