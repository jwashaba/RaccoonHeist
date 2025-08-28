using UnityEngine;

public class SceneLoadButton : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        SceneManager.Instance.LoadScene(sceneName);
    }
}
