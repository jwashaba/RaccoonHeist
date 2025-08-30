using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    //Reference Player Movement
    public PlayerMovement movSpeed;

    //Create a variable tracking weight phases
    //Create a variable tracking biscuits eaten
    //Create a variable indicatiing hidden status
    public int weight = 1;
    public int biscuitsAte = 0;
    public bool hiddenState = false;
    public bool detected = false;
    private float CooldownTime = 0f;

    // Update is called once per frame
    void Update()
    {
        // Test key for biscuits eaten for now
        CooldownTime -= Time.deltaTime;
        if (Input.GetMouseButton(0) && CooldownTime <= 0f)
        {
            CooldownTime = 3.0f;
            // biscuitsAte++;
        }
        biscuitsAmt(biscuitsAte);
        weightPhaseEffect(weight);

        // Game Over Check
        gameOverCheck();
    }

    // Create a function that sets weight according to biscuits
    void biscuitsAmt(int biscuits)
    {
        // Note: Temp Biscuit Amt needed for weight until we all decide

        if (biscuits == 0)
        {
            weight = 1;
        }
        else if (biscuits == 1)
        {
            weight = 2;
        }
        else if (biscuits == 2)
        {
            weight = 3;
        }
        else if (biscuits == 3)
        {
            weight = 4;
        }
        else if (biscuits == 4)
        {
            weight = 5;
        }
        else if (biscuits == 5)
        {
            weight = 6;
        }
    }

    //Create a function indicating states of weight phases
    void weightPhaseEffect(int weight)
    {
        if (weight == 1)
        {
            movSpeed.moveSpeed = 3;
        }
        else if (weight == 2)
        {
            movSpeed.moveSpeed = 2.5f;
        }
        else if (weight == 3)
        {
            movSpeed.moveSpeed = 2f;
        }
        else if (weight == 4)
        {
            movSpeed.moveSpeed = 1.5f;
        }
        else if (weight == 5)
        {
            movSpeed.moveSpeed = 1f;
        }
        else if (weight == 6)
        {
            movSpeed.moveSpeed = 0f;
        }
    }

    // Create Gameover Check function
    void gameOverCheck()
    {
        if (detected == true)
        {
            Debug.Log("Gameover!");
            biscuitsAte = 5;
        }
    }
}
