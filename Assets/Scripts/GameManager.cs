using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //define all managers and generators
    public MovementManager movementManager;
    public AudioManager audioManager;
    public UIManager uiManager;
    public LevelGenerator levelGenerator;
    
    //variables to keep track of the current lives and score
    private int score = 0;
    public int lives = 3;
    
    //variables for whether game is paused or in startgame stage
    public bool isPaused = false;
    public bool isGameStarted = true;

    private void Awake()
    {
        //get all the managers and init them
        movementManager.Init(this);
        
        //[TODO]: create respective init functions for the other managers and levelgenerator
        //audioManager.Init(this);
        //uiManager.Init(this);
        //levelGenerator.Init(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    IEnumerator StartGame() {
        lives = 3;
        //uIManager.Reset();
        movementManager.pacStudentController.MoveToStart();
        //audioManager.PlayStartMusic();
        yield return null;
        //audioManager.PlayNormalMusic();
        //uIManager.StartTimer();
        Play();
    }
    
    public void AddScore(int x) {
        score += x;
        //uIManager.SetScore(score);
    }
    
    public void Play() {
        isGameStarted = true;
        isPaused = false;
        //movementManager.cherryController.StartCherrySpawn();
    }
    
    public void Pause() {
        isPaused = true;
        //movementManager.cherryController.StopCherrySpawn();
    }
    
    public void GameOver() {
        Pause();
        movementManager.pacStudentController.pacStudentAnimator.enabled = false;
        SaveHighScore();
        //uIManager.ShowGameOver();
        Invoke("ShowStartScene", 3);
    }
    
    public void SaveHighScore() {
        int currentHighScore = PlayerPrefs.GetInt("highscore", 0);
        float currentTime = PlayerPrefs.GetFloat("highscoreTime", 0.0f);

        //[TODO]: Save game logic
        /*if(score > currentHighScore) {
            PlayerPrefs.SetInt("highscore", score);
            //PlayerPrefs.SetFloat("highscoreTime", uIManager.GetTimePlayed());
        } else if(score == currentHighScore) {
            if(currentTime > uIManager.GetTimePlayed()) {
                PlayerPrefs.SetInt("highscore", score);
                //PlayerPrefs.SetFloat("highscoreTime", uIManager.GetTimePlayed());
            }
        }*/
    }

    public void ShowStartScene() {
        SceneManager.LoadScene(0);
    }

    public void ShowLevel1() {
        SceneManager.LoadScene(1);
    }
}
