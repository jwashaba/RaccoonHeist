using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

public class Bench : MonoBehaviour
{
    // Reference PlayerStates for weight blockage
    public PlayerStates pStates;

    void Update()
    {
        checkBench();
    }

    /* Create a function to adjust bench properties to
     each weight state*/
    void checkBench()
    {
        // Note: Will add more later when doing qte
        if (pStates.weight == 1)
        {
            GetComponent<Collider2D>().enabled = true;
        }
        else if (pStates.weight == 2)
        {
            // Later when adding qte
        }
        else if (pStates.weight == 3)
        {
            // Turn off colider to enter
            GetComponent<Collider2D>().enabled = false;
        }
    }


}
