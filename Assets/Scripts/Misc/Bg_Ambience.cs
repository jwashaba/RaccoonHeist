using UnityEngine;
using UnityEngine.SceneManagement;

public class Bg_Ambience : MonoBehaviour
{
    int loop = 0;
    void Update()
    {
        GameObject Player = GameObject.Find("Player");
        GameObject some = GameObject.Find("Sound_ambient_bg");
        // Create bg music for in game
        if (Player != null)
        {
            if (loop == 0)
            {
                SoundManager.Instance.Play(SoundManager.SoundType.ambient_bg);
                loop = 1;
            }
            else if (some == null)
            {
                Debug.Log("What");
                loop = 0;
            }
        }
    }
}

