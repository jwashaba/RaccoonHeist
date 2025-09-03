using System;
using System.Collections;
using UnityEngine;

public class BenchMash : MonoBehaviour
{
    // Reference PlayerStates for weight blockage
    public PlayerStates pStates;
    public int weightTreshold = 2; // at this weight or higher, player must button mash
    public int temp;
    public int count;
    // Create Mashing State
    bool mashState = false;


    // Create a function for when player enters hidden area
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            {
                Debug.Log("Mash Area");
                benchMash();
            }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            pStates.hiddenState = false;
        }
    }
    
    void Update()
    {
        // Mash base on key
        if (mashState == true)
        {
            if ((count > 0))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    count--;
                    Debug.Log("Mashing");
                }
            }
            checkMashState();
        }
    }

    // Function to mash when going into bench
    void benchMash()
    {
        temp = pStates.biscuitsAte;
        
        // if passed bench threshold
        if (pStates.weight >= weightTreshold)
        {
            count = 3 * pStates.weight;
            mashState = true;
            pStates.mashState = true;
            pStates.biscuitsAte = 99;
            pStates.movSpeed.SetMovementToZero();
            pStates.movSpeed.canDash = false;
            Debug.Log("Set");
        }
        else
        {
            count = 0;
            checkMashState();
        }
    }

    // Function to check whether in mash state
    void checkMashState()
    {
        Debug.Log(count);
        if (count <= 0)
        {
            Debug.Log("elo2");
            mashState = false;
            pStates.mashState = false;
            pStates.biscuitsAte = temp;
            pStates.movSpeed.canDash = true;
            pStates.hiddenState = true;
        }
    }
}
