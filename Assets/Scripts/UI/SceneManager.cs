using System.Collections;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject pauseScreen;

    public bool pauseScreenAccessable = false;
    public bool gameIsPaused = false;

    void Update()
    {
        if (!pauseScreenAccessable) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseScreen();
        }
    }

    void TogglePauseScreen()
    {
        gameIsPaused = !gameIsPaused;
        
        if (gameIsPaused)
        {
            pauseScreen.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            pauseScreen.SetActive(false);
            Time.timeScale = 1;
        }
    }
    
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator ProgressDelay()
    {
        print(Time.time);
        yield return new WaitForSecondsRealtime(5f);
        print(Time.time);
    }

    // very scuffed state-based setter
    private void HandleManagerStates(string sceneName)
    {
        switch (sceneName)
        {
            case "StartMenu":
                pauseScreenAccessable = false;
                gameIsPaused = false;
                
                pauseScreen.SetActive(false);

                break;
            case "Museum":
                pauseScreenAccessable = true;
                gameIsPaused = false;
                
                pauseScreen.SetActive(false);
                break;
        }
    }
    
    public async void LoadScene(string sceneName)
    {
        // player cannot toggle pause, and non-time-based processes in game remain paused
        pauseScreenAccessable = false;
        gameIsPaused = true;
        
        AsyncOperation scene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;
        
        loadingScreen.SetActive(true);

        do
        {
            await Awaitable.NextFrameAsync();
        } while (scene.progress < 0.9f);
        
        scene.allowSceneActivation = true;

        while (!scene.isDone)
        {
            await Awaitable.NextFrameAsync();
        }
        
        HandleManagerStates(sceneName);
        
        loadingScreen.SetActive(false);
    }
}
