using UnityEngine;
using System;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
    }
}
