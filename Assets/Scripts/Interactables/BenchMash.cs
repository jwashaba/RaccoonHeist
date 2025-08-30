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
        if (other.CompareTag("character"))
            {
                Debug.Log("Mash Area");
                benchMash();
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
        }
        checkMashState();
    }

    // Function to mash when going into bench
    void benchMash()
    {
        if (pStates.biscuitsAte >= 1)
        {
            temp = pStates.biscuitsAte;
            count = 3 * pStates.weight;
            mashState = true;
            pStates.biscuitsAte = 5;
            Debug.Log("Set");
        }
    }

    // Function to check whether in mash state
    void checkMashState()
    {
        if (count == 0)
        {
            mashState = false;
        }
        else if (count == 1)
        {
            pStates.biscuitsAte = temp;
        }
    }
}
