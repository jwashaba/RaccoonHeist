using System.Numerics;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class Bench : MonoBehaviour
{
    // Reference PlayerStates for weight blockage
    public PlayerStates pStates;
    public PlayerMovement pmov;

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
            // GetComponent<Collider2D>().enabled = false;
        }        
        else if (pStates.weight == 5)
        {
            // Turn on colider to stop
            // GetComponent<Collider2D>().enabled = true;
        }
    }
 

}
