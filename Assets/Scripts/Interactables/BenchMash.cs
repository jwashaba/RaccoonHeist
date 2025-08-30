using System;
using System.Collections;
using UnityEngine;

public class BenchMash : MonoBehaviour
{
    // Reference PlayerStates for weight blockage
    public PlayerStates pStates;
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
        // if passed bench threshold
        if (pStates.biscuitsAte >= 1)
        {
            temp = pStates.biscuitsAte;
            count = 3 * pStates.weight;
            mashState = true;
            pStates.biscuitsAte = 5; // set to some maximum that prevents player from moving
            pStates.movSpeed.SetMovementToZero();
            pStates.movSpeed.canDash = false;
            Debug.Log("Set");
        }
        else
        {
            pStates.hiddenState = true;
        }
    }

    // Function to check whether in mash state
    void checkMashState()
    {
        if (count == 0)
        {
            mashState = false;
            pStates.biscuitsAte = temp; // set back to previous biscuit quantity
            pStates.movSpeed.canDash = true;
            pStates.hiddenState = true;
        }
    }
}
