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
    public int lives = 4;
    
    //variables for whether game is paused or in startgame stage
    public bool isGameStarted = false;
    public bool isPaused = true;

    private void Awake()
    {
        //get all the managers and init them
        movementManager.Init(this);
        audioManager.Init(this);
        uiManager.Init(this);
        levelGenerator.Init(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    IEnumerator StartGame() {
        lives = 4;
        uiManager.Reset();
        movementManager.pacStudentController.MoveToStart();
        audioManager.PlayStartMusic();
        yield return StartCoroutine(uiManager.CountDown(3));
        audioManager.PlayNormalMusic();
        uiManager.StartTimer();
        Play();
    }
    
    public void AddScore(int x) {
        score += x;
        uiManager.SetScore(score);
    }
    
    public void Play() {
        isGameStarted = true;
        isPaused = false;
        movementManager.cherryController.StartCherrySpawn();
    }
    
    public void Pause() {
        isPaused = true;
        movementManager.cherryController.StopCherrySpawn();
    }
    
    public void GameOver() {
        Pause();
        movementManager.pacStudentController.pacStudentAnimator.enabled = false;
        SaveHighScore();
        uiManager.ShowGameOver();
        Invoke("ShowStartScene", 3);
    }
    
    public void SaveHighScore() {
        int currentHighScore = PlayerPrefs.GetInt("highscore", 0);
        float currentTime = PlayerPrefs.GetFloat("highscoreTime", 0.0f);


        if(score > currentHighScore) {
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.SetFloat("highscoreTime", uiManager.GetTimePlayed());
        } else if(score == currentHighScore) {
            if(currentTime > uiManager.GetTimePlayed()) {
                PlayerPrefs.SetInt("highscore", score);
                PlayerPrefs.SetFloat("highscoreTime", uiManager.GetTimePlayed());
            }
        }
    }

    public void ShowStartScene() {
        SceneManager.LoadScene(0);
    }

    public void ShowLevel1() {
        SceneManager.LoadScene(1);
    }
}
