using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    GameManager gameManager;
    
    //game music and start music
    [SerializeField]
    private AudioSource startMusic;
    [SerializeField]
    private AudioSource gameMusic;
    
    //music for when pacstudent dies
    [SerializeField]
    private AudioSource deadMusic;
    
    //music for when ghosts are edible
    [SerializeField] 
    private AudioSource scaredMusic;
    
    //clip for when pacstudent eats a cherry
    [SerializeField]
    private AudioSource cherryClip;
    public void PlayEatSound() {
        cherryClip.Play();
    }
    
    //clip for when pacstudent collides with a wall
    [SerializeField]
    private AudioSource collideClip;
    public void PlayCollisionSound() {
        collideClip.Play();
    }
    
    //clip for when ghost is eaten
    [SerializeField]
    private AudioSource dieClip;
    public void PlayDieSound() {
        dieClip.Play();
    }
    
    //clip for when pacstudent moves
    [SerializeField]
    private AudioSource moveClip;
    public void PlayMoveSound() {
        moveClip.Play();
    }
    
    //this keeps track of current music playing
    private AudioSource currentMusic;
    
    
    //initialise the AudioManager
    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        currentMusic = startMusic;
    }
    
    // Start is called before the first frame update
    
    //music/clip management now handled by other managers based on type of movement or game situation, audio manager just provides the variables and the corresponding public functions
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {

    }

    //switch to start music
    public void PlayStartMusic() {
        currentMusic.Stop();
        startMusic.Play();
        currentMusic = startMusic;
    }

    //switch to scared music
    public void PlayScaredMusic() {
        currentMusic.Stop();
        scaredMusic.Play();
        currentMusic = scaredMusic;
    }

    //switch to game music
    public void PlayNormalMusic() {
        currentMusic.Stop();
        gameMusic.Play();
        currentMusic = gameMusic;
    }
    
    //switch to dead music
    public void PlayDeadMusic() {
        currentMusic.Stop();
        deadMusic.Play();
        currentMusic = deadMusic;
    }
}