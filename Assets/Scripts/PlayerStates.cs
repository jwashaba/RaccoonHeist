using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    //Reference Player Movement
    public PlayerMovement movSpeed;

    //Create a variable tracking weight phases
    //Create a variable tracking biscuits eaten
    //Create a variable indicatiing hidden status
    //Create a variable indicating detection 
    public int weight = 1;
    public int biscuitsAte = 0;
    public bool hiddenStatus = true;
    public int detectStat = 0;
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
    }

    //Create a function indicating states of weight phases
    void weightPhaseEffect(int weight)
    {
        if (weight == 1)
        {
            movSpeed.speed = 3;
        }
        else if (weight == 2)
        {
            movSpeed.speed = 2;
        }
        else if (weight == 3)
        {
            movSpeed.speed = 1;
        }
        else if (weight == 4)
        {
            movSpeed.speed = 0;
        }
    }

    //Create a function checking detection status based on value
    void checkDetectValue()
    {
        // Can discuss what other detection values do later
        // Guard will get suspicious and follow towards the raccoon
        if (detectStat == 4)
        {
            // Make reference to guard which we don't have rn
        }

        // Set 5 as the value to get caught
        else if (detectStat == 5)
        {
            hiddenStatus = false;
        }

    }
    
    /* Note: Later on when creating guard script, reference this script PlayerStates &
    create a function that will inc and dec the detection value based on their fov. */

}
