using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    //Reference Player Movement
    public PlayerMovement movSpeed;
    
    private SpriteRenderer _sp;

    //Create a variable tracking weight phases
    //Create a variable tracking biscuits eaten
    //Create a variable indicatiing hidden status
    public int weight = 1;
    public int biscuitsAte = 0;
    public bool hiddenState = false;
    public float detection = 0f;
    private float CooldownTime = 0f;

    void Start()
    {
        _sp = GetComponent<SpriteRenderer>();
    }
    
    // Update is called once per frame
    void Update()
    {
        // Debug.Log(detection);
        
        if (hiddenState)
        {
            _sp.color = Color.grey;
        }
        else
        {
            if (detection > 0f) detection -= Time.deltaTime;
            _sp.color = Color.white;
        }
        
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
    public void biscuitsAmt(int biscuits)
    {
        // Note: Temp Biscuit Amt needed for weight until we all decide

        if (biscuits == 0)
        {
            weight = 1;
        }
        else if (biscuits < 4)
        {
            weight = 1;
        }
        else if (biscuits < 8)
        {
            weight = 2;
        }
        else if (biscuits < 12)
        {
            weight = 3;
        }
        else if (biscuits < 16)
        {
            weight = 4;
        }
        else if (biscuits < 99)
        {
            weight = 5;
        }
        else
        {
            weight = 6;
        }

        weightPhaseEffect(weight);
    }

    public void IncrementDetection(float weight)
    {
        if (detection < 1f) detection += 2f * weight * Time.deltaTime;
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
            Debug.Log("HERE");
            movSpeed.moveSpeed = 0f;
        }
    }

    // Create Gameover Check function
    void gameOverCheck()
    {
        return;
    }
}
