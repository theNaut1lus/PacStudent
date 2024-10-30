using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void LoadFirstLevel()
    {
        // Make sure the UIManager is not destroyed when loading a new scene
        //DontDestroyOnLoad(gameObject);
        
        //check if game scene exists and current scene is start scene , if yes then load it
        if (SceneManager.sceneCountInBuildSettings.Equals(2) && SceneManager.GetActiveScene().buildIndex.Equals(0))
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            //if not, load the first scene
            Debug.Log("GameScene not found");
        }
        
    }
    
    public void ExitToStartLevel()
    {
        //check if start scene exists and current scene is game scene , if yes then load it
        if (SceneManager.sceneCountInBuildSettings.Equals(2) && SceneManager.GetActiveScene().buildIndex.Equals(1))
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            //if not, load the first scene
            Debug.Log("StartScene not found");
        }
    }
            
}
