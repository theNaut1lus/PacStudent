using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField]
    GameObject highscore;
    [SerializeField]
    GameObject highscoreTime;
    [SerializeField]
    Button level1Button;
    
    void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 0)
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
    
    public void StartGame() {
        SceneManager.LoadScene("GameScene");
    }
    
    // Start is called before the first frame update
    void Start()
    {
        level1Button.onClick.AddListener(LoadLevel1);
    }
    
    public void LoadLevel1() {
        SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
