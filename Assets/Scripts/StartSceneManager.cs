using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField]
    GameObject highscore;
    [SerializeField]
    GameObject highscoreTime;
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateHighScore();
    }
    
    public void UpdateHighScore() {
        int hs = PlayerPrefs.GetInt("highscore", 0);
        float hsTime = PlayerPrefs.GetFloat("highscoreTime", 0.0f);
        highscore.GetComponent<TextMeshProUGUI>().SetText(hs.ToString());
        highscoreTime.GetComponent<TextMeshProUGUI>().SetText(FormatTimeToString(hsTime));
    }
    public string FormatTimeToString(float time) {
        int mins = Mathf.FloorToInt(time / 60.0f);
        int seconds = Mathf.FloorToInt(time - mins * 60);
        int ms = (int)((time - seconds - mins * 60) * 1000);
        if(ms >= 100)
            ms = ms/10;
        return string.Format("{0:D2}:{1:D2}:{2:D2}", mins, seconds, ms);
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
