using UnityEngine;

public class ReturnScene : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.Instance.LoadScene("StartMenu");
        }
    }
}
