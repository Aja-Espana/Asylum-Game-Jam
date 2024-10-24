using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;

    public bool gameLoaded = true;
    public bool sceneLoaded = false;
    public bool loadPlayer = true;

    public float progressValue;


    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("GameManager").AddComponent<GameManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    public void RunCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public void RestartScene()
    {
        // Get the active scene and reload it

        /*
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.UnloadScene(currentScene);
        SceneManager.LoadScene(currentScene.name);
        */
        
        Scene currentScene = SceneManager.GetActiveScene();
        LoadGameplayScene(currentScene.name);

    }

    public void GoBackToMenu()
    {
        /*
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.UnloadScene(currentScene);
        SceneManager.LoadScene("TitleScreen");
        */

        LoadGameplayScene("TitleScreen");    
    }

    public void LoadGameplayScene(string sceneName)
    {
        StartCoroutine(LoadGameplaySceneCoroutine(sceneName));
    }

    private IEnumerator LoadGameplaySceneCoroutine(string sceneName)
    {
        gameLoaded = false;
        sceneLoaded = false;
        loadPlayer = false;

        Scene currentScene = SceneManager.GetActiveScene();

        //--------------------------------------------
        uiManager.FadeToBlack(1f);
        
        while(uiManager.isFading){
            yield return null;
        }
        //--------------------------------------------
        AsyncOperation interimOperation = SceneManager.LoadSceneAsync("LoadScreen", LoadSceneMode.Additive);
        yield return interimOperation;

        Scene loadScreen = SceneManager.GetSceneByName("LoadScreen");

        while(!loadScreen.isLoaded){
            yield return null;
        }

        SceneManager.SetActiveScene(loadScreen);

        Debug.Log("Loading screen finished loading");

        //--------------------------------------------

        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentScene);
        yield return unloadOperation;
        //Resources.UnloadUnusedAssets();

        while(!unloadOperation.isDone){
            yield return null;
        }

        Debug.Log("Unloading previous scene finished");

        //--------------------------------------------

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return loadOperation;

        loadPlayer = true;

        Scene newScene = SceneManager.GetSceneByName(sceneName);

        uiManager.DisableCanvas();
        
        while (!newScene.isLoaded)
        {
            progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            yield return null;
        }
        
        SceneManager.SetActiveScene(newScene);

        sceneLoaded = true;

        //--------------------------------------------

        interimOperation = SceneManager.UnloadSceneAsync("LoadScreen");

        while (!interimOperation.isDone){
            yield return null;
        }

        Debug.Log("Loading Screen Unloaded");

        //--------------------------------------------

        uiManager.EnableCanvas();

        uiManager.Unfade(1f);

        while(uiManager.isFading){
            yield return null;
        }

        uiManager.DisableCanvas();

        gameLoaded = true;
    }
}
