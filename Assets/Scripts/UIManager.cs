using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //game manager
    private GameManager gameManager;
    
    //game score
    [SerializeField]
    GameObject score;
    
    //Health bar, 4 different health states based on my sprites, and 1 for when the player loses all lives
    [SerializeField] 
    private Sprite[] healthSprites = new Sprite[5];

    [SerializeField] 
    private GameObject health;
    
    //initial timer
    [SerializeField] 
    private GameObject timer;
    
    //timer when scared ghost active
    [SerializeField]
    GameObject ghostTimer;
    
    //countdown timer
    [SerializeField]
    GameObject countDown;
    
    //game over screen
    [SerializeField]
    GameObject gameOver;
    
    float startTime = -1.0f;
    
    
    //initialise the UIManager
    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (startTime != -1.0f)
        {
            UpdateTimer();
        }
    }
    
    public IEnumerator CountDown(int t) {
        countDown.SetActive(true);
        countDown.GetComponent<Animator>().Play("CountDown");
        float expired = Time.time + t + 1;
        while(Time.time < expired) {
            float timeLeft = expired - Time.time;
            if(timeLeft > 1.0f) {
                countDown.GetComponent<TextMeshProUGUI>().SetText(Mathf.CeilToInt(timeLeft-1).ToString());
            } else {
                countDown.GetComponent<TextMeshProUGUI>().SetText("GO!");
            }
            yield return null;
        }
        countDown.SetActive(false);
    }

    public void Reset() {
        SetScore(0);
        DisableGhostTimer();
        ResetLives();
        gameOver.SetActive(false);
    }

    public void StartTimer() {
        startTime = Time.time;
    }

    public void UpdateTimer() {
        float timePast = GetTimePlayed();
        string text = FormatTimeToString(timePast);
        timer.GetComponent<TextMeshProUGUI>().SetText(text);
    }

    public string FormatTimeToString(float time) {
        int mins = Mathf.FloorToInt(time / 60.0f);
        int seconds = Mathf.FloorToInt(time - mins * 60);
        int ms = (int)((time - seconds - mins * 60) * 1000);
        if(ms >= 100)
            ms = ms/10;
        return string.Format("{0:D2}:{1:D2}:{2:D2}", mins, seconds, ms);
    }

    public void SetScore(int x) {
        score.GetComponent<TextMeshProUGUI>().SetText(x.ToString());
    }

    public void SetGhostTimer(float value) {
        ghostTimer.SetActive(true);
        ghostTimer.GetComponent<TextMeshProUGUI>().SetText(Mathf.CeilToInt(value).ToString());
    }

    public void DisableGhostTimer() {
        ghostTimer.SetActive(false);
    }

    //change sprite of health gameobject (TMP image) based on the number of lives
    public void LoseLife()
    {
        Debug.Log("LoseLife");
        int lives = --gameManager.lives;
        if(lives >= 0 && lives <= 4) 
        {
            health.GetComponent<Image>().sprite = healthSprites[lives];
            Debug.Log($"health: {lives}, current sprite: {healthSprites[lives]}");
        }
    }

    public void ResetLives() {
        health.GetComponent<Image>().sprite = healthSprites[4];
    }

    public float GetTimePlayed() {
        return Time.time - startTime;
    }

    public void ShowGameOver() {
        gameOver.SetActive(true);
        startTime = -1.0f;
    }
            
}
