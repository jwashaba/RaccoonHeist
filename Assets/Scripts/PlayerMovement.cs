using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D playerRB;
    public SpriteRenderer playerSR;
    public float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
