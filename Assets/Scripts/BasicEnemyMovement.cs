using UnityEngine;

public class BasicEnemyMovement : MonoBehaviour
{
    public SpriteRenderer enemySR;
    public Rigidbody2D enemyRB;
    public float moveSpeed;
    public float patrolDist;
    public float idleTime;
    private Vector3 startingPosition;
    private int direction = 1;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= idleTime)
            {
                float clampedX = startingPosition.x + direction * patrolDist;
                transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
                isWaiting = false;
                waitTimer = 0f;
                direction *= -1;
                enemySR.flipX = !enemySR.flipX;
            }
            return;
        }

        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - startingPosition.x) >= patrolDist) {
            isWaiting = true;
        }
    }
}