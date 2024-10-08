using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartScene()
    {
        // Get the active scene and reload it
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.UnloadScene(currentScene);
        SceneManager.LoadScene(currentScene.name);
    }

    public void GoBackToMenu()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.UnloadScene(currentScene);
        SceneManager.LoadScene("TitleScreen");        
    }
}
